using IIS.Dashboard.Logic;
using Microsoft.EntityFrameworkCore;


namespace IIS.Dashboard.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Guid superUserRoleGuid = Guid.NewGuid();
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Guid);
                entity.Property(e => e.Name).HasMaxLength(250);

                entity.HasData(new Role
                {
                   Guid = superUserRoleGuid,
                    Name ="SupperUser"
                });
                entity.HasData(new Role
                {
                    Guid = Guid.NewGuid(),
                    Name = "WebisteManager"
                });
                entity.HasData(new Role
                {
                    Guid = Guid.NewGuid(),
                    Name = "AccountManager"
                });
                entity.HasData(new Role
                {
                    Guid = Guid.NewGuid(),
                    Name = "NormalUser"
                });
            });
            string password = "11111";
            string user = "SupperUser";
            byte[] saltBytes = AuthLogic.GenerateSalt();
            string hashedPassword = AuthLogic.HashPassword(password, saltBytes);
            string base64Salt = Convert.ToBase64String(saltBytes);
            Guid userGuid=Guid.NewGuid();
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Guid);
                entity.Property(e => e.Name).HasMaxLength(250);

                entity.HasData(new User
                {
                    Guid = userGuid,
                    Name = user,
                    Password = hashedPassword,
                     Email="iis.dashboard@gmail.com",
                      Salt=base64Salt,
                });
            });
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Guid);

                entity.HasData(new UserRole
                {
                    Guid = Guid.NewGuid(),
                    RoleGuid= superUserRoleGuid,
                    UserGuid=userGuid
                });
            });
            modelBuilder.Entity<UserSite>(entity =>
            {
                entity.HasKey(e => e.Guid);

            });
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserSite> UserSites { get; set; }
    }
}
