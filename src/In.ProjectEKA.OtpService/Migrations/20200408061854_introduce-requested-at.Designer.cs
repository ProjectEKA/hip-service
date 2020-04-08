﻿// <auto-generated />
using System;
using In.ProjectEKA.OtpService.Otp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace In.ProjectEKA.OtpService.Migrations
{
    [DbContext(typeof(OtpContext))]
    [Migration("20200408061854_introduce-requested-at")]
    partial class IntroduceRequestedAt
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("In.ProjectEKA.OtpService.Otp.Model.OtpRequest", b =>
                {
                    b.Property<string>("SessionId")
                        .HasColumnType("text");

                    b.Property<string>("OtpToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RequestedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("SessionId");

                    b.ToTable("OtpRequests");
                });
#pragma warning restore 612, 618
        }
    }
}
