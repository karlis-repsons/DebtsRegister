using System;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;

namespace DebtsRegister.Core
{
    /*
     * Manages data of document AchieversDoc
     * and table CurrentPeopleStatisticsDocuments.
     * Listens to IDebtDealsRegister events: DebtDealReceived,
     * DebtDealAdded.
     */
    public class AchieversRegister : IAchieversRegister
    {
        public AchieversRegister(
            TablesDbContext dbc,
            TablesDbContextForReader rdbc,
            IMongoCollection<AchieversDoc> achieversCollection,
            IDebtDealsRegister debtDealsRegister,
            IPersonDebtsRegister personDebtsRegister
        ) {
            this.dbc = dbc;
            this.rdbc = rdbc;
            this.mongoCollection = achieversCollection;
            this.personDebtsRegister = personDebtsRegister;

            debtDealsRegister.DebtDealReceived += this.OnDebtDealReceived;
            debtDealsRegister.DebtDealAdded += this.OnDebtDealAdded;

            this.ByID = new AchieversByID(() => this.currentAchieversDoc);

            this.InitializeDocument();
        }

        public IAchieversByID ByID { get; private set; }


        protected virtual void OnDebtDealReceived(
            object o_debtDealsRegister, DebtDealReceivedEventData evData
        ) {
            var newDoc = new AchieversDoc() { Id = ObjectId.GenerateNewId() };
            this.SetIdsOfPeopleWithMaxTotalDueDebts(newDoc, evData);
            this.SetIdsOfPeopleWhoCreditedMaxHistoricalTotal(newDoc, evData);
            this.SetIdsOfBestDebtorsByRepaidFractionThenTotal(newDoc, evData);

            if (this.IsDocumentContentDifferent(newDoc, this.currentAchieversDoc)) {
                this.SaveDocumentInDB(newDoc);
                
                PeopleStatisticsDocumentRow achieversDocRow
                    = this.dbc.CurrentPeopleStatisticsDocuments
                        .Where(sd => sd.DocumentName == AchieversDoc.Name)
                        .FirstOrDefault();
                bool shouldAddNewRow = false;
                if (achieversDocRow == null) {
                    shouldAddNewRow = true;
                    achieversDocRow = new PeopleStatisticsDocumentRow();
                }

                achieversDocRow.DocumentName = AchieversDoc.Name;
                achieversDocRow.DocumentId = newDoc.Id.ToString();
                
                if (shouldAddNewRow)
                    this.dbc.CurrentPeopleStatisticsDocuments
                        .Add(achieversDocRow);

                this.dbc.SaveChanges();

                this.currentAchieversDoc = newDoc;
            }

            // keep execution serialized until DebtDealAdded event is handled.
        }

        protected virtual void OnDebtDealAdded(
            object o, DebtDealAddedEventData evData
        ) {
            this.RemoveObsoleteDocumentsFromDB(this.currentAchieversDoc.Id);
        }


        private void InitializeDocument() {
            AchieversDoc result = GetDbDoc() ?? new AchieversDoc();
            this.currentAchieversDoc = result;

            AchieversDoc GetDbDoc() {
                PeopleStatisticsDocumentRow achieversDocRow
                    = this.rdbc.CurrentPeopleStatisticsDocuments
                        .Where(sd => sd.DocumentName == AchieversDoc.Name)
                        .FirstOrDefault();

                if (achieversDocRow == null)
                    return null;

                var mongoObjectId = new ObjectId(achieversDocRow.DocumentId);
                return this.mongoCollection.AsQueryable()
                    .Where(ad => ad.Id == mongoObjectId)
                    .FirstOrDefault();
            }
        }

        private void SetIdsOfPeopleWithMaxTotalDueDebts(
            AchieversDoc achieversDoc,
            DebtDealReceivedEventData evData
        ) {
            HashSet<string> previousIds
                = this.currentAchieversDoc.MaxDueDebtsTotalPersonIds;
            var result = new HashSet<string>(1 + previousIds?.Count ?? 0);

            decimal initialMax = 0;
            if (previousIds != null && previousIds.Count > 0)
                initialMax = GetDbDueDebtsTotalForPerson(previousIds.First());

            if (evData.Analysis.IsPayback == false) {
                decimal newTakerDebtsTotal
                    = GetDbDueDebtsTotalForPerson(evData.Deal.TakerId);

                if (   newTakerDebtsTotal > initialMax
                    && newTakerDebtsTotal > DebtConstants.MaxZeroEquivalent
                )
                    result.Add(evData.Deal.TakerId);
                else
                if (      Math.Abs(newTakerDebtsTotal - initialMax)
                       <= DebtConstants.ValueRelativeError * initialMax
                    &&
                       initialMax > DebtConstants.MaxZeroEquivalent
                ) {
                    if (previousIds != null)
                        result.UnionWith(previousIds);
                    result.Add(evData.Deal.TakerId);
                }
            }
            else if (  previousIds != null
                    && previousIds.Count == 1
                    && previousIds.Contains(evData.Deal.GiverId)
            ) {
                decimal newMaxTotal = 0;
                foreach (PersonTotalsRow t in
                    this.rdbc.CurrentTotalsPerPerson
                            .OrderByDescending(ct => ct.DueDebtsTotal)
                ) {
                    if (t.DueDebtsTotal <= DebtConstants.MaxZeroEquivalent)
                        break;

                    if (result.Count == 0) {
                        result.Add(t.PersonId);
                        newMaxTotal = t.DueDebtsTotal;
                        continue;
                    }

                    if (   t.DueDebtsTotal - newMaxTotal
                        <= DebtConstants.ValueRelativeError * newMaxTotal
                    )
                        result.Add(t.PersonId);
                    else
                        break;
                }
            }

            achieversDoc.MaxDueDebtsTotalPersonIds
                = result.Count > 0 ? result : null;

            decimal GetDbDueDebtsTotalForPerson(string personId) {
                return this.dbc.CurrentTotalsPerPerson
                    .Where(ct => ct.PersonId == personId)
                    .Select(ct => ct.DueDebtsTotal)
                    .FirstOrDefault();
            }
        }

        private void SetIdsOfPeopleWhoCreditedMaxHistoricalTotal(
            AchieversDoc achieversDoc,
            DebtDealReceivedEventData evData
        ) {
            HashSet<string> previousIds = this.currentAchieversDoc
                .HistoricallyCreditedMaxTotalPersonIds;
            var result = new HashSet<string>(1 + previousIds?.Count ?? 0);

            decimal initialMax = 0;
            if (previousIds != null && previousIds.Count > 0)
                initialMax = GetDbTotalForPerson(previousIds.First());

            if (evData.Analysis.IsPayback == false) {
                decimal newGiverTotal
                    = GetDbTotalForPerson(evData.Deal.GiverId);

                if (   newGiverTotal > initialMax
                    && newGiverTotal > DebtConstants.MaxZeroEquivalent
                )
                    result.Add(evData.Deal.GiverId);
                else 
                if (      Math.Abs(newGiverTotal - initialMax)
                       <= DebtConstants.ValueRelativeError * initialMax
                    &&
                       initialMax > DebtConstants.MaxZeroEquivalent
                ) {
                    if (previousIds != null)
                        result.UnionWith(previousIds);
                    result.Add(evData.Deal.GiverId);
                }
            }

            if (result.Count == 0 && previousIds != null)
                result.UnionWith(previousIds);

            achieversDoc.HistoricallyCreditedMaxTotalPersonIds
                = result.Count > 0 ? result : null;

            decimal GetDbTotalForPerson(string personId) {
                return this.dbc.CurrentTotalsPerPerson
                    .Where(ct => ct.PersonId == personId)
                    .Select(ct => ct.HistoricallyCreditedInTotal)
                    .FirstOrDefault();
            }
        }

        private void SetIdsOfBestDebtorsByRepaidFractionThenTotal(
            AchieversDoc achieversDoc,
            DebtDealReceivedEventData evData
        ) {
            if (evData.Analysis.IsPayback != true)
                return;

            var previousIds = this.currentAchieversDoc
                    .BestDebtorIdsByRepaidFractionThenTotal;
            var potentialBestDebtors = new HashSet<string>(
                    1 + previousIds?.Count ?? 0) { evData.Deal.GiverId };
            if (previousIds != null)
                potentialBestDebtors.UnionWith(previousIds);

            HashSet<string> result = GetBestDebtors(potentialBestDebtors);
            achieversDoc.BestDebtorIdsByRepaidFractionThenTotal
                = result.Count > 0 ? result : null;

            HashSet<string> GetBestDebtors(HashSet<string> input) {
                if (input.Count == 0)
                    return input;

                var table
                    = input.Select(id =>
                        new {
                            Id = id,
                            RepaidFraction
                                = this.personDebtsRegister
                                    .GetRepaidFraction(id),
                            HistoricallyOwedInTotal
                                = this.personDebtsRegister
                                    .GetTotals(id).HistoricallyOwedInTotal
                        })
                    .OrderByDescending(r => r.RepaidFraction ?? 0)
                    .ThenByDescending(r => r.HistoricallyOwedInTotal);

                var firstBestRow = table.First();
                decimal bestRFValue = firstBestRow.RepaidFraction.Value;
                decimal maxTotal = firstBestRow.HistoricallyOwedInTotal;

                if (   bestRFValue <= DebtConstants.ValueRelativeError
                    || maxTotal <= DebtConstants.MaxZeroEquivalent
                )
                    return new HashSet<string>(0);

                return table
                    .Where(r =>
                                r.RepaidFraction == bestRFValue
                             && r.HistoricallyOwedInTotal == maxTotal)
                    .Select(r => r.Id).ToHashSet();
            }
        }

        private bool IsDocumentContentDifferent(
            AchieversDoc doc1, AchieversDoc doc2
        ) {
            return PointToDifferentSets(
                        d => d.MaxDueDebtsTotalPersonIds)
                || PointToDifferentSets(
                        d => d.HistoricallyCreditedMaxTotalPersonIds)
                || PointToDifferentSets(
                        d => d.BestDebtorIdsByRepaidFractionThenTotal);

            bool PointToDifferentSets(
                Func<AchieversDoc, HashSet<string>> fieldGetter
            ) {
                HashSet<string> s1 = fieldGetter(doc1) ?? new HashSet<string>(0),
                                s2 = fieldGetter(doc2) ?? new HashSet<string>(0);

                return s1.SetEquals(s2) == false;
            }
        }

        private void SaveDocumentInDB(AchieversDoc doc) {
            this.mongoCollection.InsertOne(doc);
        }

        private void RemoveObsoleteDocumentsFromDB(
            ObjectId currentDocumentId
        ) {
            FilterDefinition<AchieversDoc> filter
                = Builders<AchieversDoc>.Filter
                    .Ne(ad => ad.Id, currentDocumentId);

            this.mongoCollection.DeleteMany(filter);
        }


        private readonly TablesDbContext dbc;
        private readonly TablesDbContextForReader rdbc;
        private readonly IMongoCollection<AchieversDoc> mongoCollection;
        private readonly IPersonDebtsRegister personDebtsRegister;
        private AchieversDoc currentAchieversDoc;
    }
}