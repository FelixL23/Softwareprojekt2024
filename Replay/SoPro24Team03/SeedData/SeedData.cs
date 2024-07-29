using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SoPro24Team03.Data;

namespace SoPro24Team03.Models
{
    public class SeedData
    {
        public static void initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ReplayContext())
            {
                Console.WriteLine($"initializing database. path: {context.DbPath}.");
                string outputDir = Path.GetFullPath("./persistence");
                string inputDir  = Path.GetFullPath("./SeedData/inputJSON");

                // create or update database if necessary
                context.Database.Migrate();

                // is database already seeded?
                if (context.Departments.Any())
                {
                    ExportJSON(outputDir);
                    printTables(context);
                    return;
                }

                if (File.Exists(Path.Join(inputDir, "Department.json")))
                    seedFromJSON(inputDir);
                else
                    seedManually(context);
                
                printTables(context);
            }
        }

        public static void ExportJSON(string outputDir)
        {
            Console.WriteLine($"exporting database to JSON-files. path: {outputDir}");
            var options = new JsonSerializerOptions() {
                WriteIndented = true
            };

            using (var repo = new DepartmentRepository()) {
                string text = repo.ExportToJSON(options);
                File.WriteAllText(Path.Join(outputDir, nameof(Department) + ".json"), text);
            }
            using (var repo = new RoleRepository()) {
                string text = repo.ExportToJSON(options);
                File.WriteAllText(Path.Join(outputDir, nameof(Role) + ".json"), text);
            }
            using (var repo = new ProcedureRepository()) {      // also includes TaskInsts, Notes, TaskResponsible
                string text = repo.ExportToJSON(options);
                File.WriteAllText(Path.Join(outputDir, nameof(Procedure) + ".json"), text);
            }
            using (var repo = new TaskTemplateRepository()) {   // also includes TaskResponsible
                string text = repo.ExportToJSON(options);
                File.WriteAllText(Path.Join(outputDir, nameof(TaskTemplate) + ".json"), text);
            }
            using (var repo = new ProcedureTemplateRepository()) {
                string text = repo.ExportToJSON(options);
                File.WriteAllText(Path.Join(outputDir, nameof(ProcedureTemplate) + ".json"), text);
            }
            using (var repo = new UserRepository()) {
                string text = repo.ExportToJSON(options);
                File.WriteAllText(Path.Join(outputDir, nameof(User) + ".json"), text);
            }
        }

        public static void printTables(ReplayContext context)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("DEBUG: printing tables in database");

            Console.WriteLine("Departments:");
            context.Departments.ToList().ForEach(e => Console.WriteLine("  " + e.ToString()));

            Console.WriteLine("Roles:");
            context.Roles.ToList().ForEach(e => Console.WriteLine("  " + e.ToString()));

            Console.WriteLine("TaskInsts:");
            context.TaskInsts.Include(t => t.Resp).ToList().ForEach(e => Console.WriteLine("  " + e.ToString()));

            Console.WriteLine("Procedures:");
            context.Procedures.ToList().ForEach(e => Console.WriteLine("  " + e.ToString()));

            Console.WriteLine("TaskTemplates:");
            context.TaskTemplates
                .Include(tt => tt.TaskResponsible)
                .Include(tt => tt.ProcedureTemplates)
                .Include(tt => tt.Departments)
                .ToList()
                .ForEach(tt => Console.WriteLine("  " + tt.ToString()));

            Console.WriteLine("ProcedureTemplates:");
            context.ProcedureTemplates
                .Include(pt => pt.Roles)
                .Include(pt => pt.TaskTemplates)
                .ToList()
                .ForEach(pt => Console.WriteLine("  " + pt.ToString()));

            Console.WriteLine("Users:");
            context.Users.Include(u => u.Roles).ToList().ForEach(e => Console.WriteLine("  " + e.ToString()));

            Console.WriteLine("----------------------------------------");
        }

        public static void seedFromJSON(string inputDir)
        {
            Console.WriteLine($"seeding database with JSON-files. path: {inputDir}");

            using (var repo = new DepartmentRepository()) {
                string text = File.ReadAllText(Path.Join(inputDir, nameof(Department) + ".json"));
                repo.ImportFromJSON(text);
            }
            using (var repo = new RoleRepository()) {
                string text = File.ReadAllText(Path.Join(inputDir, nameof(Role) + ".json"));
                repo.ImportFromJSON(text);
            }
            using (var repo = new UserRepository()) {
                string text = File.ReadAllText(Path.Join(inputDir, nameof(User) + ".json"));
                repo.ImportFromJSON(text);
            }
            using (var repo = new TaskTemplateRepository()) {
                string text = File.ReadAllText(Path.Join(inputDir, nameof(TaskTemplate) + ".json"));
                repo.ImportFromJSON(text);
            }
            using (var repo = new ProcedureTemplateRepository()) {
                string text = File.ReadAllText(Path.Join(inputDir, nameof(ProcedureTemplate) + ".json"));
                repo.ImportFromJSON(text);
            }
            using (var repo = new ProcedureRepository()) {
                string text = File.ReadAllText(Path.Join(inputDir, nameof(Procedure) + ".json"));
                repo.ImportFromJSON(text);
            }
        }

        public static void seedManually(ReplayContext context)
        {
            Console.WriteLine("seeding database manually.");

            var deps = new List<Department>() {
                new Department() {Name = "Entwicklung"},
                new Department() {Name = "Operations"},
                new Department() {Name = "UI/UX"},
                new Department() {Name = "Projektmanagement"},
                new Department() {Name = "Backoffice"},
                new Department() {Name = "People & Culture"},
                new Department() {Name = "Sales"}
            };
            context.Departments.AddRange(deps);

            var roleAdmin = new Role() { Name = "Administratoren", IsAdmin = true };
            var roleIt = new Role() { Name = "IT" };
            var roleBackoffice = new Role() { Name = "Backoffice" };
            var roleCeo = new Role() { Name = "Geschäftsleitung" };
            var rolePersonal = new Role() { Name = "Personal" };

            var roles = new List<Role>() {
                roleAdmin,
                roleIt,
                roleBackoffice,
                roleCeo,
                rolePersonal
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();


            var user1 = new User()
            {
                Id = 3,
                UserName = "mustmax",
                FirstName = "Maximilian",
                LastName = "Musterman",
                DateOfBirth = new DateTime(2000, 1, 1),
                EmailAddress = "max.musterman@xyz.de",
                ContractType = ContractType.werkstudent,
                PasswordHash = Models.User.HashPassword("Passwort"),
                Roles = new List<Role> {roleAdmin},
                DepartmentId = 1,
                isArchived = false,
                isSuspended = false,
                changedInitialPassword = true
            };
            var user2 = new User()
            {
                Id = 5,
                UserName = "karl",
                FirstName = "heinz",
                LastName = "karl",
                DateOfBirth = new DateTime(2000, 1, 1),
                EmailAddress = "max.musterman@xyz.de",
                ContractType = ContractType.werkstudent,
                PasswordHash = "$2a$11$AJI7rpL/PXZ3yuy5M4OFRuB.iAV5zfRwXupdfDFSe7BdR/cqAccTa",
                Roles = new List<Role> { roleBackoffice },
                DepartmentId = 2,
                isArchived = true,
                isSuspended = false,
                changedInitialPassword = true
            };
            var user3 = new User()
            {
                Id = 4,
                UserName = "woodwoo",
                FirstName = "Woody",
                LastName = "Woodpecker",
                DateOfBirth = new DateTime(2000, 1, 1),
                EmailAddress = "woody.woodpecker@xyz.de",
                ContractType = ContractType.werkstudent,
                PasswordHash = Models.User.HashPassword("Test"),
                Roles = new List<Role> { rolePersonal },
                DepartmentId = 3,
                isArchived = false,
                isSuspended = false,
                changedInitialPassword = true
            };
            context.Users.Add(user1);
            context.Users.Add(user2);
            context.Users.Add(user3);
            context.SaveChanges();


            var mytasktemp = new TaskTemplate()
            {
                Name = "Create Accounts",
                DueType = DueType.at_start,
                ContractTypes = new List<ContractType>() {
                    ContractType.trainee
                },
                TaskResponsible = new TaskResponsible()
                {
                    TaskRespType = TaskRespType.role,
                    Role = roleIt
                },
                Instruction = "",
            };
            var myproctemp = new ProcedureTemplate()
            {
                Name = "Onboarding",
                TaskTemplates = new List<TaskTemplate>() { mytasktemp },
                Roles = new List<Role>()
                {
                    roleAdmin,
                    roleCeo,
                    rolePersonal
                },
                OrderedTaskTemplateIds = "1"
            };
            context.ProcedureTemplates.Add(myproctemp);
            context.SaveChanges();


            var mytask = new TaskInst()
            {
                Name = "SSH-Key erstellen",
                TargetDate = DateTime.Parse("2024-06-30T22:56:30"),
                Status = CompletionStatus.backlog,
                Template = mytasktemp,
                Resp = new TaskResponsible()
                {
                    TaskRespType = TaskRespType.role,
                    RoleId = 2
                }
            };
            var mytask2 = new TaskInst()
            {
                Name = "Create Acounts",
                TargetDate = DateTime.Parse("2024-06-16T22:56:30"),
                Status = CompletionStatus.done,
                Template = mytasktemp,
                Resp = new TaskResponsible()
                {
                    TaskRespType = TaskRespType.role,
                    RoleId = 2
                }
            };

            var myproc = new Procedure()
            {
                Name = "On-Boarding",
                TargetDate = DateTime.Parse("2024-06-25T08:32:10"),
                IsArchived = false,
                NumTasksDone = 0,
                NumTasksTotal = 1,
                CreatedContractType = ContractType.werkstudent,
                Template = myproctemp,
                Resp = user1,
                Refer = user3,
                FutureDepartmentId = 1
            };

            var myproc2 = new Procedure()
            {
                Name = "Neuzugang HR",
                TargetDate = DateTime.Parse("2024-06-20"),
                IsArchived = false,
                NumTasksDone = 1,
                NumTasksTotal = 2,
                CreatedContractType = ContractType.permanent,
                Template = myproctemp,
                Resp = user1,
                Refer = user2,
                FutureDepartmentId = 2
            };

            myproc.TaskInsts.Add(mytask);
            myproc2.TaskInsts.Add(mytask);
            myproc2.TaskInsts.Add(mytask2);

            context.Procedures.Add(myproc);
            context.Procedures.Add(myproc2);
            context.SaveChanges();
        }
    }
}
