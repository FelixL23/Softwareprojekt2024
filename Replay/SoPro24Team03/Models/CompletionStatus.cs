using System.ComponentModel.DataAnnotations;

namespace SoPro24Team03.Models
{
    /// <summary>
    /// Status to track the completion of a TaskInst.
    /// </summary>
    public enum CompletionStatus
    {
        [Display(Name = "Ausstehend")]
        backlog,

        [Display(Name = "In Bearbeitung")]
        in_progress,

        [Display(Name = "Erledigt")]
        done,

        [Display(Name = "Abgebrochen")]
        aborted
    }

}
