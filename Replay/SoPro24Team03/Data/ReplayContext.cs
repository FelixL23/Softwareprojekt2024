using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data
{
    /// <summary>
    /// Context for accessing the database.
    /// </summary>
    public class ReplayContext : DbContext
    {
        /// <value>path in the filesystem to store the database</value>
        public string DbPath { get; }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<TaskInst> TaskInsts { get; set; }
        public DbSet<Note> TaskInstNotes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ProcedureTemplate> ProcedureTemplates { get; set; }
        public DbSet<TaskTemplate> TaskTemplates { get; set; }
        public DbSet<TaskResponsible> TaskResponsibles { get; set; }

        public ReplayContext()
            : base()
        {
            DbPath = System.IO.Path.GetFullPath("./persistence/database.sqlite");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}").EnableSensitiveDataLogging(true);
            options.EnableSensitiveDataLogging(true);
            options.EnableDetailedErrors(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcedureTemplate>()
                .HasMany(pt => pt.TaskTemplates)
                .WithMany(tt => tt.ProcedureTemplates)
                .UsingEntity<Dictionary<string, object>>(
                    "ProcedureTemplateTaskTemplate",
                    j => j.HasOne<TaskTemplate>().WithMany().HasForeignKey("TaskTemplateId"),
                    j => j.HasOne<ProcedureTemplate>().WithMany().HasForeignKey("ProcedureTemplateId")
                );

            modelBuilder.Entity<ProcedureTemplate>()
                .HasMany(pt => pt.Roles)
                .WithMany(r => r.ProcedureTemplates)
                .UsingEntity<Dictionary<string, object>>(
                    "ProcedureTemplateRole",
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    j => j.HasOne<ProcedureTemplate>().WithMany().HasForeignKey("ProcedureTemplateId")
                );

            modelBuilder.Entity<TaskResponsible>()
                .HasOne(tr => tr.Role)
                .WithMany(r => r.TaskResponsibles)
                .HasForeignKey(tr => tr.RoleId)
                .IsRequired(false);

            modelBuilder
                .Entity<TaskTemplate>()
                .Property(e => e.ContractTypes)
                .HasConversion(
                    v => v != null ? string.Join(",", v.Select(e => e.ToString()).ToArray()) : "",
                    v => v != "" ? v.Split(new[] { ',' })
                        .Select(e => Enum.Parse(typeof(ContractType), e))
                        .Cast<ContractType>()
                        .ToList() ?? new List<ContractType>()
                        : new List<ContractType>()
                );
        }
    }
}