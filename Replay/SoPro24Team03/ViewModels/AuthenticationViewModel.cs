using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoPro24Team03.ViewModels
{
    public class AuthenticationViewModel
    {

        [Display(Name = "Benutzername")]
        [Required(ErrorMessage = "Bitte geben Sie Ihren Benutzernamen ein.")]
        public String userName { get; set; } = String.Empty;

        [Display(Name = "Passwort")]
        [Required(ErrorMessage = "Bitte geben Sie Ihr Passwort ein.")]
        public String inputPassword { get; set; } = String.Empty;

    }
}
