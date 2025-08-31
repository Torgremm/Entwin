using System.Net.Http.Json;
using System.Text.Json;
using Entwin.Shared.Data;
using Entwin.Shared.Models;

namespace Entwin.Client.Services
{
    public class LoadService
    {
        private readonly HttpClient _http;

        public List<ProjectSaveDTO> AvailableProjects { get; private set; } = new();
        public ProjectSaveDTO? CurrentProject { get; private set; }

        public LoadService(HttpClient http)
        {
            _http = http;
        }

        public async Task LoadProjectsFromServerAsync()
        {
            try
            {
                var projects = await _http.GetFromJsonAsync<List<ProjectSaveDTO>>("api/projects");
                AvailableProjects = projects ?? new List<ProjectSaveDTO>();
            }
            catch
            {
                AvailableProjects = new();
                throw;
            }
        }

        public async Task LoadProjectFromServerAsync(string projectName)
        {
            var project = await _http.GetFromJsonAsync<ProjectSaveDTO>($"api/projects/{projectName}");
            if (project == null)
                throw new InvalidOperationException("Project not found on server.");

            CurrentProject = project;
        }

        public Task LoadFromFileAsync(byte[] fileBytes)
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(fileBytes);
                var project = JsonSerializer.Deserialize<ProjectSaveDTO>(json);

                if (project == null)
                    throw new InvalidOperationException("Invalid project file.");

                CurrentProject = project;
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to parse project file.", ex);
            }
        }
    }
}
