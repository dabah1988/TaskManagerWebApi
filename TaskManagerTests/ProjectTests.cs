using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Core.Domain.RepositoryContracts;
using TaskManager.Core.DTO;
using TaskManager.Core.Services;
using TaskManager.Core.ServicesContract;
using AutoFixture;
using WebApiTaskManager.Core.Domain.Entities;

namespace TaskManagerTests
{
    public class ProjectTest
    {
        private readonly Mock<ITaskManagerRepository> _taskManagerRepositoryMock;
        private readonly Mock<ITaskManagerService> _taskManagerServiceMock;
        private readonly ITaskManagerRepository _taskManagerRepository;
        private readonly ITaskManagerService _taskManagerService;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TaskManagerService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskManagerService> _logger; 
        private readonly IFixture _fixture;

        public ProjectTest()
        {
            _taskManagerRepositoryMock = new Mock<ITaskManagerRepository>();
            _taskManagerServiceMock = new Mock<ITaskManagerService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new  Mock<ILogger<TaskManagerService>>();

            _taskManagerRepository = _taskManagerRepositoryMock.Object;
            _logger = _loggerMock.Object;
            _mapper = _mapperMock.Object;
            _taskManagerService =  new TaskManagerService(_taskManagerRepository,_logger, _mapper);
            _fixture = new Fixture();
        }
        [Fact]
        public async void AddProjet_nullProject_should_return_ArgumentNullException()
        {
            ProjectAddRequest? projectAddRequest = null;
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _taskManagerService.AddProject(projectAddRequest));
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async void AddProjet_ProjectName_isNullOrEmpty_should_return_ArgumentNullException()
        {
            ProjectAddRequest? projectAddRequest = _fixture.Build<ProjectAddRequest>()
                .With(p => p.ProjectName, String.Empty)
                .Create();
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _taskManagerService.AddProject(projectAddRequest));
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Contains("is null or empty", exception.Message);
        }

        [Fact]
        public async void AddProjet_ProjecTeamSize_lowerThan_zero_should_return_ArgumentException()
        {
            ProjectAddRequest? projectAddRequest = _fixture.Build<ProjectAddRequest>()
                .With(p => p.TeamSize, 0)
                .Create();
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _taskManagerService.AddProject(projectAddRequest));
            Assert.IsType<ArgumentException>(exception);
            Assert.Contains("ne peut être inférieu ou égal à 0", exception.Message);
        }

        [Fact]
        public async void AddProjet_should_return_ProperProjectResponse()
        {
            ProjectAddRequest? projectAddRequest = _fixture.Build<ProjectAddRequest>().Create();
            ProjectResponse? projectResponse = _fixture.Build<ProjectResponse>()
                .With(p => p.ProjectName, projectAddRequest.ProjectName)
                .With(p => p.TeamSize, projectAddRequest.TeamSize)
                .With(p => p.ProjectDescription, projectAddRequest.ProjectDescription)
                .With(p => p.DateOfStart, projectAddRequest.DateOfStart)
                .With(p => p.ProjectId, Guid.NewGuid())
                .Create();

            _mapperMock.Setup( m => m.Map<ProjectResponse>(It.IsAny<ProjectAddRequest>()))
                .Returns(projectResponse);

            Project project = _fixture.Build<Project>()
                .With(p => p.ProjectName, projectResponse.ProjectName)
                 .With(p => p.ProjectId, projectResponse.ProjectId)
                 .With(p => p.TeamSize, projectResponse.TeamSize)
                 .With(p => p.ProjectDescription, projectResponse.ProjectDescription)
                  .With(p => p.DateOfStart, projectResponse.DateOfStart)
                  .Create();

                _mapperMock.Setup(m => m.Map<Project>(It.IsAny<ProjectAddRequest>()))
                 .Returns(project);

            _taskManagerRepositoryMock.Setup(p => p.AddProjectAsync(It.Is<Project>(
               pr => pr.ProjectName == project.ProjectName &&
             pr.TeamSize == project.TeamSize)))
                .ReturnsAsync(projectResponse);

            var result = await _taskManagerService.AddProject(projectAddRequest);
            Assert.IsType<ProjectResponse>(result);
            Assert.NotNull(projectResponse);
            Assert.Equal(projectResponse.ProjectName, project.ProjectName);
        }

        [Fact]
        public async void GetAllProjects_should_return_ArgumentNulleExcetionWithCollectionOfNullProjects()
        {
            List<Project> projects= null;
            _taskManagerRepositoryMock.Setup(p => p.GetProjectsAsync()).ReturnsAsync(projects);
            var exception = await  Assert.ThrowsAsync<ArgumentNullException>(
                () => _taskManagerService.GetProjects());
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async void GetAllProjects_should_return_ProperCollectionOfProjects()
        {
            List<Project> projects = _fixture.Create<List<Project>>();
            _taskManagerRepositoryMock.Setup(p => p.GetProjectsAsync()).ReturnsAsync(projects);
            List<Project> projectsReturned = await _taskManagerService.GetProjects();
            Assert.True(projectsReturned.Any()); 
        }

        [Fact]
        public async void UpdateProject_should_return_true_If_UpdatedIsSucceeded()
        {
            Project project = _fixture.Create<Project>();
            _taskManagerRepositoryMock.Setup(p => p.UpdateProjectAsync(It.IsAny<Project>())).ReturnsAsync(true);
            bool isUpdated = await _taskManagerService.UpdateAsync(project);
            Assert.True(isUpdated);
        }

    }
}