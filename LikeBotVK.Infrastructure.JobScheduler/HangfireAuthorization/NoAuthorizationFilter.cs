using Hangfire.Dashboard;

namespace LikeBotVK.Infrastructure.JobScheduler.HangfireAuthorization;

public class NoAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}