using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Domain.RepositoryContracts;
using TaskManager.Core.DTO;
using TaskManager.Core.ServicesContract;
using WebApiTaskManager.Core.Domain.Entities;

namespace WebApiTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskManagerController : ControllerBase
    {
        private readonly ITaskManagerService _serviceTaskManager;
        private readonly ILogger<TaskManagerController> _logger;
        private readonly IMapper _mapper;
        public TaskManagerController(
            ITaskManagerService serviceTaskManager,
            ILogger<TaskManagerController> logger,
            IMapper mapper
            )
        {
            _logger = logger;
            _serviceTaskManager = serviceTaskManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Project>>> GetAllProjects()
        {
            try
            {
                var projects = await _serviceTaskManager.GetAllProjects();
                if (projects == null || !projects.Any())
                {
                    _logger.LogInformation("Aucun projet trouvé.");
                    return NotFound("Aucun projet disponible.");
                }
                _logger.LogInformation("Projects founds {@projects}", projects);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                return StatusCode(500, "Erreur serveur interne.");
            }
        }

        [HttpGet("search/{seachBy}/{searchText}")]
        public async Task<ActionResult<List<Project>>> Search(string seachBy,string seachText)
        {
            try
            {
               var projects = await _serviceTaskManager.GetAllProjects(); ; 
                switch (seachBy)
                {
                    case nameof(Project.ProjectName):
                        projects = projects.Where(p => p.ProjectName.Contains(seachText)).ToList();  break;

                    case nameof(Project.DateOfStart):
                        projects = projects.Where(p => p.DateOfStart.Equals(seachText)).ToList(); break;

                    case nameof(Project.ProjectDescription):
                        projects = projects.Where(p => p.ProjectDescription.Contains(seachText)).ToList(); break;

                    case nameof(Project.TeamSize):
                        projects = projects.Where(p => p.TeamSize.Equals(seachText)).ToList(); break;

                }
                if (projects == null || !projects.Any())
                {
                    _logger.LogInformation("Aucun projet trouvé.");
                    return NotFound("Aucun projet disponible.");
                }
                _logger.LogInformation("Projects founds {@projects}",projects);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                return StatusCode(500, "Erreur serveur interne.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProject([FromBody]ProjectAddRequest projectAddRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // retourne les erreurs de validation
                }
                if (projectAddRequest == null)
                {
                    _logger.LogWarning("Le projet envoyé est null.");
                    return BadRequest("Projet invalide.");
                }
                ProjectResponse projectResponse = await _serviceTaskManager.AddProject(projectAddRequest);
                if (projectResponse == null)
                {
                    _logger.LogError("Le service a retourné null après tentative d'ajout.");
                    return StatusCode(500, "Erreur lors de la création du projet.");
                }
                _logger.LogInformation("inserttion dans la base de données réussie ; {@project}",projectResponse);
                return Ok(projectResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                return StatusCode(500, "Erreur serveur interne.");
            }
        }


        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject( Guid projectId)
        {
            try
            {
                if (projectId == Guid.Empty)
                {
                    _logger.LogWarning("{project} Le projet envoyé is null.", projectId);
                    return BadRequest("Projet null");
                }
                Project? project = await _serviceTaskManager.GetProjectByid(projectId);
                if (project == null)           return NotFound();
           
                
               bool result = await _serviceTaskManager.DeleteAsync(projectId);
                if(result)   return Ok(project);
                else
                {
                    _logger.LogError("La suppresson projet {@project} a échoué", project);
                    return StatusCode(500, "La suppression du projet a échoué.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                return StatusCode(500, "Erreur serveur interne.");
            }
        }

        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject([FromBody]Project projectForUpdate)
        {
            try
            {
                if (projectForUpdate == null)
                {
                    _logger.LogWarning("{@project} Le projet envoyé is null.", projectForUpdate);
                    return BadRequest("Projet null");
                }
                bool result =  await _serviceTaskManager.UpdateAsync(projectForUpdate);

                if (!result)
                {
                    _logger.LogError("La mise à jour du projet {@project} a échoué", projectForUpdate);
                    return StatusCode(500, "La mise à jour du projet a échoué.");
                }
                else   return Ok(projectForUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                return StatusCode(500, "Erreur serveur interne.");
            }
        }

    }
}
