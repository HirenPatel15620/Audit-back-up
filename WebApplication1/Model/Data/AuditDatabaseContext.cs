using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Model.Models;
using Repository;
using Model.Data;
using Z.BulkOperations;
using AuditEntry = Model.Models.AuditEntry;

namespace Model.Data;

public partial class AuditDatabaseContext : DbContext
{
    AuthenticationStateProvider authenticationStateProvider;

    public AuditDatabaseContext()
    {
    }

    public AuditDatabaseContext(DbContextOptions<AuditDatabaseContext> options, AuthenticationStateProvider authenticationStateProvider)
        : base(options)
    {
        this.authenticationStateProvider = authenticationStateProvider;
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<ChangeAudit> ChangeAudits { get; set; }

    public virtual DbSet<ChangeAuditAddress> ChangeAuditAddresses { get; set; }

    public virtual DbSet<User> Users { get; set; }


    public override int SaveChanges()
    {
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            string auditTableName;

            if (entry.State == EntityState.Added)
            {
                auditTableName = GetAuditTableName(entry.Entity);
                auditEntries.Add(CreateAuditEntry(entry, "Insert"));
            }
            else if (entry.State == EntityState.Modified)
            {
                auditTableName = GetAuditTableName(entry.Entity);
                auditEntries.AddRange(CreateAuditEntriesForUpdate(entry));
            }
            else if (entry.State == EntityState.Deleted)
            {
                auditTableName = GetAuditTableName(entry.Entity);
                auditEntries.Add(CreateAuditEntry(entry, "Delete"));
            }
            else
            {
                continue;
            }

            SaveAuditEntriesToTable(auditTableName, auditEntries);
        }



        return base.SaveChanges();
    }
    private string GetAuditTableName(object entity)
    {

        if (entity is User)
        {
            return "AuditUser";
        }
        else if (entity is Address)
        {
            return "AuditAddress";
        }
        else
        {
            throw new NotSupportedException($"Audit for entity type {entity.GetType().Name} is not supported.");
        }
    }

    private Dictionary<string, Type> AuditEntryMap = new Dictionary<string, Type>
    {

    { "AuditUser", typeof(ChangeAudit) },
    { "AuditAddress", typeof(ChangeAuditAddress) },

    };



    private void SaveAuditEntriesToTable(string tableName, IEnumerable<AuditEntry> entries)
    {
        using (var auditContext = new AuditDatabaseContext())
        {
            if (AuditEntryMap.TryGetValue(tableName, out Type auditEntryType))
            {
                // Use reflection to dynamically invoke the Set<TEntity> method
                var setMethodInfo = typeof(DbContext).GetMethods()
                    .Where(method => method.Name == "Set" && method.IsGenericMethodDefinition)
                    .Single(method => method.GetParameters().Length == 0)
                    .MakeGenericMethod(auditEntryType);

                var dbSet = setMethodInfo.Invoke(auditContext, null);
                dbSet.GetType().GetMethod("AddRange").Invoke(dbSet, new object[] { entries });
                auditContext.SaveChanges();
            }
            else
            {
                throw new NotSupportedException($"Saving audit entries to table {tableName} is not supported.");
            }
        }
    }



    private AuditEntry CreateAuditEntry(EntityEntry entry, string action)
    {
        var auditEntry = new AuditEntry
        {
            EntityName = entry.Entity.GetType().Name,
            Action = "Created",
            Timestamp = DateTime.UtcNow,
            EntityIdentifier = "hii"
        };


        return auditEntry;
    }
    private IEnumerable<AuditEntry> CreateAuditEntriesForUpdate(EntityEntry entry)
    {
        var auditEntries = new List<AuditEntry>();


        foreach (var property in entry.OriginalValues.Properties)
        {
            var originalValue = entry.OriginalValues[property];
            var currentValue = entry.CurrentValues[property];

            if (!object.Equals(originalValue, currentValue))
            {
                var auditEntry = new AuditEntry
                {
                    EntityName = entry.Entity.GetType().Name,
                    Action = "Update",
                    PropertyName = property.Name,
                    OldValue = originalValue?.ToString(),
                    NewValue = currentValue?.ToString(),
                    Timestamp = DateTime.UtcNow,
                    UserName = "Hiren",
                    EntityIdentifier = "hii"


                };
                auditEntries.Add(auditEntry);
            }
        }

        return auditEntries;
    }


    //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    //{
    //    await AuditChanges();
    //    return await base.SaveChangesAsync(cancellationToken);
    //}

    //private async Task AuditChanges()
    //{
    //    DateTime now = DateTime.Now;

    //    var entityEntries = ChangeTracker.Entries()
    //        .Where(x => x.State == EntityState.Added ||
    //                    x.State == EntityState.Modified ||
    //                    x.State == EntityState.Deleted).ToList();

    //    foreach (EntityEntry entityEntry in entityEntries)
    //    {
    //        IncrementVersionNumber(entityEntry);

    //        if (entityEntry.Entity is IAuditable)
    //        {
    //            var auditTable = GetAuditTableForEntity(entityEntry.Entity);
    //            await CreateAuditAsync(entityEntry, now, auditTable);
    //        }
    //    }
    //}

    //private void IncrementVersionNumber(EntityEntry entityEntry)
    //{
    //    if (entityEntry.Metadata.FindProperty("Version") != null)
    //    {
    //        var currentVersionNumber = Convert.ToInt32(entityEntry.CurrentValues["Version"]);
    //        entityEntry.CurrentValues["Version"] = currentVersionNumber + 1;
    //    }
    //}

    //private async Task CreateAuditAsync(EntityEntry entityEntry, DateTime timeStamp, DbSet<ChangeAuditBase> auditTable)
    //{
    //    if (entityEntry.State == EntityState.Added)
    //    {
    //        foreach (var prop in entityEntry.OriginalValues.Properties)
    //        {
    //            var originalValue = !string.IsNullOrWhiteSpace(entityEntry.OriginalValues[prop]?.ToString()) ?
    //                entityEntry.OriginalValues[prop]?.ToString() : null;

    //            var currentValue = !string.IsNullOrWhiteSpace(entityEntry.CurrentValues[prop]?.ToString()) ?
    //                entityEntry.CurrentValues[prop]?.ToString() : null;

    //            var changeAudit = await GetChangeAuditAsync(entityEntry, timeStamp);
    //            changeAudit.PropertyName = prop.Name;
    //            changeAudit.OldValue = string.Empty;
    //            changeAudit.NewValue = currentValue;
    //            if (auditTable != null)
    //            {
    //                await auditTable.AddAsync(changeAudit);
    //            }
    //        }
    //    }
    //    else if (entityEntry.State == EntityState.Modified)
    //    {
    //        foreach (var prop in entityEntry.OriginalValues.Properties)
    //        {
    //            var originalValue = !string.IsNullOrWhiteSpace(entityEntry.OriginalValues[prop]?.ToString()) ?
    //                entityEntry.OriginalValues[prop]?.ToString() : null;

    //            var currentValue = !string.IsNullOrWhiteSpace(entityEntry.CurrentValues[prop]?.ToString()) ?
    //                entityEntry.CurrentValues[prop]?.ToString() : null;

    //            if (originalValue != currentValue)
    //            {
    //                var changeAudit = await GetChangeAuditAsync(entityEntry, timeStamp);
    //                changeAudit.PropertyName = prop.Name;
    //                changeAudit.OldValue = originalValue;
    //                changeAudit.NewValue = currentValue;
    //                if (auditTable != null)
    //                {
    //                    await auditTable.AddAsync(changeAudit);
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        var changeAudit = await GetChangeAuditAsync(entityEntry, timeStamp);
    //        if (auditTable != null)
    //        {
    //            await auditTable.AddAsync(changeAudit);
    //        }

    //    }

    //}

    //private async Task<ChangeAudit> GetChangeAuditAsync(EntityEntry entityEntry, DateTime timeStamp)
    //{
    //    return new ChangeAudit
    //    {
    //        EntityName = entityEntry.Entity.GetType().Name,
    //        Action = entityEntry.State.ToString(),
    //        EntityIdentifier = GetEntityIdentifier(entityEntry),
    //        UserName = "Test 123",
    //        TimeStamp = timeStamp
    //    };
    //}

    //private static string GetEntityIdentifier(EntityEntry entityEntry)
    //{
    //    if (entityEntry.Entity is IdentityUser)
    //        return $"{entityEntry.CurrentValues["UserName"]}";
    //    else if (entityEntry.Entity is User)
    //        return $"{entityEntry.CurrentValues["Email"]} {entityEntry.CurrentValues["Contact"]}"; 
    //    else if (entityEntry.Entity is Address)
    //        return $"{entityEntry.CurrentValues["Id"]} {entityEntry.CurrentValues["Pincode"]}";


    //    return "None";
    //}

    //private async Task<string?> GetUserNameAsync() 
    //{
    //    var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();

    //    if (authenticationState.User.Identity.IsAuthenticated)
    //        return authenticationState.User.Identity.Name;

    //    return null;
    //}
    //private DbSet<ChangeAuditBase> GetAuditTableForEntity(object entity)
    //{
    //    if (entity is User)
    //    {
    //        return ChangeAudits;
    //    }
    //    else if (entity is Address)
    //    {
    //        return ChangeAuditAddresses;
    //    }

    //    //!if any tbale is not available for audit
    //    else
    //    {
    //        return ChangeAudits;
    //    }
    //}




    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=PCA189\\SQL2019;DataBase=AuditDatabase;User ID=sa;Password=Tatva@123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("address");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address1).HasColumnName("address");
            entity.Property(e => e.Pincode)
                .HasMaxLength(50)
                .HasColumnName("pincode");
        });

        modelBuilder.Entity<ChangeAudit>(entity =>
        {
            entity.ToTable("change_audit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action).HasColumnName("action");
            entity.Property(e => e.EntityIdentifier)
                .HasMaxLength(50)
                .HasColumnName("entity_identifier");
            entity.Property(e => e.EntityName).HasColumnName("entity_name");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.PropertyName)
                .HasMaxLength(50)
                .HasColumnName("property_name");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasColumnName("user_name");
        });

        modelBuilder.Entity<ChangeAuditAddress>(entity =>
        {
            entity.ToTable("change_audit_address");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action).HasColumnName("action");
            entity.Property(e => e.EntityIdentifier)
                .HasMaxLength(50)
                .HasColumnName("entity_identifier");
            entity.Property(e => e.EntityName).HasColumnName("entity_name");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.PropertyName)
                .HasMaxLength(50)
                .HasColumnName("property_name");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserName).HasColumnName("user_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contact)
                .HasMaxLength(50)
                .HasColumnName("contact");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
