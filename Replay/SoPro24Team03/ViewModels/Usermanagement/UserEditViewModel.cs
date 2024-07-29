using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SoPro24Team03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SoPro24Team03.ViewModels;
public class UserEditViewModel
{
    
    [Display(Name = "Username")]
    [DataType(DataType.Text)]
    public string? UserName { get; set; }

    [Display(Name = "FirstName")]
    public string? FirstName { get; set; }

    [Display(Name = "LastName")]
    public string? LastName { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Required(ErrorMessage = "Geburtsdatum ist erforderlich")]
    [DataType(DataType.Date)]
    [Display(Name = "DateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Required(ErrorMessage = "Vertragsbeginn ist erforderlich")]
    [DataType(DataType.Date)]
    [Display(Name = "ContractStart")]
    public DateTime ContractStart { get; set; }

    [Required(ErrorMessage = "Vertragsart ist erforderlich")]
    [Display(Name = "ContractType")]
    public ContractType ContractType { get; set; }
    public SelectList? ContractTypeList { get; set; }
    
    [Display(Name = "Roles")]
    public List<SelectListItem>? Roles { get; set; } = new List<SelectListItem>();

    [Display(Name = "SelectedRoleIds")]
    public List<int>? SelectedRoleIds {get; set;}

    [Required(ErrorMessage = "Abteilung ist erforderlich")]
    [Display(Name = "Department")]
    public int SelectedDepartmentId { get; set; }

    [Required(ErrorMessage = "e-Mail ist erforderlich")]
    [EmailAddress(ErrorMessage = "Ungültige e-Mail-Adresse")]
    [Display(Name = "EmailAddress")]
    public string? EmailAddress { get; set; }

    [Display(Name = "isSuspended")]
    [Required(ErrorMessage = "isSuspended is required")]
    public bool isSuspended {get; set;} //= false

    [Display(Name = "isArchived")]
    public bool isArchived {get; set;} //= false

    [Display(Name = "PasswordHash")]
    public string? PasswordHash { get; set; }


    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    public List<SelectListItem>? AllDepartments {get; set;}
}
