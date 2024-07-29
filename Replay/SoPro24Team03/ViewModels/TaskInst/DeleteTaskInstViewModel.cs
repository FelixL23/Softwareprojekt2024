using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels
{
    public class DeleteTaskInstViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Fehler! Der Name darf nicht leer sein.")]
        public String? Name {get; set;}

        [Display(Name = "Zieldatum")]
        public DateTime TargetDate {get; set;}

        [Display(Name = "Status")]
        public CompletionStatus Status {get; set;}

        [Display(Name = "Zuständiger")]
        public String RespStr {get; set;} = null!;
    }
}
