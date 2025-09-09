using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.DTO;
using WebApiTaskManager.Core.Domain.Entities;

namespace TaskManager.Core.ServicesContract
{
    public interface ITaskManagerService
    {
        Task<Project?> GetProjectByid(Guid projectId);
        Task<List<Project>> GetProjects(int pageNumber,int pageSize);
        Task<List<Project>> SearchProjectsAsync(int pageNumber, int pageSize,string searchBy, string searchText);
        Task<ProjectResponse> AddProject(ProjectAddRequest projectAddRequest);
        Task<bool> UpdateAsync(Project project);
        Task<bool> DeleteAsync(Guid projectId);
    }
}
