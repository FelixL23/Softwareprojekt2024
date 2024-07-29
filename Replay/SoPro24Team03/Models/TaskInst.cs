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
    /// Class representing a task. Is part of a Procedure.
    /// </summary>
    public class TaskInst
    {
        /// <value>Id of the task. Key for database</value>
        [Display(Name = "Id")]
        [Range(0, int.MaxValue, ErrorMessage = "Fehler! Die Id darf nicht negativ sein.")]
        public int Id {get; set;}

        /// <value>Name of the task.</value>
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
        public String? Name {get; set;}

        /// <value>Date when the task is due.</value>
        [Display(Name = "Zieldatum")]
        public DateTime TargetDate {get; set;}
        
        /// <value>Is this task archived?</value>
        [Display(Name = "archiviert")]
        public bool IsArchived {get; set;} = false;

        /// <value>Status to track the completion of the task.</value>
        [Display(Name = "Status")]
        public CompletionStatus Status {get; set;} = CompletionStatus.backlog;

        /// <value>Notes users left for the task.</value>
        public List<Note> Notes {get; set;} = new List<Note>();

        /// <value>Id of procedure that the task belongs to.</value>
        public int ProcedureId {get; set;}

        /// <value>Procedure that the task belongs to.</value>
        [JsonIgnore]
        public Procedure Procedure {get; set;} = null!;

        /// <value>Id of the template this task was created from (optional).</value>
        public int? TemplateId {get; set;} = null;

        /// <value>TaskTemplate this task was created from (optional).</value>
        [JsonIgnore]
        public TaskTemplate? Template {get; set;} = null;

        /// <value>Id of the TaskResponsible object containing who is responsible for this task.</value>
        public int RespId {get; set;}

        /// <value>TaskResponsible object containing who is responsible for this task.</value>
        public TaskResponsible Resp {get; set;} = null!;

        /// <summary>
        /// Determines wether this TaskInst is overdue.
        /// </summary>
        /// <returns>
        /// True, if the task is not done and it's target date has passed; false otherwise.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public bool isOverdue()
        {
            if (this.Status != CompletionStatus.done) {
                return DateTime.Now >= this.TargetDate;
            }
            return false;
        }

        /// <summary>
        /// Determines a User's permission level for this task:
        /// 0: access denied,
        /// 1: can edit Status, Resp, Notes (task responsible, procedure responsible, admin),
        /// 2: can also edit TargetDate, create/delete TaskInst (procedure responsible, admin),
        /// 3: can also edit Name (admin)
        /// </summary>
        /// <param name="user">The user to test permissions for.</param>
        /// <returns>
        /// The permission level.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public int getPermissionLevel(User user)
        {
            if (user.Roles.Any(r => r.IsAdmin))
                return 3;
            
            if (this.Procedure.RespId == user.Id)
                return 2;
            
            if (this.Resp.TaskRespType == TaskRespType.user) {
                if (this.Resp.UserId == user.Id)
                    return 1;
            }
            else if (this.Resp.TaskRespType == TaskRespType.role) {
                if (user.Roles.Any(r => r.Id == this.Resp.RoleId))
                    return 1;
            }
            return 0;
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
            string res = "TaskInst(";
            res += $"Id={this.Id}, Name={this.Name}, IsArchived={this.IsArchived}, TemplateId={this.TemplateId}, ";
            res += $"RespType={this.Resp.TaskRespType}, Resp.UserId={this.Resp.UserId}, Resp.RoleId={this.Resp.RoleId})";
            return res;
        }
    }
}
