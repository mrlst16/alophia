using Alophia.Services;
using Alophia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace Alophia
{
    public partial class App : Application
    {
        private Window? _window;

        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public App()
        {
            InitializeComponent();
            SetupServices();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }

        private static void SetupServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<ISimulationService, SimulationService>();

            // ViewModels (will be created in Phase 2+)
            services.AddSingleton<MainViewModel>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
