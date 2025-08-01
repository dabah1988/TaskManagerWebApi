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
                    _logger.LogError("La suppresson projet {project} a échoué", project);
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
                    _logger.LogWarning("{project} Le projet envoyé is null.", projectForUpdate);
                    return BadRequest("Projet null");
                }
                bool result =  await _serviceTaskManager.UpdateAsync(projectForUpdate);
                if (result) return Ok(projectForUpdate);
                else
                {
                    _logger.LogError("La mise à jour du projet {project} a échoué", projectForUpdate);
                    return StatusCode(500, "La mise à jour du projet a échoué.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des projets.");
                return StatusCode(500, "Erreur serveur interne.");
            }
        }

    }
}
