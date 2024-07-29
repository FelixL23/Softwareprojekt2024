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
    public class EditProcedureViewModel
    {
        [Display(Name = "Id")]
        public int Id {get; set;}

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
        public String? Name {get; set;}

        [Display(Name = "Zieldatum")]
        public DateTime TargetDate {get; set;}

        [Display(Name = "Vertragsart")]
        public ContractType? ContractType {get; set;}

        [Display(Name = "Zuk. Abteilung")]
        public int? FutureDepartmentId {get; set;}

        [Display(Name = "Verantwortlicher")]
        public int RespId {get; set;}

        [Display(Name = "Bezugsperson")]
        public int ReferId {get; set;}

        public String OrderedTaskInstIds {get; set;} = "";


        // display only
        public bool isAdmin {get; set;} = false;
        public List<TaskInst>? Tasks {get; set;}
        public int NumTasksDone {get; set;}
        public int NumTasksTotal {get; set;}
        public int? TemplateId {get; set;}
        public List<SelectListItem>? AllDepartments {get; set;}
        public List<SelectListItem>? AllUsers {get; set;}
    }
}
