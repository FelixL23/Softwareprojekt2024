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
    public class NoteViewModel
    {
        [Display(Name = "Title")]
        [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
        public String Title {get; set;} = null!;

        [Display(Name = "Text")]
        public String Text {get; set;} = null!;

        [Display(Name = "sichtbar für andere")]
        public Boolean VisibleToOthers {get; set;}
    }
}
