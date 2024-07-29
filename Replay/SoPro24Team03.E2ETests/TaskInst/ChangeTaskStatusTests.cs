using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit.Abstractions;

namespace SoPro24Team03.E2ETests.TaskInst
{
    // Made by Felix
    public class ChangeTaskStatusTests : TestBase
    {
        public ChangeTaskStatusTests(ITestOutputHelper output) : base(output, headless: true)
        {
        }

        public void Dispose()
        {
            _driver.Quit();
        }
         // Scroll to a specific element on the page
        public void ScrollToElement(IWebElement element)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        [Fact]
        public void TestTaskStatusChange()
        {
            //Navigate to Login
            _driver.Navigate().GoToUrl(url);

            //find and enter username
            var userNameField = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("userName"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            userNameField.SendKeys("mustmax");

            //find and enter password
            var passwordField = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("password"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            passwordField.SendKeys("Passwort");

            //find and click login button
            var loginButton = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("loginButton"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            loginButton.Click();

            _driver.Navigate().GoToUrl(url + "/TaskInst");

             // Find and click the edit button for a task
            var editButton = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.XPath("//a[@class='btn btn-primary' and @title='bearbeiten']"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            ScrollToElement(editButton);
            Thread.Sleep(500);
            editButton.Click();

             // Find the status dropdown
            var statusDropdown = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("Status"));
                return (element.Displayed && element.Enabled) ? element : null;
            });

             // Select "In Bearbeitung" status from the dropdown
            var selectElement = new SelectElement(statusDropdown);
            selectElement.SelectByValue("1"); 

             // Find and click the save button
            var saveButton = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.CssSelector("input[type='submit']"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            saveButton.Click();

             // Verify the status has been changed to "In Bearbeitung
            var statusElement = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.CssSelector("td:nth-child(6)"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            Assert.Equal("In Bearbeitung", statusElement.Text); 

            // Repeat the process to change status to "Erledigt"
            editButton = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.XPath("//a[@class='btn btn-primary' and @title='bearbeiten']"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            ScrollToElement(editButton);
            Thread.Sleep(500);
            editButton.Click();

            statusDropdown = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("Status"));
                return (element.Displayed && element.Enabled) ? element : null;
            });

            selectElement = new SelectElement(statusDropdown);
            selectElement.SelectByValue("2");

            saveButton = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.CssSelector("input[type='submit']"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            saveButton.Click();

            statusElement = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.CssSelector("td:nth-child(6)"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            Assert.Equal("Erledigt", statusElement.Text); 
        }
    }
}
