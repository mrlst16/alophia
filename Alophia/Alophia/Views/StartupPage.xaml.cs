using Alophia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Alophia.Views
{
    public sealed partial class StartupPage : Page
    {
        private MainViewModel ViewModel { get; set; }

        public StartupPage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

            LoadProjectButton.Click += async (s, e) =>
            {
                var window = (Application.Current as App)?.Window as MainWindow;
                if (window != null)
                {
                    var project = await ViewModel.ProjectService.OpenAsync(window);
                    if (project != null)
                    {
                        ViewModel.Project = project;
                        ViewModel.PipelineTitle = project.Title;
                        ((Frame)window.Content).Navigate(typeof(ShellPage));
                    }
                }
            };

            CreateProjectButton.Click += (s, e) =>
            {
                ((Frame)((Application.Current as App)?.Window as MainWindow)?.Content).Navigate(typeof(NewProjectWizardPage));
            };
        }
    }
}
