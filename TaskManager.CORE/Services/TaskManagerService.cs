using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManager.Core.Domain.RepositoryContracts;
using TaskManager.Core.DTO;
using TaskManager.Core.ServicesContract;
using WebApiTaskManager.Core.Domain.Entities;

namespace TaskManager.Core.Services
{
    public class TaskManagerService : ITaskManagerService
    {
        private readonly ITaskManagerRepository _taskManagerRepository;
        private readonly ILogger<TaskManagerService> _logger;
        private readonly IMapper _mapper;
        public TaskManagerService(
            ITaskManagerRepository taskManagerRepository,
            ILogger<TaskManagerService> logger,
            IMapper mapper
            )
        {
            _taskManagerRepository = taskManagerRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ProjectResponse> AddProject(ProjectAddRequest projectAddRequest)
        {
            try
            {
                if (projectAddRequest == null)
                {
                    _logger.LogError("{project} is null", projectAddRequest);
                    throw new ArgumentNullException($"{{projectId}} is null");
                }
                if ( string.IsNullOrWhiteSpace(projectAddRequest.ProjectName) )
                {
                    _logger.LogError("{projectname} is null or empty", projectAddRequest.ProjectName);
                    throw new ArgumentNullException($"{{projectname}} is null or empty", projectAddRequest.ProjectName);
                }
                if ( projectAddRequest.TeamSize <= 0)
                {
                    _logger.LogError("{projectAddRequest.TeamSize} ne peut être inférieur ou égal 0", projectAddRequest.TeamSize);
                    throw new ArgumentException($"{projectAddRequest.TeamSize} ne peut être inférieu ou égal à 0", projectAddRequest.TeamSize.ToString());                   
                }

                var project = _mapper.Map<Project>( projectAddRequest );
                ProjectResponse productResponse = (await  _taskManagerRepository.AddProjectAsync(project));  
                return productResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                _logger.LogError("{projectid} is null", projectId);
                throw new ArgumentNullException($"{{projectId}} is null");
            }
            Project? project = await _taskManagerRepository.GetProjectByIdAsync(projectId);
            if (project == null)
            {
                _logger.LogError("{project} is null", project);
                throw new ArgumentNullException($"{{projectId}} is null");
            }
            bool result = await _taskManagerRepository.DeleteProjectAsync(projectId);
            return result;
        }

        public async Task<List<Project>> GetAllProjects()
        {
            try
            {
                _logger.LogInformation("Récupération de tous les projets depuis la base de données.");
                var projects= await _taskManagerRepository.GetAllProjectsAsync();
                if(projects == null || !projects.Any())
                {
                    _logger.LogError("database project is null {projects}", projects);
                    throw new ArgumentNullException("database project is null ");
                }
                return projects  ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                throw;
            }
        }

        public async Task<Project?> GetProjectByid(Guid projectId)
        {
            try
            {
                if (projectId == Guid.Empty)
            {
                _logger.LogError("{projectId} is empty",projectId);
                throw new ArgumentNullException($"{{projectId}} is empty");
            }
            var project = await _taskManagerRepository.GetProjectByIdAsync(projectId);
            if(project == null)
            {
                _logger.LogWarning("{project} is null", project);
            }
            return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Project projectToUpdate)
        {
            try
            {
                if (projectToUpdate == null)
                {
                    _logger.LogError("{project} is null", projectToUpdate);
                    throw new ArgumentNullException($"{{projectId}} is empty");
                }
                bool result = await _taskManagerRepository.UpdateProjectAsync(projectToUpdate);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                throw;
            }
        }
    }
}
