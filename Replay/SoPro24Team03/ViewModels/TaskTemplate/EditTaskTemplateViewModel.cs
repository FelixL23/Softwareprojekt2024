using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels;

public class EditTaskTemplateViewModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name darf nicht leer sein!")]
    [DataType(DataType.Text)]
    public String? Name { get; set; }

    [Display(Name = "Zieldatum")]
    [Required(ErrorMessage = "Es muss ein Zieldatum angegeben werden!")]
    public DueType DueType { get; set; }
    public int? CustomDays {get; set;} = 0;

    public TaskRespType TaskResponsibleType {get; set;}
    public int TaskResponsibleRoleId {get; set;}

    [Display(Name = "Vertragsart")]
    public List<ContractType>? ContractTypes {get; set;} = new List<ContractType>();

    [Display(Name = "Anleitung")]
    public String? Instruction {get; set;}
    [Display(Name = "Abteilungen")]
    public List<int>? SelectedDepartmentIds {get; set;} = new List<int>();

    public List<SelectListItem> Roles {get; set;} = new List<SelectListItem>();
    public List<SelectListItem> Departments {get; set;} = new List<SelectListItem>();
}
