using AutoMapper;
using TaskManager.Core.DTO;
using WebApiTaskManager.Core.Domain.Entities;

namespace TaskManager.Core.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectResponse>();
        }
    }
}
