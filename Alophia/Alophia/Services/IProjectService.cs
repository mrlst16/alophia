using Alophia.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alophia.Services;

public interface IProjectService
{
    Task<AlophiaProject?> OpenAsync(Window ownerWindow);
    Task SaveAsync(AlophiaProject project, string path);
    Task<AlophiaProject?> LoadAsync(string path);
    Task<AlophiaProject?> CreateNewProjectAsync(string name, string contextPath, IList<string> selectedFolders);
    AlophiaProject CreateSampleProject();
}
