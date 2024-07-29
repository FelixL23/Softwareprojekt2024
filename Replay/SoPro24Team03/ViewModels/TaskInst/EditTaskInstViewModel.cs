using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Models;


namespace SoPro24Team03.ViewModels
{
    public class EditTaskInstViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
        public String? Name {get; set;}

        [Display(Name = "Zieldatum")]
        public DateTime TargetDate {get; set;}

        [Display(Name = "Status")]
        public CompletionStatus Status {get; set;}


        [Display(Name = "Typ")]
        public TaskRespType TaskRespType {get; set;}

        [Display(Name = "Rolle")]
        public int? RespRoleId {get; set;}

        [Display(Name = "Benutzer")]
        public int? RespUserId {get; set;}


        // display only
        public int? PermissionLevel { get; set; }
        public int? UserId { get; set; }
        public int? TaskInstId { get; set; }
        public List<Note>? Notes { get; set; }
        public String? Instruction { get; set; }
        public List<SelectListItem>? AllUsers {get; set;}
        public List<SelectListItem>? AllRoles {get; set;}
    }
}
