using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManager.Core.Domain.RepositoryContracts;
using TaskManager.Core.DTO;
using TaskManager.Core.ServicesContract;
using TaskManager.UI.Utilities;
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
        private readonly IConfiguration _configuration;
        private readonly paginationSettings _paginationSettings;
        public TaskManagerController(
            ITaskManagerService serviceTaskManager,
            ILogger<TaskManagerController> logger,
            IMapper mapper,
            IConfiguration configuration ,
            IOptions<paginationSettings> paginationSettings
            )
        {
            _logger = logger;
            _serviceTaskManager = serviceTaskManager;
            _mapper = mapper;
            _configuration = configuration;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponse>>> GetProjects( [FromQuery] int pageNumber = 1, [FromQuery] int? pageSize = null )
        {             
                var pageSizeTemp = pageSize.GetValueOrDefault(_paginationSettings.NumberElementsByPage);
            if (pageNumber <= 0) pageNumber = 1;
          if(pageSizeTemp <= 0)      pageSize = 10;

            var projects = await _serviceTaskManager.GetProjects(pageNumber, pageSizeTemp);
            if (projects == null  || !projects.Any())
                {
                    _logger.LogInformation("Aucun projet trouvé.");
                    return StatusCode(404,"Not found projects.");
                }
                var projectResponses = _mapper.Map<List<ProjectResponse>>(projects);
                _logger.LogInformation("Projects founds {@projects}", projectResponses.Count);
                return Ok(projectResponses);           
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProjectResponse>>> Search( [FromQuery] string searchBy, [FromQuery] string searchText, [FromQuery] int pageNumber = 1, [FromQuery] int? pageSize = null)
        {
            var pageSizeTemp = pageSize.GetValueOrDefault(_paginationSettings.NumberElementsByPage);
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSizeTemp <= 0) pageSize = 10;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return BadRequest("Search text cannot be empty.");
            }
            var projects = await _serviceTaskManager.SearchProjectsAsync(pageNumber, pageSizeTemp, searchBy, searchText);
            if (projects == null || !projects.Any())
            {
                _logger.LogInformation("Aucun projet trouvé.");
                return NotFound("Aucun projet disponible.");
            }
            var projectResponses = _mapper.Map<List<ProjectResponse>>(projects);
            _logger.LogInformation("Projects founds {@projects}", projectResponses.Count);
            return Ok(projectResponses);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject([FromBody]ProjectAddRequest projectAddRequest)
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
                _logger.LogInformation("insertion dans la base de données réussie ; {@project}",projectResponse);
                return Ok(projectResponse);            
         
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
