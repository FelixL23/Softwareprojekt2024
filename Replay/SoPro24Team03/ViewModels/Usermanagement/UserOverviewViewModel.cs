using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels;

public class UserOverviewViewModel
{
    [Required(ErrorMessage = "Vorname ist erforderlich")]
    [Display(Name = "LastName")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Nachname ist erforderlich")]
    [Display(Name = "LastName")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Geburtsdatum ist erforderlich")]
    [DataType(DataType.Date)]
    [Display(Name = "DateOfBirth")]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Vertragsbeginn ist erforderlich")]
    [DataType(DataType.Date)]
    [Display(Name = "ContractStart")]
    public DateOnly ContractStart { get; set; }

    [Required(ErrorMessage = "Vertragsart ist erforderlich")]
    [Display(Name = "ContractType")]
    public string? ContractType { get; set; }

    [Display(Name = "Roles")]
    [Required(ErrorMessage = "User needs at least one role")]
    public List<Role> Roles { get; set; } = new List<Role>();

    [Required(ErrorMessage = "Abteilung ist erforderlich")]
    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Required(ErrorMessage = "e-Mail ist erforderlich")]
    [EmailAddress(ErrorMessage = "Ungültige e-Mail-Adresse")]
    [Display(Name = "EmailAddress")]
    public string? EmailAddress { get; set; }

    [Display(Name = "isSuspended")]
    [Required(ErrorMessage = "isSuspended is required")]
    public bool isSuspended {get; set;} //= false
    public List<User>? Users { get; set; }
}
