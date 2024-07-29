using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using Xunit;
using Xunit.Abstractions;
using SoPro24Team03.Models;
using System.Globalization;


namespace SoPro24Team03.E2ETests;


// Made by Daniel Albert
public class AddUserTest : TestBase
{

    // Made by Daniel Albert
    public AddUserTest(ITestOutputHelper output) : base(output)
    { }

    // Made by Daniel Albert
    private void enterUser(Models.User inputUser, string password)
    {
        // navigate to UserAdd menu
        _driver.Navigate().GoToUrl(url + "/Usermanagement/UserAdd");
        waitUntilLoaded();
        Assert.Equal("Benutzer hinzufügen - Replay", _driver.Title);

        // enter user's data
        _driver.FindElement(By.Id("FirstName")).SendKeys(inputUser.FirstName);
        _driver.FindElement(By.Id("LastName")).SendKeys(inputUser.LastName);
        _driver.FindElement(By.Id("EmailAddress")).SendKeys(inputUser.EmailAddress);
        _driver.FindElement(By.Id("Password")).SendKeys(password);
        inputUser.PasswordHash = Models.User.HashPassword(password);

        _driver.FindElement(By.Id("DateOfBirth"))
            .SendKeys(inputUser.DateOfBirth.ToShortDateString());
        _driver.FindElement(By.Id("ContractStart"))
            .SendKeys(inputUser.ContractStart.ToShortDateString());

        new SelectElement(_driver.FindElement(By.Id("ContractType")))
            .SelectByText(inputUser.ContractType.GetDisplayName());
        new SelectElement(_driver.FindElement(By.Id("SelectedDepartmentId")))
            .SelectByText(inputUser.Department.Name);

        var roleSelect = new SelectElement(_driver.FindElement(By.Id("SelectedRoleIds")));
        foreach (var role in inputUser.Roles) {
            roleSelect.SelectByText(role.Name);
        }

        if (inputUser.isSuspended)
            _driver.FindElement(By.Id("isSuspended")).Click();

        // submit
        var button = _driver.FindElement(By.XPath("//div[@class='user-details']/form/div/button[@type='submit']"));
        scrollTo(button);
        button.Submit();
    }

    // Made by Daniel Albert
    // verify that referenceUser exists in the database
    private void checkUser(Models.User referenceUser)
    {
        // navigate to user overview
        _driver.Navigate().GoToUrl(url + "/Usermanagement/UserOverview");
        waitUntilLoaded();
        Assert.Equal("Benutzerliste - Replay", _driver.Title);

        // check if user is present
        int foundRef = 0;
        IWebElement editButton = null!;
        var tableEntries = _driver.FindElements(By.XPath("//table[@class='user-table']/tbody/tr"));
        foreach (var entry in tableEntries) {
            string readName = entry.FindElement(By.XPath("./td[1]")).Text;
            string refName = referenceUser.FirstName + " " + referenceUser.LastName;
            DateTime readDoB = DateTime.Parse(entry.FindElement(By.XPath("./td[2]")).Text);

            if (readName.Equals(refName) && readDoB.Equals(referenceUser.DateOfBirth)) {
                foundRef++;
                editButton = entry.FindElement(By.XPath("./td[5]/a"));
            }
        }
        Assert.Equal(1, foundRef);
        Assert.NotNull(editButton);

        // navigate to user edit
        scrollTo(editButton);
        editButton.Click();
        waitUntilLoaded();
        Assert.Equal("Benutzer bearbeiten - Replay", _driver.Title);

        // read user data from form
        string UserName = _driver.FindElement(By.Id("UserName")).GetAttribute("value");
        DateTime DateOfBirth = DateTime.Parse(_driver.FindElement(By.Id("DateOfBirth")).GetAttribute("value"));
        DateTime ContractStart = DateTime.Parse(_driver.FindElement(By.Id("ContractStart")).GetAttribute("value"));
        string EmailAddress = _driver.FindElement(By.Id("EmailAddress")).GetAttribute("value");
        bool isSuspended = _driver.FindElement(By.Id("isSuspended")).Selected;
        bool isArchived = _driver.FindElement(By.Id("isArchived")).Selected;

        var ctSelect = new SelectElement(_driver.FindElement(By.Id("ContractType")));
        string ctName = ctSelect.SelectedOption.Text;
        _output.WriteLine($"ctName: {ctName}");

        var depSelect = new SelectElement(_driver.FindElement(By.Id("SelectedDepartmentId")));
        string depName = depSelect.SelectedOption.Text;
        _output.WriteLine($"depName: {depName}");

        var selectedRoles = _driver.FindElements(By.XPath("//ul[@id='select2-SelectedRoleIds-container']/li/span"));
        var roleNames = selectedRoles.Select(s => s.Text).ToList();
        roleNames.ForEach(r => _output.WriteLine($"role: \'{r}\'"));

        // check if user data is correct
        Assert.Equal(referenceUser.UserName, UserName);
        Assert.Equal(referenceUser.DateOfBirth, DateOfBirth);
        Assert.Equal(referenceUser.ContractStart, ContractStart);
        Assert.Equal(referenceUser.EmailAddress, EmailAddress);
        Assert.Equal(referenceUser.isSuspended, isSuspended);
        Assert.Equal(referenceUser.isArchived, isArchived);
        Assert.Equal(referenceUser.ContractType.GetDisplayName(), ctName);
        Assert.Equal(referenceUser.Department.Name, depName);
        
        var refRoleNames = referenceUser.Roles.Select(r => r.Name).ToList();
        int numRoles = refRoleNames.Count();
        Assert.Equal(numRoles, roleNames.Count());
        if (numRoles > 0) {
            refRoleNames.Sort();
            roleNames.Sort();
            Assert.True(Enumerable.SequenceEqual(refRoleNames, roleNames), "user roles are different");
        }
    }

    // Made by Daniel Albert
    // verify that referenceUser does not exists in the database
    private void checkUserMissing(Models.User referenceUser)
    {
        // navigate to user overview
        _driver.Navigate().GoToUrl(url + "/Usermanagement/UserOverview");
        waitUntilLoaded();
        Assert.Equal("Benutzerliste - Replay", _driver.Title);

        // check if user is missing
        int foundRef = 0;
        var tableEntries = _driver.FindElements(By.XPath("//table[@class='user-table']/tbody/tr"));
        foreach (var entry in tableEntries) {
            string readName = entry.FindElement(By.XPath("./td[1]")).Text;
            string refName = referenceUser.FirstName + " " + referenceUser.LastName;
            _output.WriteLine($"lang: {CultureInfo.CurrentCulture.Name}");
            DateTime readDoB = DateTime.Parse(entry.FindElement(By.XPath("./td[2]")).Text);

            if (readName.Equals(refName) && readDoB.Equals(referenceUser.DateOfBirth)) {
                foundRef++;
            }
        }
        Assert.True(foundRef == 0);
    }

    // Made by Daniel Albert
    private Models.User generateTestUser(string password)
    {
        var inputUser = new Models.User() {
            FirstName = "Perry",
            LastName = "Otter",
            EmailAddress = "test@example.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            ContractStart = new DateTime(2020, 1, 1),
            ContractType = ContractType.internship,
            isSuspended = false,
            PasswordHash = Models.User.HashPassword(password),
            Department = new Department() {
                Id = 4,
                Name = "Projektmanagement"
            }
        };
        inputUser.UserName = Models.User.GenerateUserName(inputUser.FirstName, inputUser.LastName);
        inputUser.DepartmentId = inputUser.Department.Id;

        inputUser.Roles = new List<Role>() {
            new Role() {Id = 2, Name = "IT", IsAdmin = false},
            new Role() {Id = 5, Name = "Personal", IsAdmin = false}
        };
        
        return inputUser;
    }

    // Made by Daniel Albert
    [Fact]
    public void AddUser_success()
    {
        login();

        // enter valid user data
        var password = "abc123";
        var inputUser = generateTestUser(password);
        enterUser(inputUser, password);

        // on success: redirect to Useroverview
        waitUntilLoaded();
        Assert.Equal("Benutzerliste - Replay", _driver.Title);
        checkUser(inputUser);
    }

    // Made by Daniel Albert
    [Theory]
    [InlineData("Albert", "Einstein", "")]
    [InlineData("", "Einstein", "abc123")]
    [InlineData("Albert", "", "abc123")]
    public void AddUser_fail(string FirstName, string LastName, string password)
    {
        login();

        // enter bad user data
        var inputUser = generateTestUser(password);
        inputUser.FirstName = FirstName;
        inputUser.LastName = LastName;
        enterUser(inputUser, password);

        // on failure: show UserAdd form again
        waitUntilLoaded();
        Assert.Equal("Benutzer hinzufügen - Replay", _driver.Title);
        checkUserMissing(inputUser);
    }
}
