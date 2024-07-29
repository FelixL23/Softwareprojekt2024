using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SoPro24Team03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SoPro24Team03.ViewModels;

public class EditProcTemplViewModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
    public String? Name { get; set; }

    [Display(Name = "Aufgabenvorlagen")]
    public String SelectedTaskTemplateIds {get; set;} = "";

    [Display(Name = "Rollen")]
    public List<int> SelectedRoles {get; set;} = new List<int>();

    [Display(Name = "Archiviert")]
    public Boolean IsArchived { get; set; } = false;

    public List<SelectListItem> Roles {get; set;} = new List<SelectListItem>();
    public List<TaskTemplate> TaskTemplates {get; set;} = new List<TaskTemplate>();
}
