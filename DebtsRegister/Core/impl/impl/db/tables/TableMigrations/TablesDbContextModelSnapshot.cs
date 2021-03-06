﻿// <auto-generated />
using DebtsRegister.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace DebtsRegister.Core.TableMigrations
{
    [DbContext(typeof(TablesDbContext))]
    partial class TablesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DebtsRegister.Core.DebtDealAnalysisRow", b =>
                {
                    b.Property<long>("DebtDealId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsPayback");

                    b.HasKey("DebtDealId");

                    b.ToTable("DebtDealsAnalysis");
                });

            modelBuilder.Entity("DebtsRegister.Core.DebtDealRow", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("GiverId")
                        .HasMaxLength(20);

                    b.Property<string>("TakerId")
                        .HasMaxLength(20);

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("GiverId");

                    b.HasIndex("TakerId");

                    b.HasIndex("Time");

                    b.ToTable("DebtDeals");
                });

            modelBuilder.Entity("DebtsRegister.Core.DebtRow", b =>
                {
                    b.Property<string>("CreditorId")
                        .HasMaxLength(20);

                    b.Property<string>("DebtorId")
                        .HasMaxLength(20);

                    b.Property<decimal>("DebtTotal");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("CreditorId", "DebtorId");

                    b.HasIndex("CreditorId");

                    b.HasIndex("DebtorId");

                    b.ToTable("CurrentDebts");
                });

            modelBuilder.Entity("DebtsRegister.Core.PeopleStatisticsDocumentRow", b =>
                {
                    b.Property<string>("DocumentName")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(45);

                    b.Property<string>("DocumentId")
                        .HasMaxLength(24);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("DocumentName");

                    b.ToTable("CurrentPeopleStatisticsDocuments");
                });

            modelBuilder.Entity("DebtsRegister.Core.PersonRow", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("Surname");

                    b.ToTable("People");
                });

            modelBuilder.Entity("DebtsRegister.Core.PersonStatisticsRow", b =>
                {
                    b.Property<string>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20);

                    b.Property<decimal>("HistoricalDebtAverageThroughCasesOfTaking");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("PersonId");

                    b.ToTable("CurrentStatisticsPerPerson");
                });

            modelBuilder.Entity("DebtsRegister.Core.PersonTotalsRow", b =>
                {
                    b.Property<string>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20);

                    b.Property<decimal>("DueDebtsTotal");

                    b.Property<int>("HistoricalCountOfCreditsTaken");

                    b.Property<decimal>("HistoricallyCreditedInTotal");

                    b.Property<decimal>("HistoricallyOwedInTotal");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("PersonId");

                    b.HasIndex("DueDebtsTotal");

                    b.ToTable("CurrentTotalsPerPerson");
                });
#pragma warning restore 612, 618
        }
    }
}
