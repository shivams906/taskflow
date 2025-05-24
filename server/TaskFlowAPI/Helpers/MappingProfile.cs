using AutoMapper;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enums;

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

            CreateMap<AddProjectUserDto, ProjectUser>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src =>
                    Enum.Parse<ProjectRole>(src.Role, true)));


            CreateMap<WorkspaceUser, WorkspaceUserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<ProjectUser, ProjectUserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        }
    }
}
