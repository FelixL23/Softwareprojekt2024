using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoPro24Team03.Models;

public class TaskResponsible
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Die Id darf nicht negativ sein!")]
    public int Id { get; set; }

    [Display(Name = "Rolle")]
    [JsonIgnore]
    public Role? Role { get; set; } = null;
    public int? RoleId { get; set; } = null;

    [Display(Name = "Benutzer")]
    [JsonIgnore]
    public User? User { get; set; } = null;
    public int? UserId { get; set; } = null;

    public TaskRespType TaskRespType { get; set; }


    public TaskResponsible() { }
    public TaskResponsible(TaskRespType type, int? RoleId = null, int? ReferId = null, int? RespId = null, int? UserId = null)
    {
        this.Update(type, RoleId, ReferId, RespId, UserId);
    }

    /// <summary>
    /// Update this TaskResponsible while maintaining consistency.
    /// </summary>
    /// <param name="type">The new TaskRespType. Required.</param>
    /// <param name="RoleId">The Id of the role responsible for this task. Can be null.</param>
    /// <param name="ReferId">The Id of the associated Procedure's refered user. Can be null.</param>
    /// <param name="RespId">The Id of the associated Procedure's responsible user. Can be null.</param>
    /// <param name="UserId">The Id of the user responsible for this task. Can be null.</param>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public void Update(TaskRespType type, int? RoleId, int? ReferId, int? RespId, int? UserId)
    {
        switch (type)
        {
            case TaskRespType.role:
                if (RoleId == null)
                    break;
                this.TaskRespType = TaskRespType.role;
                this.RoleId = RoleId;
                this.UserId = null;
                return;

            case TaskRespType.proc_refer:
                if (ReferId == null) {
                    this.TaskRespType = TaskRespType.proc_refer;
                    this.RoleId = null;
                    this.UserId = null;
                }
                else {
                    if (ReferId == null)
                        break;
                    this.TaskRespType = TaskRespType.user;
                    this.RoleId = null;
                    this.UserId = ReferId;
                }
                return;

            case TaskRespType.proc_resp:
                if (RespId == null) {
                    this.TaskRespType = TaskRespType.proc_resp;
                    this.RoleId = null;
                    this.UserId = null;
                }
                else {
                    if (RespId == null)
                        break;
                    this.TaskRespType = TaskRespType.user;
                    this.RoleId = null;
                    this.UserId = RespId;
                }
                return;
            
            default:
                if (UserId == null)
                    break;
                this.TaskRespType = TaskRespType.user;
                this.RoleId = null;
                this.UserId = UserId;
                return;
        }
        throw new ArgumentException("Inconsistent parameters for TaskResponsible");
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
    public String DisplayName()
    {
        switch (this.TaskRespType) {
            case TaskRespType.proc_resp:
                return "Vorgangsverantwortlicher";

            case TaskRespType.proc_refer:
                return "Bezugsperson";

            case TaskRespType.role:
                if (this.Role != null)
                    return "Rolle: " + this.Role.Name;
                break;

            case TaskRespType.user:
                if (this.User != null)
                    return this.User.FirstName + " " + this.User.LastName;
                break;
        }
        return "";
    }
}