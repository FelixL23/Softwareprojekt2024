using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoPro24Team03.Models
{
    /// <summary>
    /// Class representing a procedure. Can contain TaskInsts.
    /// </summary>
    public class Procedure
    {
        /// <value>Id of the procedure. Key for database</value>
        [Display(Name = "Id")]
        [Range(0, int.MaxValue, ErrorMessage = "Fehler! Die Id darf nicht negativ sein.")]
        public int Id {get; set;}

        /// <value>Name of the procedure.</value>
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
        public String? Name {get; set;}

        /// <value>Date when the procedure is due.</value>
        [Display(Name = "Zieldatum")]
        public DateTime TargetDate {get; set;}

        /// <value>Is this procedure archived?</value>
        [Display(Name = "archiviert")]
        public bool IsArchived {get; set;} = false;

        /// <value>Number of tasks, which are marked as done.</value>
        [Display(Name = "erledigte Aufgaben")]
        public int NumTasksDone {get; set;} = 0;

        /// <value>Total number of tasks belonging to this procedure.</value>
        [Display(Name = "Aufgaben insgesamt")]
        public int NumTasksTotal {get; set;} = 0;

        /// <value>Type of contract created by this procedure (optional).</value>
        [Display(Name = "Zu erstellender Vertragstyp")]
        public ContractType? CreatedContractType {get; set;} = null;

        /// <value>TaskInsts belonging to this procedure.</value>
        public List<TaskInst> TaskInsts {get; set; } = new List<TaskInst>();

        /// <value>String representing the order of the tasks in this procedure.</value>
        public String OrderedTaskInstIds {get; set;} = "";

        /// <value>Id of the template this procedure was created from.</value>
        public int TemplateId {get; set;}

        /// <value>ProcedureTemplate this procedure was created from.</value>
        [JsonIgnore]
        public ProcedureTemplate Template {get; set;} = null!;

        /// <value>Id of the department the refered user should join (optional).</value>
        public int? FutureDepartmentId {get; set;} = null;

        /// <value>Department the refered user should join (optional).</value>
        [JsonIgnore]
        public Department? FutureDepartment {get; set;} = null;

        /// <value>Id of the user responsible for this procedure.</value>
        public int RespId {get; set;}

        /// <value>User responsible for this procedure.</value>
        [JsonIgnore]
        public User Resp {get; set;} = null!;

        /// <value>Id of the user refered to by this procedure.</value>
        public int ReferId {get; set;}

        /// <value>User refered to by this procedure.</value>
        [JsonIgnore]
        public User? Refer {get; set;} = null!;

        /// <summary>
        /// Determines wether this Procedure is overdue.
        /// </summary>
        /// <returns>
        /// True, if the procedure has unfinished tasks and it's target date has passed; false otherwise.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public bool IsOverdue()
        {
            if (this.NumTasksDone < this.NumTasksTotal) {
                return DateTime.Now >= this.TargetDate;
            }
            return false;
        }

        /// <summary>
        /// Reads all of this procedures tasks and 
        /// sets the number of done and total tasks 
        /// to the correct values.
        /// </summary>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public void UpdateNumTasks()
        {
            int newTotal = 0, newDone = 0;
            foreach (TaskInst task in this.TaskInsts) {
                newTotal++;
                if (task.Status == CompletionStatus.done)
                    newDone++;
            }
            this.NumTasksDone = newDone;
            this.NumTasksTotal = newTotal;
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
            string res = "Procedure(";
            res += $"Id={this.Id}, Name={this.Name}, IsArchived={this.IsArchived}, TemplateId={this.TemplateId}, ";
            res += $"RespId={this.RespId}, ReferId={this.ReferId})";
            return res;
        }
    }
}
