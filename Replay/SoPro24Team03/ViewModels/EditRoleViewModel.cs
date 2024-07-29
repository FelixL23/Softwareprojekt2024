using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SoPro24Team03.Data;
using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels
{
    public class EditRoleViewModel
    {
        [Display(Name = "Name")]
        public String? Name {get; set;}

        public bool? IsAdmin { get; set; }
        [Display(Name = "Benutzer")]
        public List<User>? Users {get; set;}
    }
}