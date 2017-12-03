using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DebtsRegister.Core
{
    public class AchieversDoc
    {
        public static string Name = "Achievers";

        [BsonId]
        public ObjectId Id { get; set; }

        public HashSet<string> MaxDueDebtsTotalPersonIds
            { get; set; }

        public HashSet<string> HistoricallyCreditedMaxTotalPersonIds
            { get; set; }

        public HashSet<string> BestDebtorIdsByRepaidFractionThenTotal
            { get; set; }
    }
}