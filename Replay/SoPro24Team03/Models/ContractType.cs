using System.ComponentModel.DataAnnotations;

namespace SoPro24Team03.Models
{
    /// <summary>
    /// Type of an employee contract.
    /// </summary>
    public enum ContractType
    {
        [Display(Name = "Festanstellung")]
        permanent,
        
        [Display(Name = "Werkstudent")]
        werkstudent,

        [Display(Name = "Praktikum")]
        internship,

        [Display(Name = "Trainee")]
        trainee
    }
}