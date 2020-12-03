using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.EF.Context;
using DCGServiceDesk.EF.Factory;
using DCGServiceDesk.EF.Services;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.CurrentUser;
using DCGServiceDesk.Session.Navigation;
using DCGServiceDesk.ViewModels;
using DCGServiceDesk.ViewModels.Factory;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
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
                //DbContextFactory
                services.AddSingleton<DCGServiceDeskContextFactory>(s =>
                new DCGServiceDeskContextFactory(context.Configuration));
                services.AddSingleton<IDatabaseContextFactory, DCGServiceDeskContextFactory>();


                //ViewModels Factory
                //Not at this moment

                services.AddSingleton<IServiceDeskViewModelFactory, ServiceDeskViewModelFactory>();
                services.AddSingleton<IViewForwarding, ViewForwarding>();
                services.AddSingleton<ICrud<User>, IdentityDataServices>();
                services.AddSingleton<IUserService, IdentityDataServices>();
                services.AddSingleton<IAuthorization, Authorization>();
                services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
                services.AddSingleton<ILoggedUser, LoggedUser>();

                services.AddSingleton<CreateViewModel<LoginViewModel>>(service =>
                {
                    return () => new LoginViewModel(
                        service.GetRequiredService<IAuthorization>());
                });

                //MainWindow initializer
                services.AddScoped<MainWindowViewModel>();
                services.AddScoped<MainWindow>(s => new MainWindow(s.GetRequiredService<MainWindowViewModel>()));

            });

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            Window window = _host.Services.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

    }
}
