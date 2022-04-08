using LikeBotVK.Infrastructure.PersistentStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.PersistentStorage.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserModel> Users { get; set; } = null!;
    public DbSet<VkModel> Vks { get; set; } = null!;
    public DbSet<ProxyModel> Proxies { get; set; } = null!;
    public DbSet<PublicationModel> Publications { get; set; } = null!;
    public DbSet<JobModel> Jobs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Long relationship from the user
        modelBuilder.Entity<UserModel>()
            .HasMany(c => c.Vks).WithOne(c => c.User).HasForeignKey(vk => vk.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        //Vk and jobs
        modelBuilder.Entity<VkModel>().HasMany(c => c.Jobs).WithOne(j => j.Vk).HasForeignKey(j => j.VkId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VkModel>().HasOne(c => c.Proxy).WithMany(proxy => proxy.Vks)
            .HasForeignKey(c => c.ProxyId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<JobModel>().HasMany(j => j.Publications).WithOne(p => p.JobModel)
            .OnDelete(DeleteBehavior.Cascade);
    }
}