using Bogus;
using Microsoft.AspNetCore.Identity;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enum;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            if (context.Users.Any()) return; // already seeded

            var random = new Random();
            var faker = new Faker("en");

            // USERS
            var users = new List<User>();
            var usedUsernames = new HashSet<string>();

            for (int i = 0; i < 50; i++)
            {
                string username;
                do
                {
                    //var baseName = faker.Name.FirstName().Replace(" ", "").Replace(".", "").Replace("-", "").ToLower();
                    //var number = faker.Random.Number(1000, 9999); // 4-digit number
                    //username = $"{baseName}{number}";
                    username = $"demo{i + 1}";
                } while (!usedUsernames.Add(username)); // Ensures uniqueness

                var user = new User
                {
                    Username = username,
                    CreatedAtUtc = DateTime.UtcNow,
                };
                user.PasswordHash = passwordHasher.HashPassword(user, "demo@123");
                users.Add(user);
            }

            context.Users.AddRange(users);

            // WORKSPACES
            var workspaces = new List<Workspace>();
            var workspaceUsers = new List<WorkspaceUser>();
            var projects = new List<Project>();
            var projectUsers = new List<ProjectUser>();
            var tasks = new List<TaskItem>();
            var logs = new List<TaskTimeLog>();

            for (int i = 0; i < 5; i++)
            {
                var owner = users[i]; // Assign first 5 users as workspace owners

                var workspace = new Workspace
                {
                    Id = Guid.NewGuid(),
                    Name = faker.Company.CompanyName(),
                    InviteCode = faker.Random.AlphaNumeric(8),
                    CreatedById = owner.Id,
                    CreatedAtUtc = DateTime.UtcNow
                };
                workspaces.Add(workspace);

                // Add owner
                workspaceUsers.Add(new WorkspaceUser
                {
                    Id = Guid.NewGuid(),
                    WorkspaceId = workspace.Id,
                    UserId = owner.Id,
                    Role = WorkspaceRole.Owner,
                    CreatedAtUtc = DateTime.UtcNow
                });

                // Add random Admins and Members
                var otherMembers = users.Except(new[] { owner }).OrderBy(_ => Guid.NewGuid()).Take(random.Next(10, 20)).ToList();
                foreach (var member in otherMembers)
                {
                    workspaceUsers.Add(new WorkspaceUser
                    {
                        Id = Guid.NewGuid(),
                        WorkspaceId = workspace.Id,
                        UserId = member.Id,
                        Role = faker.PickRandom(new[] { WorkspaceRole.Admin, WorkspaceRole.Member }),
                        CreatedAtUtc = DateTime.UtcNow
                    });
                }

                var currentWorkspaceUsers = workspaceUsers.Where(wu => wu.WorkspaceId == workspace.Id).ToList();
                var eligibleProjectCreators = currentWorkspaceUsers.Where(wu => wu.Role == WorkspaceRole.Owner || wu.Role == WorkspaceRole.Admin).ToList();

                // PROJECTS
                var workspaceProjects = new Faker<Project>()
                    .RuleFor(p => p.Id, _ => Guid.NewGuid())
                    .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                    .RuleFor(p => p.WorkspaceId, _ => workspace.Id)
                    .RuleFor(p => p.CreatedById, f => f.PickRandom(eligibleProjectCreators).UserId)
                    .RuleFor(p => p.CreatedAtUtc, f => f.Date.Recent())
                    .Generate(random.Next(5, 10));

                projects.AddRange(workspaceProjects);

                foreach (var project in workspaceProjects)
                {
                    var creator = project.CreatedById;
                    projectUsers.Add(new ProjectUser
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = project.Id,
                        UserId = (Guid)creator,
                        Role = ProjectRole.Owner,
                        CreatedAtUtc = DateTime.UtcNow
                    });

                    var eligibleUsers = currentWorkspaceUsers.Where(u => u.UserId != creator).ToList();
                    var additionalProjectUsers = eligibleUsers.OrderBy(_ => Guid.NewGuid()).Take(random.Next(3, 10)).ToList();

                    foreach (var user in additionalProjectUsers)
                    {
                        projectUsers.Add(new ProjectUser
                        {
                            Id = Guid.NewGuid(),
                            ProjectId = project.Id,
                            UserId = user.UserId,
                            Role = faker.PickRandom(new[] { ProjectRole.Admin, ProjectRole.Member }),
                            CreatedAtUtc = DateTime.UtcNow
                        });
                    }

                    var projectMembers = projectUsers.Where(pu => pu.ProjectId == project.Id).ToList();
                    var taskCreators = projectMembers.Where(pu => pu.Role == ProjectRole.Owner || pu.Role == ProjectRole.Admin).ToList();
                    var taskAssignees = projectMembers.Select(pu => pu.UserId).Distinct().ToList();

                    var projectTasks = new Faker<TaskItem>()
                        .RuleFor(t => t.Id, _ => Guid.NewGuid())
                        .RuleFor(t => t.Title, f => f.Hacker.Verb() + " " + f.Commerce.Product())
                        .RuleFor(t => t.Description, f => f.Lorem.Sentence())
                        .RuleFor(t => t.ProjectId, _ => project.Id)
                        .RuleFor(t => t.Status, f => f.PickRandom<TaskItemStatus>())
                        .RuleFor(t => t.CreatedById, f => f.PickRandom(taskCreators).UserId)
                        .RuleFor(t => t.AssignedToId, f => f.PickRandom(taskAssignees))
                        .RuleFor(t => t.CreatedAtUtc, f => f.Date.Recent())
                        .Generate(random.Next(5, 10));

                    tasks.AddRange(projectTasks);

                    foreach (var task in projectTasks)
                    {
                        var taskLogs = new Faker<TaskTimeLog>()
                            .RuleFor(l => l.Id, _ => Guid.NewGuid())
                            .RuleFor(l => l.TaskItemId, _ => task.Id)
                            .RuleFor(l => l.UserId, _ => task.AssignedToId!)
                            .RuleFor(l => l.StartTime, f => f.Date.Recent())
                            .RuleFor(l => l.EndTime, (f, l) => l.StartTime.AddMinutes(f.Random.Int(30, 180)))
                            .Generate(3);

                        logs.AddRange(taskLogs);
                    }
                }
            }

            context.Workspaces.AddRange(workspaces);
            context.WorkspaceUsers.AddRange(workspaceUsers);
            context.Projects.AddRange(projects);
            context.ProjectUsers.AddRange(projectUsers);
            context.Tasks.AddRange(tasks);
            context.TaskTimeLogs.AddRange(logs);
            context.SaveChanges();
        }
    }
}
