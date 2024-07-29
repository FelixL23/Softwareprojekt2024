using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SoPro24Team03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SoPro24Team03.ViewModels;

public class CreateProcTemplViewModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
    public String? Name { get; set; }

    [Display(Name = "Archiviert")]
    public Boolean? IsArchived { get; set; } = false;

    public String SelectedTaskTemplates { get; set; }

    [Display(Name = "Aufgabenvorlagen")]
    public List<TaskTemplate> TaskTemplates { get; set; } = new List<TaskTemplate>();

    public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    public List<int> SelectedRoles { get; set; } = new List<int>();
}
