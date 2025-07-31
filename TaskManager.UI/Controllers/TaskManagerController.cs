using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Domain.RepositoryContracts;
using WebApiTaskManager.Core.Domain.Entities;

namespace WebApiTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskManagerController : ControllerBase
    {
        private readonly ITaskManagerRepository _taskManagerRepository;
        private readonly ILogger<TaskManagerController> _logger;
        public TaskManagerController(
            ITaskManagerRepository taskManagerRepository,
            ILogger<TaskManagerController> logger
            )
        {
            _logger = logger;
            _taskManagerRepository = taskManagerRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Project>>> GetAllProjects()
        {
            try
            {
                var projects = await _taskManagerRepository.GetAllProjectsAsync();

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

    }
}
