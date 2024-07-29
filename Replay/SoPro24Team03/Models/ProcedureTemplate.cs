using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoPro24Team03.Models;

public class ProcedureTemplate
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Fehler! Die Id darf nicht negativ sein.")]
    public int Id { get; set; }

    [Display(Name = "Name")]
    [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
    public String? Name { get; set; }

    [Display(Name = "Archiviert")]
    public Boolean IsArchived { get; set; } = false;

    [Display(Name = "Aufgaben")]
    public List<TaskTemplate> TaskTemplates { get; set; } = new List<TaskTemplate>();
    public String OrderedTaskTemplateIds { get; set; } = "";

    [Display(Name = "Rollen")]
    public List<Role> Roles { get; set; } = new List<Role>();

    /// <summary>
    /// Creates a new Procedure from this ProcedureTemplate.
    /// </summary>
    /// <param name="Name">Name of the new procedure.</param>
    /// <param name="targetDate">Date when the new procedure is due.</param>
    /// <param name="RespId">Id of the user responsible for the new procedure.</param>
    /// <param name="ReferId">Id of the user that the new procedure refers to.</param>
    /// <param name="CreatedContractType">Type of the contract created by the new procedure. Can be null.</param>
    /// <param name="FutureDepertmentId">Department the refered user should join. Can be null.</param>
    /// <returns>
    /// The newly created Procedure.
    /// </returns>
    /// <remarks>
    /// Made by Felix, Daniel Albert
    /// </remarks>
    public Procedure CreateProcedure(
        string name, DateTime targetDate, int respId, int referId,
        ContractType? createdContractType, int? futureDepartmentId)
    {
        var newProcedure = new Procedure
        {
            Name = name,
            TargetDate = targetDate,
            CreatedContractType = createdContractType,
            OrderedTaskInstIds = "",
            TemplateId = this.Id,
            FutureDepartmentId = futureDepartmentId,
            ReferId = referId,
            RespId = respId
        };

        var orderedTaskTemplateIds = this.OrderedTaskTemplateIds.Split(",");
        var sortedTaskTemplates = orderedTaskTemplateIds
            .Select(idStr => 
            {
                var taskTemplate = this.TaskTemplates.FirstOrDefault(t => t.Id == int.Parse(idStr));
                if (taskTemplate == null)
                {
                    throw new InvalidOperationException($"TaskTemplate with Id {idStr} not found.");
                }
                return taskTemplate;
            });

        foreach (var taskTempl in sortedTaskTemplates)
        {
            if (taskTempl.Departments.Count() > 0) {    // no departments => no restriction
                if (futureDepartmentId == null)
                    continue;
                if (!taskTempl.Departments.Any(dep => dep.Id == futureDepartmentId.Value))
                    continue;
            }
            if (taskTempl.ContractTypes.Count() > 0) {  // no contract types => no restriction
                if (createdContractType == null)
                    continue;
                if(!taskTempl.ContractTypes.Any(ct => ct == createdContractType.Value))
                    continue;
            }

            var taskInst = taskTempl.CreateTaskInstance(newProcedure);
            newProcedure.TaskInsts.Add(taskInst);
            newProcedure.NumTasksTotal += 1;
            newProcedure.NumTasksDone += (taskInst.Status == CompletionStatus.done) ? 1 : 0;
        }

        return newProcedure;
    }


    /// <summary>
    /// Determines wether a User is authorized to access this ProcedureTemplate.
    /// </summary>
    /// <param name="user">The User trying to access this ProcedureTemplate.</param>
    /// <returns>
    /// True, if the user is authorized; false otherwise.
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public bool isUserAuthorized(User user)
    {
        Console.WriteLine(user.Roles.Any(r => r.IsAdmin));
        return user.Roles.Any(ur => ur.IsAdmin || this.Roles.Any(pr => pr.Id == ur.Id));
    }

    /// <summary>
    /// Generates a string representation for this object.
    /// </summary>
    /// <returns>
    /// The string representing this object.
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public override string ToString()
    {
        string res = "ProcedureTemplate(";
        res += $"Id={this.Id}, Name={this.Name}, IsArchived={this.IsArchived}, Roles=[";
        foreach (var role in this.Roles) {
            res += role.Id + ",";
        }
        res += "], TaskTemplates=[";
        foreach (var task in this.TaskTemplates) {
            res += task.Id + ",";
        }
        res += "])";
        return res;
    }
}

