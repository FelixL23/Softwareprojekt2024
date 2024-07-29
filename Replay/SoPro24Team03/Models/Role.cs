using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoPro24Team03.Models
{

    public class Role
    {
        [Display(Name = "Id")]
        [Range(0, int.MaxValue, ErrorMessage = "Die Id darf nicht negativ sein!")]
        public int Id { get; set; }

        public String? Name { get; set; }

        public bool IsAdmin { get; set; } = false;

        [JsonIgnore]
        public List<User> users { get; set; } = new List<User>();

        [JsonIgnore]
        public List<ProcedureTemplate> ProcedureTemplates {get; set;} = new List<ProcedureTemplate>();
        
        [JsonIgnore]
        public List<TaskResponsible> TaskResponsibles {get; set;} = new List<TaskResponsible>();

        /// <summary>
        /// Generates a string representation for this object.
        /// </summary>
        /// <returns>
        /// The string representing this object.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public override string ToString()
        {
            return $"Role(Id={this.Id}, Name={this.Name}, IsAdmin={this.IsAdmin})";
        }
    }
}