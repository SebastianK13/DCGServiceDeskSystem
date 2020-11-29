using DCGServiceDesk.Data.Models;
using DCGServiceDesk.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace DCGServiceDesk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly IHost _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder(null)
            .ConfigureAppConfiguration(c =>
            {
                c.AddJsonFile("appsettings.json");
                c.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseSqlServer(context.Configuration["Data:DCTEServiceDesk:ConnectionString"]));

                services.AddDbContext<AppServiceDeskDbContext>(options =>
                    options.UseSqlServer(context.Configuration["Data:DCTEServiceDesk:ConnectionString"]));

                services.AddDbContext<AppCustomersDbContext>(options =>
                    options.UseSqlServer(context.Configuration["Data:DCTECustomers:ConnectionString"]));
            });

    }
}
