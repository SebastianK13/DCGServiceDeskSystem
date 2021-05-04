using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.EF.Context;
using DCGServiceDesk.EF.Factory;
using DCGServiceDesk.EF.Services;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.CurrentUser;
using DCGServiceDesk.Session.DataGetter;
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

                services.AddSingleton<IServiceDeskViewModelFactory, ServiceDeskViewModelFactory>();
                services.AddSingleton<IViewForwarding, ViewForwarding>();
                services.AddSingleton<IUserService, IdentityDataServices>();
                services.AddSingleton<IAuthorization, Authorization>();
                services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
                services.AddSingleton<ILoggedUser, LoggedUser>();
                services.AddScoped<IRequestService, RequestsDataService>();
                services.AddSingleton<IRequestQueue, RequestQueue>();
                services.AddSingleton<IUserInfo, UserInfo>();
                services.AddSingleton<IEmployeeProfile, EmployeeProfile>();
                services.AddSingleton<IEmployeeService, EmployeeDataService>();
                //services.AddDbContext<AppServiceDeskDbContext>(options => options.UseSqlServer("Data:DCTEServiceDesk:ConnectionString"));
                services.AddSingleton<CreateViewModel<LoginViewModel>>(service =>
                {
                    LoginInterfaceContainer interfaceContainer = new LoginInterfaceContainer()
                    {
                        EmployeeProfile = service.GetRequiredService<IEmployeeProfile>(),
                        Authorization = service.GetRequiredService<IAuthorization>()
                    };

                    return () => new LoginViewModel(
                        service.GetRequiredService<IViewForwarding>(),
                        service.GetRequiredService<IServiceDeskViewModelFactory>(),
                        interfaceContainer);
                });

                services.AddSingleton<CreateViewModel<HomeViewModel>>(service => 
                {
                    DbInterfaceContainer interfaceContainer = new DbInterfaceContainer()
                    { 
                        EmployeeProfile = service.GetRequiredService<IEmployeeProfile>(), 
                        RequestQueue = service.GetRequiredService<IRequestQueue>(),
                        UserInfo = service.GetRequiredService<IUserInfo>()
                    };
                    return () => new HomeViewModel(
                        service.GetRequiredService<ILoggedUser>(),
                        interfaceContainer);
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
        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
    }
}
