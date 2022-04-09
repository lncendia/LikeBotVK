using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Hangfire;
using LikeBotVK.Configuration;
using LikeBotVK.Extensions;
using LikeBotVK.Infrastructure.JobScheduler.HangfireAuthorization;
using LikeBotVK.Infrastructure.Web.Controllers;

var builder = WebApplication.CreateBuilder(args);

var culture = CultureInfo.GetCultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
CultureInfo.CurrentCulture = culture;

var configuration = builder.Configuration.Get<Configuration>();

var validationBase = new ValidationContext(configuration, null, null);
var validationBot = new ValidationContext(configuration.BotConfiguration, null, null);
var validationDatabase = new ValidationContext(configuration.DatabaseConfiguration, null, null);
var validationPayment = new ValidationContext(configuration.PaymentConfiguration, null, null);
Validator.ValidateObject(configuration, validationBase, true);
Validator.ValidateObject(configuration.BotConfiguration, validationBot, true);
Validator.ValidateObject(configuration.DatabaseConfiguration, validationDatabase, true);
Validator.ValidateObject(configuration.PaymentConfiguration, validationPayment, true);

foreach (var project in configuration.BotConfiguration.Projects)
{
    var validation = new ValidationContext(project, null, null);
    Validator.ValidateObject(project, validation, true);
}

builder.Services.AddUnitOfWorks(configuration);
builder.Services.AddInfrastructureDependencies(configuration);
builder.Services.AddDomainServices(configuration);
builder.Services.AddApplicationServices(configuration);

builder.Services.AddMvc().AddNewtonsoftJson().AddApplicationPart(typeof(BotController).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseHangfireDashboard($"/{configuration.BotConfiguration.TelegramToken}/hangfire",
    new DashboardOptions
    {
        AppPath = $"/{configuration.BotConfiguration.TelegramToken}",
        Authorization = new[] {new NoAuthorizationFilter()},
    });
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("tgwebhook",
        configuration.BotConfiguration.TelegramToken,
        new {controller = "Bot", action = "Post"});
    endpoints.MapControllerRoute(
        "default",
        configuration.BotConfiguration.TelegramToken + "/{controller=Proxy}/{action=Index}/{id?}");
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});
app.Run();