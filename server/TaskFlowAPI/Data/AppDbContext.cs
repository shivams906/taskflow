using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Models;

namespace TaskFlowAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectUser> ProjectUsers => Set<ProjectUser>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TaskTimeLog> TaskTimeLogs => Set<TaskTimeLog>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Workspace> Workspaces => Set<Workspace>();
        public DbSet<WorkspaceUser> WorkspaceUsers => Set<WorkspaceUser>();
        public DbSet<ChangeLog> ChangeLogs => Set<ChangeLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Workspace>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Workspace>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<WorkspaceUser>()
                .HasOne(wu => wu.Workspace)
                .WithMany(w => w.WorkspaceUsers)
                .HasForeignKey(wu => wu.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkspaceUser>()
                .HasOne(wu => wu.User)
                .WithMany(u => u.WorkspaceUsers)
                .HasForeignKey(wu => wu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkspaceUser>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<WorkspaceUser>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<WorkspaceUser>()
                .Property(wu => wu.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Workspace)
                .WithMany(w => w.Projects)
                .HasForeignKey(p => p.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.Project)
                .WithMany(p => p.ProjectUsers)
                .HasForeignKey(pu => pu.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.User)
                .WithMany(u => u.ProjectUsers)
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<ProjectUser>()
                .Property(pu => pu.Role)
                .HasConversion<string>();

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<TaskItem>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.AuditType)
                .HasConversion<string>();

            modelBuilder.Entity<TaskTimeLog>()
                .HasOne(tt => tt.User)
                .WithMany()
                .HasForeignKey(tt => tt.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<TaskTimeLog>()
                .HasOne(tt => tt.TaskItem)
                .WithMany(t => t.TimeLogs)
                .HasForeignKey(tt => tt.TaskItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<TaskTimeLog>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<TaskTimeLog>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<ChangeLog>()
                .HasOne(c => c.ChangedByUser)
                .WithMany()
                .HasForeignKey(c => c.ChangedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
