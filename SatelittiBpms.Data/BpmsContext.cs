using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Data.Configuration;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data
{
    public class BpmsContext : DbContext
    {
        public BpmsContext(DbContextOptions<BpmsContext> options) : base(options) { }
        public DbSet<ActivityInfo> Activities { get; set; }
        public DbSet<ActivityFieldInfo> ActivityFields { get; set; }
        public DbSet<FieldInfo> Fields { get; set; }
        public DbSet<FieldValueInfo> FieldValues { get; set; }
        public DbSet<FieldValueFileInfo> FieldValueFiles { get; set; }
        public DbSet<FlowInfo> Flows { get; set; }
        public DbSet<FlowPathInfo> FlowPaths { get; set; }
        public DbSet<ProcessInfo> Processes { get; set; }
        public DbSet<ProcessVersionRoleInfo> ProcessRoles { get; set; }
        public DbSet<ProcessVersionInfo> ProcessVersions { get; set; }
        public DbSet<RoleInfo> Roles { get; set; }
        public DbSet<RoleUserInfo> RoleUsers { get; set; }
        public DbSet<TaskInfo> Tasks { get; set; }
        public DbSet<TaskHistoryInfo> TaskHistories { get; set; }
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<ActivityNotificationInfo> ActivityNotifications { get; set; }
        public DbSet<ActivityUserInfo> ActivityUsers { get; set; }
        public DbSet<ActivityUserOptionInfo> ActivityUsersOptions { get; set; }
        public DbSet<TenantInfo> Tenants { get; set; }
        public DbSet<VersionNormalizationInfo> VersionNormalization { get; set; }
        public DbSet<TemplateInfo> Templates { get; set; }
        public DbSet<SignerIntegrationActivityAuthorizerInfo> SignerIntegrationActivityAuthorizers { get; set; }
        public DbSet<SignerIntegrationActivityInfo> SignerIntegrationActivities { get; set; }
        public DbSet<SignerIntegrationActivityFileInfo> SignerIntegrationActivityFiles { get; set; }
        public DbSet<SignerIntegrationActivitySignatoryInfo> SignerIntegrationActivitySignatories { get; set; }
        public DbSet<TaskSignerInfo> TaskSigner { get; set; }
        public DbSet<TaskSignerFileInfo> TaskSignerFiles { get; set; }
        public DbSet<NotificationInfo> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ActivityEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityFieldEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FieldEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FieldValueEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FlowEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FlowPathEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessRoleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessVersionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RoleUserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskHistoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityUserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityUserOptionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityNotificationEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TenantEntityConfiguration());
            modelBuilder.ApplyConfiguration(new VersionNormalizationEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FieldValueFileEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SignerIntegrationActivityEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SignerIntegrationActivityAuthorizerEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SignerIntegrationActivitySignatoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SignerIntegrationActivityFileEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskSignerInfoConfiguration());
            modelBuilder.ApplyConfiguration(new TaskSignerFileInfoConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationEntityConfiguration());
        }
    }
}
