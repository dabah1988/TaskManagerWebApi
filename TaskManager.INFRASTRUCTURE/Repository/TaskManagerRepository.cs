using Microsoft.Extensions.Logging;
using TaskManager.Core.Domain.RepositoryContracts;
using TaskManager.Core.DTO;
using TaskManager.Infrastructure.DatabaseContext;
using WebApiTaskManager.Core.Domain.Entities;
using AutoMapper;
using TaskManager.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Infrastructure.Repository
{
    public class TaskManagerRepository : ITaskManagerRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TaskManagerRepository> _logger;
        private readonly IMapper _mapper;
        public TaskManagerRepository(
            ApplicationDbContext dbContext,
            ILogger<TaskManagerRepository> logger,
            IMapper mapper

            )
        {
             _logger = logger;  
            _dbContext = dbContext;
            _mapper = mapper;   
        }
        public async Task<ProjectResponse> AddProjectAsync(Project project)
        {
            try
            {
                if (project == null)
                {
                    _logger.LogError("   {project} is null", project);
                    throw new ArgumentNullException($" {nameof(project)} is null ");
                }
                await _dbContext.AddAsync(project);
                int rowsAffected = await _dbContext.SaveChangesAsync();
                if (rowsAffected == 0)
                {
                    _logger.LogError(" l'insertion de {project} a échoué", project);
                    throw new Exception("Échec de l'insertion du projet.");
                }
                var projectResponse = _mapper.Map<ProjectResponse>(project);
                return projectResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(" Erreur survenue {ex.message}", ex.Message);
                throw new Exception($"Échec de l'insertion du projet {ex.Message}");
            }
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId)
        {
            try
            {

                if (projectId == Guid.Empty )
                {
                    _logger.LogError("   {projectid} is empty", projectId);
                    throw new ArgumentNullException($" {nameof(projectId)} is empty ");
                }

                Project? project = await _dbContext.Projects!.FirstOrDefaultAsync(p => p.ProjectId == projectId);
                if(project == null)
                {
                    _logger.LogWarning(" project not found {project}", project);
                    throw new NotFoundException($"Projet avec l'ID {projectId} non trouvé.");
                }
                _dbContext.Projects!.Remove(project);
                int affectedRows = await _dbContext.SaveChangesAsync();
                if (affectedRows == 0)
                {
                    _logger.LogWarning("Suppression échouée pour le projet avec ID {projectId}", projectId);
                    throw new Exception("La suppression n'a pas pu être effectuée.");
                }
                _logger.LogInformation("Projet supprimé avec succès : {projectId}", projectId);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(" Error occured {ex}", ex.Message);
                throw new Exception($"Error occured {ex.Message} non trouvé.");
            }
        }

        public Task<List<Project>> GetProjectsAsync(int pageNumber,int pageSize)
        {
            try
            {
                return _dbContext.Projects!
                    .OrderByDescending(p =>p.DateOfStart)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(" Error occured {ex}", ex.Message);
                throw new Exception($"Error occured {ex.Message} non trouvé.");
            }
        }

        public async Task<Project?> GetProjectByIdAsync(Guid projectId)
        {
            try
            {

                if (projectId == Guid.Empty)
                {
                    _logger.LogError("   {projectid} is empty", projectId);
                    throw new ArgumentNullException($" {nameof(projectId)} is empty ");
                }
                Project? project =await  _dbContext.Projects!.FirstOrDefaultAsync(x => x.ProjectId == projectId);
                if (project == null)
                {
                    _logger.LogWarning("projet with id {projectid} not found ", projectId);
                    throw new NotFoundException($" product with Id : {projectId} is not found ");
                }
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(" Error occured {ex}", ex.Message);
                throw new Exception($"Error occured {ex.Message} non trouvé.");
            }
        }

        public async Task<bool> UpdateProjectAsync(Project projectToUpdate)
        {
            try
            {
                if (projectToUpdate == null)
                {
                    _logger.LogError(" {@project} is null", projectToUpdate);
                    throw new ArgumentNullException($" {nameof(projectToUpdate)} is null ");
                }
                Project? project = await _dbContext.Projects!.FirstOrDefaultAsync(x => x.ProjectId == projectToUpdate.ProjectId);
                if (project == null)
                {
                    _logger.LogError("projet not is null ");
                    throw new NotFoundException($" product   is null ");
                }
                project.ProjectName = projectToUpdate.ProjectName;
                project.DateOfStart = projectToUpdate.DateOfStart;
                project.TeamSize = projectToUpdate.TeamSize;
                project.ProjectDescription = projectToUpdate.ProjectDescription;
                int result = _dbContext.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(" Error occured {ex}", ex.Message);
                throw new Exception($"Error occured {ex.Message} non trouvé.");
            }
        }

        public async Task<List<Project>> SearchProjectsAsync(int pageNumber, int pageSize, string searchBy, string searchText)
        {
            var query = _dbContext.Projects.AsQueryable();

            switch (searchBy)
            {
                case nameof(Project.ProjectName):
                    query = query.Where(p => p.ProjectName.Contains(searchText));
                    break;
                case nameof(Project.DateOfStart) :
                    if (DateTime.TryParse(searchText, out var date))
                        query = query.Where(p => p.DateOfStart == date);
                    break;
                case nameof(Project.ProjectDescription) :
                    query = query.Where(p => p.ProjectDescription.Contains(searchText));
                    break;
                case nameof( Project.TeamSize):
                    if (int.TryParse(searchText, out var size))
                        query = query.Where(p => p.TeamSize == size);
                    break;
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
