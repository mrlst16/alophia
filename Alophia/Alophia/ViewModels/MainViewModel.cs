using Alophia.Models;
using Alophia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alophia.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IProjectService _projectService;
    private readonly ISimulationService _simulationService;

    [ObservableProperty]
    private AlophiaProject? project;

    [ObservableProperty]
    private string projectPath = string.Empty;

    [ObservableProperty]
    private bool isBrowsingPath;

    [ObservableProperty]
    private string pipelineTitle = "Alophia";

    [ObservableProperty]
    private int ticksElapsed;

    [ObservableProperty]
    private bool isRunning;

    public MainViewModel(IProjectService projectService, ISimulationService simulationService)
    {
        _projectService = projectService;
        _simulationService = simulationService;

        // Wire up simulation service
        if (_simulationService is SimulationService simService)
        {
            simService.Tick += (s, e) =>
            {
                TicksElapsed = e.TicksElapsed;
                IsRunning = _simulationService.IsRunning;
            };
        }

        // Load sample project by default
        Project = projectService.CreateSampleProject();
        PipelineTitle = Project.Title;
    }

    public async Task OpenProject(Microsoft.UI.Xaml.Window owner)
    {
        var project = await _projectService.OpenAsync(owner);
        if (project != null)
        {
            Project = project;
            PipelineTitle = project.Title;
            ProjectPath = project.ActiveProjectPath ?? string.Empty;
        }
    }

    public async Task SaveProject(Microsoft.UI.Xaml.Window owner)
    {
        if (Project == null) return;

        var picker = new Windows.Storage.Pickers.FileSavePicker();
        picker.FileTypeChoices.Add("Alophia Project", new List<string> { ".ap" });
        picker.DefaultFileExtension = ".ap";
        picker.SuggestedFileName = Project.Title;

        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(owner));

        var file = await picker.PickSaveFileAsync();
        if (file != null)
        {
            await _projectService.SaveAsync(Project, file.Path);
            ProjectPath = file.Path;
            Project = Project with { ActiveProjectPath = file.Path };
        }
    }

    [RelayCommand]
    public void Run()
    {
        _simulationService.Start();
        IsRunning = _simulationService.IsRunning;
    }

    [RelayCommand]
    public void Pause()
    {
        _simulationService.Pause();
        IsRunning = _simulationService.IsRunning;
    }

    [RelayCommand]
    public void Reset()
    {
        _simulationService.Reset();
        TicksElapsed = 0;
        IsRunning = _simulationService.IsRunning;
    }

    [RelayCommand]
    public void ToggleBrowsePath()
    {
        IsBrowsingPath = !IsBrowsingPath;
    }
}
