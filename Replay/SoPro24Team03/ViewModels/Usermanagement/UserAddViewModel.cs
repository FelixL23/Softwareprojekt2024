using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using SoPro24Team03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace SoPro24Team03.ViewModels;
public class UserAddViewModel
{
    [Required(ErrorMessage = "Vorname ist erforderlich")]
    [Display(Name = "FirstName")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Nachname ist erforderlich")]
    [Display(Name = "LastName")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Passwort muss gesetzt sein!")]
    public String? Password {get; set;}

    [Required(ErrorMessage = "Geburtsdatum ist erforderlich")]
    [DataType(DataType.Date)]
    [Display(Name = "DateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Vertragsbeginn ist erforderlich")]
    [DataType(DataType.Date)]
    [Display(Name = "ContractStart")]
    public DateTime ContractStart { get; set; }

    [Required(ErrorMessage = "Vertragsart ist erforderlich")]
    [Display(Name = "ContractType")]
    public ContractType ContractType { get; set; }

    [Display(Name = "SelectedRoleIds")]
    public List<int>? SelectedRoleIds { get; set; } = new List<int>();

    [Required(ErrorMessage = "e-Mail ist erforderlich")]
    [EmailAddress(ErrorMessage = "Ungültige e-Mail-Adresse")]
    [Display(Name = "EmailAddress")]
    public string? EmailAddress { get; set; }

    [Display(Name = "isSuspended")]
    [Required(ErrorMessage = "isSuspended is required")]
    public bool isSuspended {get; set;} //= false

    [Required(ErrorMessage = "Abteilung ist erforderlich")]
    [Display(Name = "Department")]
    public int SelectedDepartmentId { get; set; }

    public List<SelectListItem>? AllRoles { get; set; }
    public List<SelectListItem>? AllDepartments {get; set;}
    public SelectList? ContractTypeList { get; set; }
}
