using LikeBotVK.Infrastructure.ApplicationData.Models;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.ApplicationData.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserData> UsersData { get; set; } = null!;
    public DbSet<JobData> JobsData { get; set; } = null!;
}