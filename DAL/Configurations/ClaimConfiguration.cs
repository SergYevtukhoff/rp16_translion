﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace DAL.Configurations
{
    public class ClaimConfiguration : EntityTypeConfiguration<Claim>
    {
        public ClaimConfiguration()
        {
            ToTable("Claim");

            HasKey(x => x.ClaimId)
                .Property(x => x.ClaimId)
                .HasColumnName("ClaimId")
                .HasColumnType("int")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(x => x.UserId)
                .HasColumnName("UserId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(x => x.ClaimType)
                .HasColumnName("ClaimType")
                .HasColumnType("nvarchar")
                .IsMaxLength()
                .IsOptional();

            Property(x => x.ClaimValue)
                .HasColumnName("ClaimValue")
                .HasColumnType("nvarchar")
                .IsMaxLength()
                .IsOptional();

            HasRequired(x => x.User)
                .WithMany(x => x.Claims)
                .HasForeignKey(x => x.UserId);
        }
    }
}