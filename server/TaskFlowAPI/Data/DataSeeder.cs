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

            // USERS
            var faker = new Bogus.Faker("en");
            var users = new List<User>();

            for (int i = 0; i < 20; i++)
            {
                var user = new User
                {
                    Username = faker.Internet.UserName(),
                    CreatedAtUtc = DateTime.UtcNow,
                };

                var password = "Password123!"; // same for all demo users
                user.PasswordHash = passwordHasher.HashPassword(user, password);

                users.Add(user);
            }

            context.Users.AddRange(users);

            // WORKSPACES
            var workspaces = new Faker<Workspace>()
                .RuleFor(w => w.Id, f => Guid.NewGuid())
                .RuleFor(w => w.Name, f => $"{f.Company.CompanyName()}")
                .RuleFor(w => w.InviteCode, f => f.Random.AlphaNumeric(8))
                .RuleFor(w => w.CreatedById, f => f.PickRandom(users).Id)
                .RuleFor(w => w.CreatedAtUtc, f => f.Date.Recent())
                .Generate(5);

            context.Workspaces.AddRange(workspaces);

            var workspaceUsers = new List<WorkspaceUser>();
            var projects = new List<Project>();
            var projectUsers = new List<ProjectUser>();
            var tasks = new List<TaskItem>();
            var logs = new List<TaskTimeLog>();


            foreach (var workspace in workspaces)
            {
                workspaceUsers.Add(new WorkspaceUser
                {
                    Id = Guid.NewGuid(),
                    WorkspaceId = workspace.Id,
                    UserId = (Guid)workspace.CreatedById!,
                    Role = WorkspaceRole.Owner,
                    CreatedAtUtc = DateTime.UtcNow

                });

                var workspaceMembers = users.OrderBy(_ => Guid.NewGuid())
                                .Take(random.Next(5, 11))
                                .Where(u => u.Id != (Guid)workspace.CreatedById!)
                                .ToList();

                foreach (var member in workspaceMembers)
                {
                    workspaceUsers.Add(new WorkspaceUser
                    {
                        Id = Guid.NewGuid(),
                        WorkspaceId = workspace.Id,
                        UserId = member.Id,
                        Role = WorkspaceRole.Member,
                        CreatedAtUtc = DateTime.UtcNow

                    });
                }

                var workspaceProjects = new Faker<Project>()
                    .RuleFor(p => p.Id, f => Guid.NewGuid())
                    .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                    .RuleFor(p => p.WorkspaceId, _ => workspace.Id)
                    .RuleFor(p => p.CreatedById, f => f.PickRandom(users).Id)
                    .RuleFor(p => p.CreatedAtUtc, f => f.Date.Recent())
                    .Generate(10);

                projects.AddRange(workspaceProjects);

                foreach (var project in workspaceProjects)
                {
                    var members = users.OrderBy(_ => Guid.NewGuid())
                       .Take(random.Next(3, 8))
                       .Where(u => u.Id != (Guid)project.CreatedById!)
                       .ToList();


                    projectUsers.Add(new ProjectUser
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = project.Id,
                        UserId = (Guid)project.CreatedById!,
                        Role = ProjectRole.Owner,
                        CreatedAtUtc = DateTime.UtcNow
                    });

                    foreach (var member in members)
                    {
                        projectUsers.Add(new ProjectUser
                        {
                            Id = Guid.NewGuid(),
                            ProjectId = project.Id,
                            UserId = member.Id,
                            Role = ProjectRole.Member,
                            CreatedAtUtc = DateTime.UtcNow
                        });
                    }

                    var projectTasks = new Faker<TaskItem>()
                        .RuleFor(t => t.Id, f => Guid.NewGuid())
                        .RuleFor(t => t.Title, f => f.Hacker.Verb() + " " + f.Commerce.Product())
                        .RuleFor(t => t.Description, f => f.Lorem.Sentence())
                        .RuleFor(t => t.ProjectId, _ => project.Id)
                        .RuleFor(t => t.Status, f => f.PickRandom<TaskItemStatus>())
                        .RuleFor(t => t.CreatedById, f => f.PickRandom(members).Id)
                        .RuleFor(t => t.AssignedToId, f => f.PickRandom(members).Id)
                        .RuleFor(t => t.CreatedAtUtc, f => f.Date.Recent())
                        .Generate(6);

                    tasks.AddRange(projectTasks);

                    foreach (var task in projectTasks)
                    {
                        var taskLogs = new Faker<TaskTimeLog>()
                            .RuleFor(l => l.Id, f => Guid.NewGuid())
                            .RuleFor(l => l.TaskItemId, _ => task.Id)
                            .RuleFor(l => l.UserId, _ => task.AssignedToId ?? users[0].Id)
                            .RuleFor(l => l.StartTime, f => f.Date.Recent())
                            .RuleFor(l => l.EndTime, (f, l) => l.StartTime.AddMinutes(f.Random.Int(30, 120)))
                            .Generate(3);

                        logs.AddRange(taskLogs);
                    }
                }
            }

            context.WorkspaceUsers.AddRange(workspaceUsers);
            context.Projects.AddRange(projects);
            context.ProjectUsers.AddRange(projectUsers);
            context.Tasks.AddRange(tasks);
            context.TaskTimeLogs.AddRange(logs);
            context.SaveChanges();
        }
    }
}