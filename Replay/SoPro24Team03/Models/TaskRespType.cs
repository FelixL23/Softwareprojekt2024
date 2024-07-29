using System.ComponentModel.DataAnnotations;

namespace SoPro24Team03.Models;

public enum TaskRespType
{
    [Display(Name = "Vorgangsverantwortlicher")]
    proc_resp,

    [Display(Name = "Bezugsperson")]
    proc_refer,
    
    [Display(Name = "Rolle")]
    role,

    [Display(Name = "Benutzer")]
    user
}