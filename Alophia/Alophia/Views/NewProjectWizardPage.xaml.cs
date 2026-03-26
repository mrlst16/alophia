using Alophia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage.Pickers;

namespace Alophia.Views
{
    public sealed partial class NewProjectWizardPage : Page
    {
        private MainViewModel ViewModel { get; set; }
        private int CurrentStep = 1;
        private string SelectedContextPath = string.Empty;
        private List<string> AvailableFolders = new();
        private HashSet<string> SelectedFolders = new();

        public NewProjectWizardPage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

            BrowseFolderButton.Click += async (s, e) => await BrowseForFolder();
            BackButton.Click += (s, e) => GoToStep(CurrentStep - 1);
            NextButton.Click += (s, e) => GoToStep(CurrentStep + 1);
            CreateButton.Click += async (s, e) => await CreateProject();

            ShowStep(1);
        }

        private async System.Threading.Tasks.Task BrowseForFolder()
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");

            var window = (Application.Current as App)?.Window as MainWindow;
            if (window != null)
            {
                WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(window));
                var folder = await picker.PickSingleFolderAsync();

                if (folder != null)
                {
                    SelectedContextPath = folder.Path;
                    FolderPathBox.Text = SelectedContextPath;

                    // Load subfolders
                    AvailableFolders.Clear();
                    SelectedFolders.Clear();

                    try
                    {
                        var subfolders = await folder.GetFoldersAsync();
                        AvailableFolders.AddRange(subfolders.Select(f => f.Path));
                    }
                    catch { }

                    ShowFolderList();
                }
            }
        }

        private void ShowFolderList()
        {
            var items = new List<CheckBox>();
            foreach (var folderPath in AvailableFolders)
            {
                var folderName = System.IO.Path.GetFileName(folderPath);
                var checkbox = new CheckBox
                {
                    Content = folderName,
                    Tag = folderPath,
                    Margin = new Thickness(0, 4, 0, 4)
                };
                checkbox.Checked += (s, e) => SelectedFolders.Add(folderPath);
                checkbox.Unchecked += (s, e) => SelectedFolders.Remove(folderPath);
                items.Add(checkbox);
            }
            FoldersList.ItemsSource = items;
        }

        private void ShowStep(int stepNum)
        {
            CurrentStep = stepNum;

            // Hide all steps
            Step1.Visibility = Visibility.Collapsed;
            Step2.Visibility = Visibility.Collapsed;
            Step3.Visibility = Visibility.Collapsed;

            // Show current step
            switch (stepNum)
            {
                case 1:
                    Step1.Visibility = Visibility.Visible;
                    StepTitle.Text = "New Project - Step 1: Basic Info";
                    StepProgress.Value = 33;
                    BackButton.IsEnabled = false;
                    NextButton.Visibility = Visibility.Visible;
                    CreateButton.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    Step2.Visibility = Visibility.Visible;
                    StepTitle.Text = "New Project - Step 2: Select Folders";
                    StepProgress.Value = 66;
                    BackButton.IsEnabled = true;
                    NextButton.Visibility = Visibility.Visible;
                    CreateButton.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    Step3.Visibility = Visibility.Visible;
                    StepTitle.Text = "New Project - Step 3: Review";
                    StepProgress.Value = 100;
                    BackButton.IsEnabled = true;
                    NextButton.Visibility = Visibility.Collapsed;
                    CreateButton.Visibility = Visibility.Visible;
                    UpdateSummary();
                    break;
            }
        }

        private void GoToStep(int stepNum)
        {
            if (stepNum >= 1 && stepNum <= 3)
            {
                // Validate step 1
                if (CurrentStep == 1 && string.IsNullOrWhiteSpace(ProjectNameBox.Text))
                {
                    return;
                }
                if (CurrentStep == 1 && string.IsNullOrWhiteSpace(SelectedContextPath))
                {
                    return;
                }

                ShowStep(stepNum);
            }
        }

        private void UpdateSummary()
        {
            SummaryProjectName.Text = ProjectNameBox.Text;
            SummaryFolder.Text = SelectedContextPath;
            SummaryStages.Text = $"{SelectedFolders.Count} selected";
        }

        private async System.Threading.Tasks.Task CreateProject()
        {
            try
            {
                await ViewModel.CreateNewProject(
                    ProjectNameBox.Text,
                    SelectedContextPath,
                    SelectedFolders.ToList()
                );

                // Navigate to ShellPage
                var frame = ((Frame)((Application.Current as App)?.Window as MainWindow)?.Content);
                frame?.Navigate(typeof(ShellPage));
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error Creating Project",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }
    }
}
