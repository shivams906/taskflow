using AutoMapper;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Models;

namespace TaskFlow.API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Workspace, WorkspaceDto>();
            CreateMap<CreateWorkspaceDto, Workspace>();

            CreateMap<Project, ProjectDto>();

            CreateMap<CreateProjectDto, Project>();

            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateTaskDto, TaskItem>();

            CreateMap<TaskTimeLog, TimeLogDto>();

            CreateMap<CreateTimeLogDto, TaskTimeLog>();

            CreateMap<User, UserDto>();

            CreateMap<AddProjectAdminDto, ProjectUser>();

        }
    }
}
