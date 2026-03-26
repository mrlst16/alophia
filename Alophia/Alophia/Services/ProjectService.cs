using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Alophia.Models;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Alophia.Services;

public class ProjectService : IProjectService
{
    public async Task<AlophiaProject?> OpenAsync(Window ownerWindow)
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.List,
            FileTypeFilter = { ".ap" }
        };

        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(ownerWindow));
        var file = await picker.PickSingleFileAsync();

        if (file == null) return null;

        return await LoadAsync(file.Path);
    }

    public async Task SaveAsync(AlophiaProject project, string path)
    {
        try
        {
            var json = JsonSerializer.Serialize(project, AlophiaJsonContext.Default.AlophiaProject);
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
            await Windows.Storage.FileIO.WriteTextAsync(file, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save project to {path}", ex);
        }
    }

    public async Task<AlophiaProject?> LoadAsync(string path)
    {
        try
        {
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
            var json = await Windows.Storage.FileIO.ReadTextAsync(file);
            var project = JsonSerializer.Deserialize(json, AlophiaJsonContext.Default.AlophiaProject);
            return project;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load project from {path}", ex);
        }
    }

    public async Task<AlophiaProject?> CreateNewProjectAsync(string name, string contextPath, IList<string> selectedFolders)
    {
        // Define fixed stages (6)
        var fixedStages = new List<Stage>
        {
            new() { Id = "requirements", Label = "Requirements", Bot = "System", Start = 0, Duration = 2, ColorSolid = "#BA7517", ColorLight = "#FAEEDA" },
            new() { Id = "ux", Label = "UX Design", Bot = "System", Start = 2, Duration = 3, ColorSolid = "#993C1D", ColorLight = "#FAECE7" },
            new() { Id = "architecture", Label = "Architecture", Bot = "System", Start = 2, Duration = 3, ColorSolid = "#534AB7", ColorLight = "#EEEDFE" },
        };

        // Project stages (user-selected folders, Start=5, Duration=5 each)
        var projectColors = new[]
        {
            ("#185FA5", "#E6F1FB"),
            ("#0F6E56", "#E1F5EE"),
            ("#1D6FA5", "#B5D4F4"),
            ("#534AB7", "#EEEDFE"),
        };

        var projectStages = new List<Stage>();
        for (int i = 0; i < selectedFolders.Count; i++)
        {
            var folderName = System.IO.Path.GetFileName(selectedFolders[i]);
            var colorIdx = i % projectColors.Length;
            projectStages.Add(new()
            {
                Id = $"project-{i}",
                Label = folderName,
                Bot = "System",
                Start = 5,
                Duration = 5,
                FolderPath = selectedFolders[i],
                ColorSolid = projectColors[colorIdx].Item1,
                ColorLight = projectColors[colorIdx].Item2,
            });
        }

        // Fixed stages after projects
        var finalStages = new List<Stage>(fixedStages);
        finalStages.AddRange(projectStages);
        finalStages.AddRange(new[]
        {
            new Stage { Id = "be-testing", Label = "Back End Testing", Bot = "System", Start = 10, Duration = 3, ColorSolid = "#993556", ColorLight = "#FBEAF0" },
            new Stage { Id = "e2e", Label = "E2E Testing", Bot = "System", Start = 10, Duration = 3, ColorSolid = "#3C3489", ColorLight = "#CECBF6" },
            new Stage { Id = "docs", Label = "Documentation", Bot = "System", Start = 13, Duration = 2, ColorSolid = "#A32D2D", ColorLight = "#FCEBEB" },
        });

        var filePath = System.IO.Path.Combine(contextPath, $"{name}.ap");

        var project = new AlophiaProject
        {
            Title = name,
            TotalWeeks = 16,
            ContextPath = contextPath,
            FilePath = filePath,
            Stages = finalStages,
            Bots = [ new Bot { Name = "System", Initials = "SY", BgColor = "#F0F0F0", FgColor = "#333333" } ],
        };

        // Save to file
        try
        {
            var folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(contextPath);
            var file = await folder.CreateFileAsync($"{name}.ap", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await SaveAsync(project, file.Path);
            return project;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create project at {filePath}", ex);
        }
    }

    public AlophiaProject CreateSampleProject()
    {
        return new AlophiaProject
        {
            Title = "State Filings Platform",
            TotalWeeks = 16,
            Bots =
            [
                new Bot { Name = "BeckyBot", Initials = "BB", BgColor = "#FAEEDA", FgColor = "#BA7517" },
                new Bot { Name = "UXBot", Initials = "UX", BgColor = "#FAECE7", FgColor = "#993C1D" },
                new Bot { Name = "OlgaBot", Initials = "OB", BgColor = "#EEEDFE", FgColor = "#534AB7" },
                new Bot { Name = "CarlosBot", Initials = "CB", BgColor = "#E6F1FB", FgColor = "#185FA5" },
                new Bot { Name = "MattBot", Initials = "MB", BgColor = "#E1F5EE", FgColor = "#0F6E56" },
                new Bot { Name = "SvetlanaBot", Initials = "SB", BgColor = "#FBEAF0", FgColor = "#993556" },
                new Bot { Name = "MatBot", Initials = "MT", BgColor = "#CECBF6", FgColor = "#3C3489" },
                new Bot { Name = "DocuBot", Initials = "DB", BgColor = "#FCEBEB", FgColor = "#A32D2D" },
            ],
            Stages =
            [
                new Stage { Id = "requirements", Label = "Requirements", Bot = "BeckyBot", Start = 0, Duration = 2, Actual = 2, ColorSolid = "#BA7517", ColorLight = "#FAEEDA" },
                new Stage { Id = "ux", Label = "UX Design", Bot = "UXBot", Start = 2, Duration = 3, Actual = 2, ColorSolid = "#993C1D", ColorLight = "#FAECE7" },
                new Stage { Id = "architecture", Label = "Architecture", Bot = "OlgaBot", Start = 2, Duration = 3, Actual = 3, ColorSolid = "#534AB7", ColorLight = "#EEEDFE" },
                new Stage { Id = "sf", Label = "State Filings", Bot = "CarlosBot", Start = 5, Duration = 5, Actual = 6, ColorSolid = "#185FA5", ColorLight = "#E6F1FB" },
                new Stage { Id = "sf-api", Label = "State Filings API", Bot = "MattBot", Start = 5, Duration = 5, Actual = 4, ColorSolid = "#0F6E56", ColorLight = "#E1F5EE" },
                new Stage { Id = "wc", Label = "Web Components", Bot = "CarlosBot", Start = 5, Duration = 4, Actual = 4, ColorSolid = "#1D6FA5", ColorLight = "#B5D4F4" },
                new Stage { Id = "be-testing", Label = "Back End Testing", Bot = "SvetlanaBot", Start = 10, Duration = 3, Actual = 3, ColorSolid = "#993556", ColorLight = "#FBEAF0" },
                new Stage { Id = "e2e", Label = "E2E Testing", Bot = "MatBot", Start = 10, Duration = 3, Actual = 3, ColorSolid = "#3C3489", ColorLight = "#CECBF6" },
                new Stage { Id = "docs", Label = "Documentation", Bot = "DocuBot", Start = 13, Duration = 2, Actual = 2, ColorSolid = "#A32D2D", ColorLight = "#FCEBEB" },
            ],
            Requirements =
            [
                new Requirement
                {
                    Id = "EP-1",
                    Type = "epic",
                    Title = "State Filings Platform",
                    Status = "in progress",
                    Assignee = "BeckyBot",
                    Description = "Build a full-stack platform allowing users to submit, track, and manage state filing documents across all US states and territories.",
                    Acceptance =
                    [
                        "Users can submit filings for any US state",
                        "Filing status is tracked in real time",
                        "All actions are audit logged",
                        "Platform is accessible on mobile and desktop"
                    ],
                    Tasks =
                    [
                        new RequirementTask
                        {
                            Id = "ST-1",
                            Type = "story",
                            Title = "Filing submission form",
                            Status = "done",
                            Assignee = "CarlosBot",
                            Description = "Build the multi-step form for submitting a new state filing.",
                            Acceptance = ["Form validates state and year fields", "Document upload supports PDF and DOCX"]
                        },
                        new RequirementTask
                        {
                            Id = "ST-2",
                            Type = "story",
                            Title = "Filing status tracking",
                            Status = "in progress",
                            Assignee = "MattBot",
                            Description = "Implement real-time status tracking for submitted filings.",
                            Acceptance = ["Status updates within 5 seconds", "Users can see full status history"]
                        },
                    ]
                },
                new Requirement
                {
                    Id = "EP-2",
                    Type = "epic",
                    Title = "Web Component Library",
                    Status = "done",
                    Assignee = "BeckyBot",
                    Description = "Create a shared reusable component library used across all front-end projects.",
                    Acceptance = ["Components published as an npm package", "Storybook documentation available", "Accessible (WCAG 2.1 AA)"],
                    Tasks = []
                }
            ],
            Repositories =
            [
                new Repository
                {
                    Id = "sf",
                    Label = "State Filings",
                    Color = "#185FA5",
                    Commits =
                    [
                        new Commit
                        {
                            Hash = "a1b2c3d",
                            Author = "CarlosBot",
                            Message = "Add FilingsPage and FilingsTable",
                            Time = "w6",
                            Files =
                            [
                                new FileDiff
                                {
                                    File = "pages/filings/index.tsx",
                                    Lines = ["+  export default function FilingsPage() {", "+    return <FilingsTable data={useFilings()} />", "+  }"]
                                }
                            ]
                        }
                    ]
                }
            ],
            DirectMessages = new Dictionary<string, IReadOnlyList<DmMessage>>
            {
                { "BeckyBot", new List<DmMessage> { new DmMessage { From = "BeckyBot", Text = "Requirements locked. Ready to go.", Time = "w2" } }.AsReadOnly() },
                { "UXBot", new List<DmMessage> { new DmMessage { From = "UXBot", Text = "Wireframes done.", Time = "w3" } }.AsReadOnly() },
            }
        };
    }
}
