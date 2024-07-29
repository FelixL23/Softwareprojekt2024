using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoPro24Team03.Models;

public class TaskTemplate
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Die Id darf nicht negativ sein!")]
    public int Id { get; set; }

    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name darf nicht leer sein!")]
    [DataType(DataType.Text)]
    public String? Name { get; set; }

    [Display(Name = "Archiviert")]
    [Required(ErrorMessage = "isArchived is required")]
    public bool IsArchived { get; set; } // = false

    [Display(Name = "Anleitung")]
    public String? Instruction { get; set; }

    [Display(Name = "Zieldatum")]
    [Required(ErrorMessage = "Es muss ein Zieldatum angegeben werden!")]
    public DueType DueType { get; set; }
    [Display(Name ="Beliebige Tage")]
    public int? CustomDays {get; set;} = 0;

    [Display(Name = "Aufgabenzuständig")]
    public TaskResponsible TaskResponsible { get; set; }
    public int TaskResponsibleId { get; set; }

    [Display(Name = "Abteilungen")]
    [Required]
    public List<Department> Departments { get; set; } = new List<Department>();

    [Display(Name = "Aufgaben")]
    [JsonIgnore]
    public List<TaskInst> TaskInstances { get; set; } = new List<TaskInst>();

    [Display(Name = "Vertragsart")]
    [Required]
    public List<ContractType> ContractTypes { get; set; } = new List<ContractType>();

    [JsonIgnore]
    public List<ProcedureTemplate> ProcedureTemplates { get; set; } = new List<ProcedureTemplate>();

    /// <summary>
    /// Creates a new TaskInst from this TaskTemplate.
    /// </summary>
    /// <param name="Procedure">Procedure the new task should belong to.</param>
    /// <returns>
    /// The newly created Procedure.
    /// </returns>
    /// <remarks>
    /// Made by Fabio Tomic, Daniel Albert
    /// </remarks>
    public TaskInst CreateTaskInstance(Procedure procedure)
    {
        DateTime targetDate = this.DueType switch
        {
            DueType.asap => new DateTime(),
            DueType.at_start => procedure.TargetDate.AddDays(0),
            DueType.before_2weeks => procedure.TargetDate.AddDays(-14),
            DueType.after_3weeks => procedure.TargetDate.AddDays(21),
            DueType.after_3months => procedure.TargetDate.AddMonths(3),
            DueType.after_6months => procedure.TargetDate.AddMonths(6),
            DueType.custom => procedure.TargetDate.AddDays(CustomDays != null ? this.CustomDays.Value : 0),
            _ => new DateTime()
        };

        var taskInst = new TaskInst() {
            Name = this.Name,
            ProcedureId = procedure.Id,
            Resp = new TaskResponsible(
                this.TaskResponsible.TaskRespType,
                this.TaskResponsible.RoleId,
                procedure.ReferId,
                procedure.RespId,
                null),
            TemplateId = this.Id,
            TargetDate = targetDate
        };

        return taskInst;
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
        string res = "TaskTemplate(";
        res += $"Id={this.Id}, Name={this.Name}, IsArchived={this.IsArchived}, Departments=[";
        foreach (var dep in this.Departments) {
            res += dep.Id + ",";
        }
        res += "], ";
        res += $"RespType={this.TaskResponsible.TaskRespType}, Resp.UserId={this.TaskResponsible.UserId}, Resp.RoleId={this.TaskResponsible.RoleId}";
        res += ")";
        return res;
    }
}