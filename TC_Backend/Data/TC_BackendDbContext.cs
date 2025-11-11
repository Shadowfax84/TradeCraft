using Microsoft.EntityFrameworkCore;
using TC_Backend.Models;

namespace TC_Backend.Data
{
    public class TC_BackendDbContext : DbContext
    {
        public TC_BackendDbContext(DbContextOptions<TC_BackendDbContext> options) : base(options){}

        public DbSet<CompanyList> CompanysList { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<FinancialReport> FinancialReports { get; set; }
        public DbSet<StockRecord> StockRecords { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GameModule> GameModules { get; set; } 
        public DbSet<GameSubModule> GameSubModules { get; set; } 
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

             // Unique constraint on CompanyList.TickerSymbol
            modelBuilder.Entity<CompanyList>()
                .HasIndex(cl => cl.TickerSymbol)
                .IsUnique();

            // FK relation 1 Company in company List has 1 Company Profile based on TickerSymbol
            modelBuilder.Entity<CompanyProfile>()
                .HasOne<CompanyList>()
                .WithOne()
                .HasForeignKey<CompanyProfile>(cp => cp.TickerSymbol)
                .HasPrincipalKey<CompanyList>(cl => cl.TickerSymbol);


            //FK relation 1 Company List with many Financial Report based on TickerSymbol
            modelBuilder.Entity<FinancialReport>()
                .HasOne<CompanyList>()
                .WithMany()
                .HasForeignKey(fr => fr.TickerSymbol)
                .HasPrincipalKey(cl => cl.TickerSymbol);

            //FK relation 1 Company List with many stock Records based on TickerSymbol
            modelBuilder.Entity<StockRecord>()
                .HasOne<CompanyList>()
                .WithMany()
                .HasForeignKey(sr => sr.TickerSymbol)
                .HasPrincipalKey(cl => cl.TickerSymbol);

            //Composite unique key for StockRecord combination of Date and Ticker Symbol
            modelBuilder.Entity<StockRecord>()
                .HasIndex(sr => new { sr.Date, sr.TickerSymbol })
                .IsUnique();

            //RoleName in Role table is indexed and is unique
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            //FK relation 1 role has many user based on RoleID
            modelBuilder.Entity<User>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(u => u.RoleID)
                .HasPrincipalKey(r => r.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            //Validate username is only of small char b/w a-z,0-9 and _
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.NormalizedEmail)
                    .IsUnique();
                entity.HasIndex(u => u.NormalizedUserName)
                    .IsUnique();
            });

            // Configure check constraint using ToTable per EF Core guidance
            modelBuilder.Entity<User>().ToTable(tb => tb.HasCheckConstraint("CK_User_Username_validat", "Username NOT LIKE '%[^a-z0-9_]%'"));


            //FK relation 1 role has many Game Modules based on RoleID
            modelBuilder.Entity<GameModule>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(gm => gm.RoleID)
                .HasPrincipalKey(r => r.RoleId);

            //FK relation 1 GameModule has many Game Sub Modules based on ModuleID
            modelBuilder.Entity<GameSubModule>()
                .HasOne<GameModule>()
                .WithMany()
                .HasForeignKey(sgm => sgm.ModuleID)
                .HasPrincipalKey(gm => gm.ModuleID);

            modelBuilder.Entity<UserProgress>(entity =>
            {
                // UserProgress has 1 User has many UserProgress 
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(up => up.UserID)
                    .HasPrincipalKey(u => u.Id);

                // UserProgress has one GameModule has many UserProgress
                entity.HasOne<GameModule>()
                    .WithMany()
                    .HasForeignKey(up => up.ModuleID)
                    .HasPrincipalKey(m => m.ModuleID);

                // UserProgress has one GameSubModule has many UserProgress
                entity.HasOne<GameSubModule>()
                    .WithMany()
                    .HasForeignKey(up => up.SubmoduleID)
                    .HasPrincipalKey(sm => sm.SubModuleID)
                    .OnDelete(DeleteBehavior.NoAction);

                // Composite Key of UserID,ModuleID and SubmoduleID
                entity.HasIndex(up => new { up.UserID, up.ModuleID, up.SubmoduleID })
                    .IsUnique();
            });
            // UserFile must be unique to avoid duplicate upload
            modelBuilder.Entity<UserFile>()
                .HasIndex(uf => uf.PublicId)
                .IsUnique();
                
            // 1 user can have many userfiles based on userId
            modelBuilder.Entity<UserFile>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(uf => uf.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
