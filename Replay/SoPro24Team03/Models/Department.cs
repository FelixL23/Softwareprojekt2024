using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoPro24Team03.Models;

public class Department
{
    [Required]
    public int Id {get; set;}

    [Display(Name = "Name")]
    public String? Name {get; set;}

    [JsonIgnore]
    public List<TaskTemplate> TaskTemplates { get; set; } = new List<TaskTemplate>();

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
        return $"Department(Id={this.Id}, Name={this.Name})";
    }
}