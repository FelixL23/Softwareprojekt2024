using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoPro24Team03.Models;
using System.ComponentModel.DataAnnotations;

namespace SoPro24Team03.ViewModels
{
    public class CreateRoleViewModel
    {
        [Display(Name = "Name")]
        public String? Name {get; set;}

        [Display(Name = "Berechtigungen")]

        public bool? IsAdmin { get; set; }

        public List<User> Users {get; set;} = new List<User>();

        public List<User>? AllUsers {get; set;}
        public List<int> SelectedUsers { get; set; } = new List<int>();
    }
}