using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.DTO;
using WebApiTaskManager.Core.Domain.Entities;

namespace TaskManager.Core.Domain.RepositoryContracts
{
    public interface ITaskManagerRepository
    {
        Task<Project?> GetProjectByIdAsync(Guid id);
        Task<List<Project>> GetAllProjectsAsync();
        Task<ProjectResponse> AddProjectAsync(Project project);
        Task UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(Guid projectId);
    }
}
