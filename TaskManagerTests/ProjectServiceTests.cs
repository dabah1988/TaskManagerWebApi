using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Core.Domain.RepositoryContracts;
using TaskManager.Core.DTO;
using TaskManager.Core.Services;
using TaskManager.Core.ServicesContract;
using AutoFixture;
using WebApiTaskManager.Core.Domain.Entities;
using Utilitaire;
using AutoFixture.AutoMoq;
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
            List<Project>? projects= null;
            _taskManagerRepositoryMock.Setup(p => p.GetProjectsAsync(1,5)).ReturnsAsync(projects);
            var exception = await  Assert.ThrowsAsync<ArgumentNullException>(
                () => _taskManagerService.GetProjects(1,10));
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async void GetAllProjects_should_return_ProperCollectionOfProjects()
        {
            List<Project> projects = _fixture.Create<List<Project>>();
            _taskManagerRepositoryMock.Setup(p => p.GetProjectsAsync(1,10)).ReturnsAsync(projects);
            List<Project> projectsReturned = await _taskManagerService.GetProjects(1,10);
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

        //fonction Search
        [Fact]
        public async void Search_by_projectName_should_return_listOfProperProjects_accordingName()
        {
            int pageNumber=1, pageSize=2;
            Project  project1 = _fixture.Build<Project>()
              .With(p => p.ProjectName, "doublure du patron")
              .With(p =>p.TeamSize, 100)
              .With(p => p.DateOfStart, DateTime.UtcNow)
              .With(p=>p.ProjectDescription, "Un livre qui montre comment être incontournable en entreprise")
              .Create();

            Project project2 = _fixture.Build<Project>()
        .With(p => p.ProjectName, "planifier sa carrière")
           .With(p=>p.TeamSize,1)
                   .With(p => p.DateOfStart, DateTime.UtcNow)
           .With(p=>p.ProjectDescription,"un livre où Il est question de bâtir son propre profil")
         .Create();

            Project project3 = _fixture.Build<Project>()
                .With( p => p.TeamSize,100)
                .With(p => p.ProjectDescription,"un livre où il est question de  courage")
                .Create();

            Project project4 = _fixture.Build<Project>()
                .With(p => p.TeamSize, 100)
                .With(p => p.ProjectName, "Expertise technique")
                .With(p => p.ProjectDescription, "un livre où il est question d'expertise technique")
                .Create();

            List<Project> projects = new List<Project>() { project1,project2,project3,project4};

            _taskManagerRepositoryMock.
                Setup(p => p.SearchProjectsAsync(pageNumber,pageSize,CriteriaOfSearch.ProjectName, "doublure "))
                .ReturnsAsync(new List<Project>() { project1});

            _taskManagerRepositoryMock.
             Setup(p => p.SearchProjectsAsync(pageNumber, pageSize, CriteriaOfSearch.projectDescription, "livre"))
             .ReturnsAsync(projects);

            List<Project> projectNameDoublure =
       await _taskManagerService.SearchProjectsAsync(pageNumber, pageSize, CriteriaOfSearch.ProjectName, "doublure");
          foreach(var myProject  in projectNameDoublure)
            {
                Assert.True(myProject.ProjectName == project1.ProjectName);
            }

            List<Project> projectDescriptionForLivres =
                await _taskManagerService.SearchProjectsAsync(pageNumber, pageSize, CriteriaOfSearch.projectDescription, "livre");
            Assert.True(projectDescriptionForLivres.Count == projects.Count);

        }

    }
}