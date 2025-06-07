using System.ComponentModel;

namespace TaskFlowAPI.Models.Enum
{
    public enum WorkspacePermission
    {
        [Description("Manage Workspace")]
        ManageWorkspace,

        [Description("View Workspace")]
        ViewWorkspace,

        [Description("Delete Workspace")]
        DeleteWorkspace
    }

    public enum ProjectPermission
    {
        [Description("Manage Project")]
        ManageProject,

        [Description("View Project")]
        ViewProject,

        [Description("Delete Project")]
        DeleteProject
    }

    public enum TaskPermission
    {
        [Description("Manage Task")]
        ManageTask,

        [Description("View Task")]
        ViewTask,

        [Description("Delete Task")]
        DeleteTask,

        [Description("Update Task Status")]
        UpdateTaskStatus,

        [Description("Log Time")]
        LogTime
    }
}
