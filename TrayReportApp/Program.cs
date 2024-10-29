using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using TrayReportApp.Components;
using TrayReportApp.Data;
using TrayReportApp.Data.Repositories;
using TrayReportApp.Services;
using TrayReportApp.Services.Contracts;

namespace TrayReportApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DB context to the container
            builder.Services.AddDbContext<TrayReportAppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    options => options.UseAdminDatabase("postgres")));      

            builder.Services.AddQuickGridEntityFrameworkAdapter();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Add MudBlazor services
            builder.Services.AddMudServices();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Add dependencies to the container
            builder.Services.AddScoped<ITrayReportAppDbRepository, TrayReportAppDbRepository>();
            builder.Services.AddScoped<ISupportService, SupportService>();
            builder.Services.AddScoped<ITrayService, TrayService>();
            builder.Services.AddScoped<ICableTypeService, CableTypeService>();
            builder.Services.AddScoped<ICableService, CableService>();
            builder.Services.AddScoped<ITrayTypeService, TrayTypeService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
    app.UseMigrationsEndPoint();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
