using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace SoPro24Team03.Models
{
    /// <summary>
    /// Note a user can add to a TaskInst.
    /// </summary>
    public class Note
    {
        /// <value>Id of the note. Key for database</value>
        [Display(Name = "Id")]
        [Range(0, int.MaxValue, ErrorMessage = "Fehler! Die Id darf nicht negativ sein.")]
        public int Id {get; set;}

        /// <value>Date when the note was created.</value>
        [Display(Name = "Erstelldatum")]
        [Required(ErrorMessage = "Fehler! Das Erstelldatum muss angegeben werden.")]
        public DateTime CreationDate {get; set;}

        /// <value>Is the note visible to users other than the author.</value>
        [Display(Name = "Sichtbar für andere")]
        public bool VisibleToOthers {get; set;}

        /// <value>Title of the note.</value>
        [Display(Name = "Titel")]
        [Required(ErrorMessage = "Fehler! Ein Text muss angegeben werden.")]
        public String Title {get; set;} = null!;

        /// <value>Main text content of the note.</value>
        [Display(Name = "Text")]
        [Required(ErrorMessage = "Fehler! Ein Text muss angegeben werden.")]
        public String Text {get; set;} = null!;

        /// <value>Id of the user, who created the note.</value>
        public int AuthorId {get; set;}

        /// <value>User, who created the note.</value>
        [JsonIgnore]
        public User Author {get; set;} = null!;

        /// <value>Id of the task, the note belongs to.</value>
        public int TaskInstId {get; set;}

        /// <value>TaskInst, the note belongs to.</value>
        [JsonIgnore]
        public TaskInst TaskInst {get; set;} = null!;

        /// <summary>
        /// Determines wether a Note can be access by users
        /// other than the author.
        /// </summary>
        /// <param name="UserId">Id of the user trying to access the note</param>
        /// <returns>
        /// True, if the note can be accessed by others; false, otherwise.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public bool IsVisible(int UserId)
        {
            if (!this.VisibleToOthers) {
                return this.AuthorId == UserId;
            }
            return true;
        }
    }
}