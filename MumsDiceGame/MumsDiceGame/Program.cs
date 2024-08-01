using LoggingHelpers;
using MumsDiceGame.Client.Pages;
using MumsDiceGame.Components;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

Log.Logger = new LoggerConfiguration()
    .ConfigureBasic()
    .MinimumLevel.Debug()//.ConfigureMinLoggingLevel()
    .ConfigureWriteToDefaultFile()
    .ConfigureWriteToConsole()
    .WriteTo.Debug()
    .CreateLogger();
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, dispose: true));
Log.Logger.Debug("Serilog added (in addition to)");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MumsDiceGame.Client._Imports).Assembly);

app.Run();
