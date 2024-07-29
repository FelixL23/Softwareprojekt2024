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
    public class CreateProcedureViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
        public String? Name {get; set;}

        [Display(Name = "Zieldatum")]
        public DateTime TargetDate {get; set;}

        [Display(Name = "Vertragsart")]
        public ContractType? ContractType {get; set;} = null;

        [Display(Name = "Zuk. Abteilung")]
        public int? FutureDepartmentId {get; set;} = null;

        [Display(Name = "Verantwortlicher")]
        public int RespId {get; set;}

        [Display(Name = "Bezugsperson")]
        public int ReferId {get; set;}


        // display only
        public List<SelectListItem>? AllDepartments {get; set;}
        public List<SelectListItem>? AllUsers {get; set;}
    }
}
