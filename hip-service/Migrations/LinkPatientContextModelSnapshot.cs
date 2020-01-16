﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using hip_service.Link.Patient.Models;

namespace hip_service.Migrations
{
    [DbContext(typeof(LinkPatientContext))]
    partial class LinkPatientContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("hip_service.Link.Patient.Models.LinkRequest", b =>
                {
                    b.Property<string>("LinkReferenceNumber")
                        .HasColumnType("text");

                    b.Property<string>("ConsentManagerId")
                        .HasColumnType("text");

                    b.Property<string>("ConsentManagerUserId")
                        .HasColumnType("text");

                    b.Property<string>("DateTimeStamp")
                        .HasColumnType("text");

                    b.Property<string>("PatientReferenceNumber")
                        .HasColumnType("text");

                    b.HasKey("LinkReferenceNumber");

                    b.ToTable("LinkRequest");
                });

            modelBuilder.Entity("hip_service.Link.Patient.Models.LinkedCareContext", b =>
                {
                    b.Property<string>("CareContextNumber")
                        .HasColumnType("text");

                    b.Property<string>("LinkReferenceNumber")
                        .HasColumnType("text");

                    b.HasKey("CareContextNumber", "LinkReferenceNumber")
                        .HasName("Id");

                    b.HasIndex("LinkReferenceNumber");

                    b.ToTable("LinkedCareContext");
                });

            modelBuilder.Entity("hip_service.OTP.Models.OtpRequest", b =>
                {
                    b.Property<string>("SessionId")
                        .HasColumnType("text");

                    b.Property<string>("DateTimeStamp")
                        .HasColumnType("text");

                    b.Property<string>("OtpToken")
                        .HasColumnType("text");

                    b.HasKey("SessionId");

                    b.ToTable("OtpRequests");
                });

            modelBuilder.Entity("hip_service.Link.Patient.Models.LinkedCareContext", b =>
                {
                    b.HasOne("hip_service.Link.Patient.Models.LinkRequest", "LinkRequest")
                        .WithMany("CareContexts")
                        .HasForeignKey("LinkReferenceNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
