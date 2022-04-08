using LikeBotVK.Infrastructure.ApplicationData.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LikeBotVK.Infrastructure.ApplicationData.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserData> UsersData { get; set; } = null!;
    public DbSet<JobData> JobsData { get; set; } = null!;
    public DbSet<SubscribeData> SubscribesData { get; set; } = null!;
    public DbSet<PaymentData> PaymentsData { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserData>().HasMany(u => u.Subscribes).WithOne(s => s.UserData);
        modelBuilder.Entity<UserData>().Property(u => u.CurrentJobsId).HasConversion(
            ints => JsonConvert.SerializeObject(ints), str => JsonConvert.DeserializeObject<List<int>>(str));
    }
}