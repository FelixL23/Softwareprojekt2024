using System.ComponentModel.DataAnnotations;

namespace SoPro24Team03.Models;
public enum DueType
{
    [Display(Name = "So schnell wie möglich")]
    asap,
    [Display(Name = "vor 2 Wochen")]
    before_2weeks,
    [Display(Name = "Beim Start")]
    at_start,
    [Display(Name = "Nach 3 Wochen")]
    after_3weeks,
    [Display(Name = "Nach 3 Monaten")]
    after_3months,
    [Display(Name = "Nach 6 Monaten")]
    after_6months,
    [Display(Name = "Beliebig")]
    custom
}