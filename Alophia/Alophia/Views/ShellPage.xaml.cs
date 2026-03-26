using Alophia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Alophia.Views
{
    public sealed partial class ShellPage : Page
    {
        private MainViewModel ViewModel { get; set; }

        public ShellPage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
            DataContext = ViewModel;

            // Bind UI elements
            PipelineTitle.Text = ViewModel.PipelineTitle;
            ContextPathBox.Text = ViewModel.ProjectPath;

            // Wire up commands
            ResetButton.Click += (s, e) => ViewModel.ResetCommand.Execute(null);
            RunPauseButton.Click += (s, e) =>
            {
                if (ViewModel.IsRunning)
                    ViewModel.PauseCommand.Execute(null);
                else
                    ViewModel.RunCommand.Execute(null);
            };

            // Wire up tab navigation
            StagesTab.Click += (s, e) => TabContent.Navigate(typeof(StagesPage));
            RequirementsTab.Click += (s, e) => TabContent.Navigate(typeof(RequirementsPage));
            ArchitectureTab.Click += (s, e) => TabContent.Navigate(typeof(ArchitecturePage));
            DiffTab.Click += (s, e) => TabContent.Navigate(typeof(DiffPage));
            ChatTab.Click += (s, e) => TabContent.Navigate(typeof(ChatPage));

            // Update UI on property changes
            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.IsRunning))
                {
                    RunPauseButton.Content = ViewModel.IsRunning ? "Pause" : "Run";
                }
                if (e.PropertyName == nameof(ViewModel.TicksElapsed))
                {
                    int week = ViewModel.TicksElapsed / 4;
                    WeekIndicator.Text = ViewModel.IsRunning
                        ? $"Week {week} / 16"
                        : "Not running";
                }
                if (e.PropertyName == nameof(ViewModel.PipelineTitle))
                {
                    PipelineTitle.Text = ViewModel.PipelineTitle;
                }
            };

            // Navigate to first tab
            TabContent.Navigate(typeof(StagesPage));
        }
    }
}
