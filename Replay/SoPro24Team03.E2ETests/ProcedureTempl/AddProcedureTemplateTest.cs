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
public class AddProcedureTemplateTest : TestBase
{
    class TestData  // representation of ProcedureTemplate
    {
        public string Name {get; set;}
        public string[] Roles {get; set;}   // Name of Roles
        public string[] Tasks {get; set;}   // Name of TaskTemplates
    }

    // Made by Daniel Albert
    public AddProcedureTemplateTest(ITestOutputHelper output) :
        base(output)
    { }

    // Made by Daniel Albert
    private void enterProcTempl(TestData inputProcTempl)
    {
        // navigate to create menu
        _driver.Navigate().GoToUrl(url + "/ProcedureTemplate/Create");
        waitUntilLoaded();
        Assert.Equal("Prozess erstellen - Replay", _driver.Title);

        // enter template name
        _output.WriteLine($"Adding ProcedureTemplate: {inputProcTempl.Name}");
        _driver.FindElement(By.Id("Name")).SendKeys(inputProcTempl.Name);
        
        // select roles
        var roleSelect = new SelectElement(_driver.FindElement(By.Id("SelectedRoles")));
        foreach (var roleName in inputProcTempl.Roles) {
            roleSelect.SelectByText(roleName);
        }

        // open add task menu
        var addTaskButton = _driver.FindElement(By.XPath("//div[@id='taskContainer']/../div[@class='row']/button"));
        scrollTo(addTaskButton);
        addTaskButton.Click();
        Thread.Sleep(1000);

        // select task templates
        var taskTemplates = _driver.FindElements(By.XPath("//div[@id='taskTemplate-accordion']/div[@class='accordion-item']"));
        foreach (var taskTempl in taskTemplates) {
            string taskName = taskTempl.FindElement(By.XPath("./h2/button/span")).Text;
            _output.WriteLine($"  {taskName}");

            if (inputProcTempl.Tasks.Any(t => t.Equals(taskName))) {
                // select this task template
                taskTempl.FindElement(By.XPath("./h2/button/input")).Click();
            }
        }

        // save and close add task menu
        var saveTasksButton = _driver.FindElement(By.Id("saveTasksBtn"));
        scrollTo(saveTasksButton);
        saveTasksButton.Click();
        Thread.Sleep(1000);

        // submit
        var button = _driver.FindElement(By.Id("submit"));
        scrollTo(button);
        button.Submit();
    }

    // Made by Daniel Albert
    // verify that refProcTempl exists in the database
    private void checkProcTempl(TestData refProcTempl)
    {
        // navigate to overview
        _driver.Navigate().GoToUrl(url + "/ProcedureTemplate");
        waitUntilLoaded();
        Assert.Equal("Prozesse - Replay", _driver.Title);

        // select test template
        _output.WriteLine($"ProcTempl list:");
        int foundRef = 0;
        IWebElement detailsButton = null!;
        var procTemplates = _driver.FindElements(By.XPath("//section[@id='details-container']/../section[1]/span"));
        foreach (var procTempl in procTemplates) {
            string procName = procTempl.FindElement(By.XPath("./div")).Text;
            _output.WriteLine($"  {procName}");

            if (procName.Equals(refProcTempl.Name)) {
                foundRef++;
                detailsButton = procTempl;
            }
        }
        Assert.Equal(1, foundRef);
        Assert.NotNull(detailsButton);
        detailsButton.Click();
        Thread.Sleep(1000);

        // check roles
        var roleEntries = _driver.FindElements(By.XPath("//section[@id='details-container']/div/div/div[1]/div/div/ul/li"));
        var roleNames = roleEntries.Select(e => e.Text).ToList();
        var refRoleNames = refProcTempl.Roles.ToList();
        Assert.Equal(refRoleNames.Count(), roleNames.Count());
        if (refRoleNames.Count() > 0) {
            refRoleNames.Sort();
            roleNames.Sort();
            Assert.True(Enumerable.SequenceEqual(refRoleNames, roleNames), "procedure template roles are different");
        }

        // check tasks
        var taskEntries = _driver.FindElements(By.XPath("//section[@id='details-container']/div/div/div[2]/div/div"));
        var taskNames = taskEntries.Select(e => e.Text).ToList();
        var refTaskNames = refProcTempl.Tasks.ToList();
        Assert.Equal(refTaskNames.Count(), taskNames.Count());
        if (refTaskNames.Count() > 0) {
            refTaskNames.Sort();
            taskNames.Sort();
            Assert.True(Enumerable.SequenceEqual(refTaskNames, taskNames), "procedure template tasks are different");
        }
    }

    // Made by Daniel Albert
    // verify that refProcTempl does not exists in the database
    private void checkProcTemplMissing(TestData refProcTempl)
    {
        // navigate to overview
        _driver.Navigate().GoToUrl(url + "/ProcedureTemplate");
        waitUntilLoaded();
        Assert.Equal("Prozesse - Replay", _driver.Title);

        // select test template
        _output.WriteLine($"ProcTempl list:");
        int foundRef = 0;
        var procTemplates = _driver.FindElements(By.XPath("//section[@id='details-container']/../section[1]/span"));
        foreach (var procTempl in procTemplates) {
            string procName = procTempl.FindElement(By.XPath("./div")).Text;
            _output.WriteLine($"  {procName}");

            if (procName.Equals(refProcTempl.Name)) {
                foundRef++;
            }
        }
        Assert.Equal(0, foundRef);
    }

    // Made by Daniel Albert
    [Theory]
    [InlineData("TestProzess1", new string[] {"Administratoren", "Personal"}, new string[] {"TestTaskTempl1", "TestTaskTempl2", "TestTaskTempl3"})]
    public void AddProcTempl_success(string name, string[] roles, string[] tasks)
    {
        login();

        // enter valid template data
        var inputProcTempl = new TestData() {
            Name = name,
            Roles = roles,
            Tasks = tasks
        };
        enterProcTempl(inputProcTempl);

        // on success: redirect to overview
        waitUntilLoaded();
        Assert.Equal("Prozesse - Replay", _driver.Title);
        checkProcTempl(inputProcTempl);
    }

    // Made by Daniel Albert
    [Theory]
    [InlineData("", new string[] {"Administratoren", "Personal"}, new string[] {"TestTaskTempl1", "TestTaskTempl2", "TestTaskTempl3"})]
     public void AddProcTempl_fail(string name, string[] roles, string[] tasks)
    {
        login();

        // enter bad template data
        var inputProcTempl = new TestData() {
            Name = name,
            Roles = roles,
            Tasks = tasks
        };
        enterProcTempl(inputProcTempl);

        // on failure: show create form again
        waitUntilLoaded();
        Assert.Equal("Prozess erstellen - Replay", _driver.Title);
        checkProcTemplMissing(inputProcTempl);
    }
}
