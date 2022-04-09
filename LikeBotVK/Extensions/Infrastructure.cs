using Hangfire;
using Hangfire.SqlServer;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Telegram.Bot;

namespace LikeBotVK.Extensions;

public static class Infrastructure
{
    public static void AddInfrastructureDependencies(this IServiceCollection services,
        Configuration.Configuration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.DatabaseConfiguration.BaseConnection + configuration.DatabaseConfiguration.DomainDb,
                optionsBuilder => { optionsBuilder.EnableRetryOnFailure(1); });
        });

        services.AddDbContext<LikeBotVK.Infrastructure.ApplicationData.Context.ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.DatabaseConfiguration.BaseConnection + configuration.DatabaseConfiguration.ApplicationDb,
                optionsBuilder => { optionsBuilder.EnableRetryOnFailure(1); });
        });

        services.AddHangfire((_, globalConfiguration) =>
        {
            globalConfiguration.UseSqlServerStorage(
                configuration.DatabaseConfiguration.BaseConnection + configuration.DatabaseConfiguration.HangfireDb,
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    PrepareSchemaIfNecessary = true
                });

            globalConfiguration.UseSerializerSettings(new JsonSerializerSettings
                {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            RecurringJob.AddOrUpdate<ISubscribeDeleter>("subscribesChecker", x => x.DeleteAsync(), Cron.Daily);
            RecurringJob.AddOrUpdate<IWorkDeleter>("worksChecker", x => x.DeleteAsync(), Cron.Daily);
        });
        services.AddHangfireServer(options => options.WorkerCount = Environment.ProcessorCount * 15);

        services.AddHttpClient("tgwebhook").AddTypedClient<ITelegramBotClient>(httpClient
            => new TelegramBotClient(configuration.BotConfiguration.TelegramToken, httpClient));
    }
}