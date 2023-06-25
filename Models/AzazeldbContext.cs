using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace MatrixApp.Models;

public partial class AzazeldbContext : DbContext
{
    public AzazeldbContext()
    {
        OnConfiguring(new DbContextOptionsBuilder());
    }

    public AzazeldbContext(DbContextOptions<AzazeldbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CountsConsolidateTxt> CountsConsolidateTxts { get; set; }

    public virtual DbSet<CountsMatrixInitial> CountsMatrixInitials { get; set; }

    public virtual DbSet<CountsMatrixResult> CountsMatrixResults { get; set; }

    public virtual DbSet<ExperimentTable> ExperimentTable { get; set; }

    public virtual DbSet<Logging> Loggings { get; set; }

    public DbContextOptionsBuilder optionsBuilder { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
     
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CountsConsolidateTxt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CountsCo__3214EC07E3FCA340");

            entity.ToTable("CountsConsolidateTxt", "MatrixAnalyzer");

            entity.Property(e => e.ConsolidatedColumns)
                .HasMaxLength(4000)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
              .HasMaxLength(60)
              .IsUnicode(false);

            entity.Property(e => e.ExperimentName).HasColumnName("ExperimentName");


            entity.Property(e => e.InsertDate)
            .HasDefaultValueSql()
            .IsUnicode(false);
        });

        modelBuilder.Entity<CountsMatrixInitial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CountsMa__3214EC074795399C");

            entity.ToTable("CountsMatrixInitial", "MatrixAnalyzer");

            entity.HasIndex(e => e.InsertDate, "IX_DATEINDEX2");

            entity.Property(e => e.Gene)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.KeyValueIntegers).IsUnicode(false);
            entity.Property(e => e.EnsembleId)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.SearchableKey)
                .HasMaxLength(181)
                .IsUnicode(false)
                .HasComputedColumnSql("(concat([EnsembleId],'_',[Gene]))", false);

            entity.HasOne(d => d.Matrix).WithMany(p => p.CountsMatrixInitials)
                .HasForeignKey(d => d.MatrixId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CountsMat__Matri__2E5BD364");
        });
       
        modelBuilder.Entity<ExperimentTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Experime__3214EC07DE2048D1")
                .HasName("IX_ExperimentTable_ExperimentName_SearchableKey");

            entity.ToTable("ExperimentTable", "MatrixAnalyzer");

            entity.Property(e => e.ExperimentName).HasColumnName("ExperimentName");
            entity.Property(e => e.ExpMatrixIds)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EnsembleId).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Gene).HasMaxLength(50).HasColumnName("Gene");

            entity.Property(e => e.SearchableKey)
                .HasMaxLength(181)
                .IsUnicode(false)
                .HasComputedColumnSql("(concat([EnsembleId],'_',[Gene]))", false);
            entity.Property(e => e.SampleName).HasMaxLength(25).IsUnicode(false);

            entity.Property(e => e.AveragedValue).HasColumnName("AveragedValue");

            entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");

            entity.HasIndex(e => new { e.ExperimentName, e.SearchableKey }, "IX_ExperimentTable_ExperimentName_SearchableKey");

            entity.HasIndex(e => e.SearchableKey,"IX_ExperimentTable_SearchableKey");

            entity.HasIndex(e => new { e.ExperimentName, e.ExpMatrixIds },"IX_ExperimentTable_ExperimentName_ExpMatrixIds");

            entity.HasIndex(e => e.InsertDate,"IX_ExperimentTable_InsertDate");

        });
        modelBuilder.Entity<CountsMatrixResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CountsMa__3214EC0757E3ACC3");

            entity.ToTable("CountsMatrixResults", "MatrixAnalyzer");

            entity.HasIndex(e => e.InsertDate, "IX_DATEINDEX1");

            entity.HasIndex(e => e.MatrixId, "IX_MatrixId");

            entity.Property(e => e.Fdr).HasColumnName("FDR");
            entity.Property(e => e.Gene)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.KeyValueFloats).IsUnicode(false);
            entity.Property(e => e.LogCpm).HasColumnName("LogCPM");
            entity.Property(e => e.LogFc).HasColumnName("LogFC");
            entity.Property(e => e.Pvalue).HasColumnName("PValue");
            entity.Property(e => e.EnsembleId)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.SearchableKey)
                .HasMaxLength(181)
                .IsUnicode(false)
                .HasComputedColumnSql("(concat([EnsembleId],'_',[Gene]))", false);

            entity.HasOne(d => d.Matrix).WithMany(p => p.CountsMatrixResults)
                .HasForeignKey(d => d.MatrixId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CountsMat__Matri__33208881");
        });


        modelBuilder.Entity<Logging>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logging__3214EC07AAEBB852");

            entity.ToTable("Logging", "MatrixAnalyzer");

            entity.Property(e => e.EventType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Log)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

      

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
