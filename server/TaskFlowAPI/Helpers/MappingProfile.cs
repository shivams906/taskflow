using AutoMapper;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enum;

namespace TaskFlow.API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Workspace, WorkspaceDto>()
                .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src =>
                    src.CreatedBy.Username))
                .ForMember(dest => dest.UpdatedByUsername, opt => opt.MapFrom(src =>
                    src.UpdatedBy.Username));
            CreateMap<CreateWorkspaceDto, Workspace>();

            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src =>
                    src.CreatedBy.Username))
                .ForMember(dest => dest.UpdatedByUsername, opt => opt.MapFrom(src =>
                    src.UpdatedBy.Username));

            CreateMap<CreateProjectDto, Project>();

            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src =>
                    src.CreatedBy.Username))
                .ForMember(dest => dest.UpdatedByUsername, opt => opt.MapFrom(src =>
                    src.UpdatedBy.Username));

            CreateMap<CreateTaskDto, TaskItem>();

            CreateMap<TaskTimeLog, TimeLogDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

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

            CreateMap<ChangeLog, ChangeLogDto>()
                .ForMember(dest => dest.ChangedByUserName, opt => opt.MapFrom(src => src.ChangedByUser.Username));
        }
    }
}
