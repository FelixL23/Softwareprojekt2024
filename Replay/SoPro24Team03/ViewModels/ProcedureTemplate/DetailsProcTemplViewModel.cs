using System.ComponentModel.DataAnnotations;
using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels;

public class DetailsProcTemplViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
    public String? Name { get; set; }

    [Display(Name = "Aufgaben")]
    public List<TaskTemplate>? TaskTemplates {get; set;}

    [Display(Name = "")]
    public List<Role> Roles {get; set;}

    [Display(Name = "Archiviert")]
    public Boolean? IsArchived { get; set; }
}