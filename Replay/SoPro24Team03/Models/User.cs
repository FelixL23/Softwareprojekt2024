using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BC = BCrypt.Net.BCrypt;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SoPro24Team03.Models;

public class User
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Die Id darf nicht negativ sein!")]
    public int Id { get; set; }

    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required")]
    [DataType(DataType.Text)]
    public string? UserName { get; set; }

    [Display(Name = "Firstname")]
    [Required(ErrorMessage = "FirstName is required")]
    [DataType(DataType.Text)]
    public string? FirstName { get; set; }

    [Display(Name = "Lastname")]
    [Required(ErrorMessage = "LastName is required")]
    [DataType(DataType.Text)]
    public string? LastName { get; set; }

    [Display(Name = "DateOfBirth")]
    [Required(ErrorMessage = "Date of Birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "ContractStart")]
    [Required(ErrorMessage = "Contract Start is required")]
    [DataType(DataType.Date)]
    public DateTime ContractStart { get; set; }

    [Display(Name = "ContractType")]
    [Required(ErrorMessage = "Contract Type is required")]
    public ContractType ContractType { get; set; }

    [Display(Name = "EmailAddress")]
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? EmailAddress { get; set; }

    [Display(Name = "PasswordHash")]
    [Required(ErrorMessage = "PasswordHash is required")]
    public string? PasswordHash { get; set; }

    [Display(Name = "Roles")]
    public List<Role>? Roles { get; set; } = new List<Role>();

    [Display(Name = "Department")]
    [Required(ErrorMessage = "User needs a department")]
    public int DepartmentId {get; set;}
    [JsonIgnore]
    public Department Department { get; set; } = null!;

    [Display(Name = "isArchived")]
    [Required(ErrorMessage = "isArchived is required")]
    public bool isArchived { get; set; } = false;

    [Display(Name = "isSuspended")]
    [Required(ErrorMessage = "isSuspended is required")]
    public bool isSuspended { get; set; } = false;

    public bool changedInitialPassword { get; set; } = false;

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Determines wether this User has administative priviledges.
    /// </summary>
    /// <returns>
    /// True, if the user is administrator; false otherwise.
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public bool isAdmin()
    {
        return this.Roles.Any(r => r.IsAdmin);
    }

    /// <summary>
    /// Validates a passoword for login.
    /// </summary>
    /// <param name="userName">The username.</param>
    /// <param name="inputPassword">The password.</param>
    /// <returns>
    /// True, if passwords match, False if not
    /// </returns>
    /// <remarks>
    /// Made by Daniel H
    /// </remarks>
    public bool ValidatePassword(string inputPassword)
    {
        var storedHash = this.PasswordHash;

        //validate hashed Password with BCrypt
        var res = BC.Verify(inputPassword, storedHash);

        return res;
    }

    /// <summary>
    /// Generates a user name from first and last name.
    /// </summary>
    /// <param name="firstName">the user's first name</param>
    /// <param name="lastName">the user's last name</param>
    /// <returns>
    /// a valid user name for logging into the system
    /// </returns>
    /// <remarks>
    /// Made by Daniel Habenicht, Daniel Albert
    /// </remarks>
    public static string GenerateUserName(string firstName, string lastName)
    {
        const int numCharsLast = 4;
        const int numCharsFirst = 3;

        if (String.IsNullOrEmpty(lastName) || String.IsNullOrEmpty(firstName)) {
            throw new ArgumentException("cannot generate user name from empty strings.");
        }
        
        string subLast = CleanupNameStr(lastName);
        subLast = subLast.Length > numCharsLast ? subLast.Substring(0, numCharsLast) : subLast;

        string subFirst = CleanupNameStr(firstName);
        subFirst = subFirst.Length > numCharsFirst ? subFirst.Substring(0, numCharsFirst) : subFirst;

        string res = subLast + subFirst;
        if (res.Length < 4) {
            throw new ArgumentException("user name invalid.");
        }

        return res;
    }

    /// <summary>
    /// helper function for GenerateUserName() 
    /// </summary>
    /// <param name="input">string that my contain Umlaute </param>
    /// <returns>
    /// input string without Umlaute
    /// </returns>
    /// <remarks>
    /// Made by Daniel Habenicht, Daniel Albert
    /// </remarks>
    private static string CleanupNameStr(string input)
    {
        // only lower case strings
        string res = input.ToLower();

        // replace umlauts
        res = res
            .Replace("ä", "ae")
            .Replace("ö", "oe")
            .Replace("ü", "ue");
        
        // remove whitespace, non alphabet characters
        res = Regex.Replace(res, @"[^a-zA-Z']", string.Empty);   

        return res;
    }

    /// <summary>
    /// Generates a cryptographic hash value of a given password.
    /// </summary>
    /// <param name="inputPassword">password to hash</param>
    /// <returns>
    /// hash of the password
    /// </returns>
    /// <remarks>
    /// Made by Daniel Habenicht
    /// </remarks>
    public static string HashPassword(string inputPassword)
    {
        // Use BCrypt to hash the password
        string hashedPassword = BC.HashPassword(inputPassword);
        return hashedPassword;
    }


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
        string res = "User(";
        res += $"Id={this.Id}, UserName={this.UserName}, IsArchived={this.isArchived}, IsSuspended={this.isSuspended}, Roles=[";
        foreach (var role in this.Roles) {
            res += role.Id + ",";
        }
        res += "])";
        return res;
    }

}