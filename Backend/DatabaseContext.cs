using Backend.Entitys;
using Microsoft.EntityFrameworkCore;

namespace Backend; 

public class DatabaseContext : DbContext {
    private string _connectionString;
    
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    public DatabaseContext(IConfiguration configuration) {
        _connectionString = configuration.GetSection("MySQL").Get<string>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (string.IsNullOrEmpty(_connectionString))
            throw new ArgumentException("MySQL Connection String was not defined correctly in the Configuration!");
        
        optionsBuilder.UseMySQL(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entry => {
            entry.HasKey(e => e.Id);
            entry.Property(e => e.FirstName);
            entry.Property(e => e.LastName);
            entry.Property(e => e.Email);
            entry.Property(e => e.Username);
            entry.Property(e => e.Password);
            entry.Property(e => e.Created);
        });

        modelBuilder.Entity<RefreshToken>(entry => {
            entry.HasKey(e => e.Id);
            entry.Property(e => e.UserId);
            entry.Property(e => e.ExpirationDate);
        });

        modelBuilder.Entity<AccessToken>(entry => {
            entry.HasKey(e => e.Id);
            entry.Property(e => e.RefreshTokenId);
            entry.Property(e => e.ExpirationDate);
        });

        modelBuilder.Entity<Permission>(entry => {
            entry.HasKey(e => e.Id);
            entry.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entry.Property(e => e.UserId);
            entry.Property(e => e.PermissionKey);
        });
    }

    public void ExecuteTableCreation() {
        Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS Users (Id VARCHAR(50) PRIMARY KEY, FirstName VARCHAR(255), LastName VARCHAR(255), Email VARCHAR(255), Username VARCHAR(255), Password VARCHAR(255), Created TIMESTAMP)");
        Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS RefreshTokens (Id VARCHAR(50) PRIMARY KEY, UserId VARCHAR(50), ExpirationDate TIMESTAMP)");
        Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS AccessTokens (Id VARCHAR(50) PRIMARY KEY, RefreshTokenId VARCHAR(50), ExpirationDate TIMESTAMP)");
        Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS Permissions (Id INT PRIMARY KEY AUTO_INCREMENT, UserId VARCHAR(50), PermissionName VARCHAR(100))");
    }
}