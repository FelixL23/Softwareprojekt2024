using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit.Abstractions;

namespace SoPro24Team03.E2ETests.TaskInst
{
    public class TaskToRole : TestBase
    {
        // End-to-end tests for assigning tasks to roles made by Felix
        public TaskToRole(ITestOutputHelper output) : base(output, headless: true)
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
        public void TestTaskToRole()
        {
            // Navigate to the login page
            _driver.Navigate().GoToUrl(url);

           // Find and enter the username
            var userNameField = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("userName"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            userNameField.SendKeys("yardbil");
            // Find and enter the password
            var passwordField = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("password"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            passwordField.SendKeys("B1llard!");

            // Find and click the login button
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
                var element = driver.FindElement(By.CssSelector("a.btn.btn-primary[title='bearbeiten']"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            ScrollToElement(editButton);
            Thread.Sleep(500); // Wait for the page to scroll
            editButton.Click();

            // Find the "SelectRespType" dropdown
            var statusDropdownResp = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("SelectRespType"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            ScrollToElement(statusDropdownResp);
            Thread.Sleep(500);

            // Select "role" from the dropdown
            var selectElement = new SelectElement(statusDropdownResp);
            selectElement.SelectByValue("role");

            // Find the "RespRoleId" dropdown
            var statusDropdownRole = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.Id("RespRoleId"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            ScrollToElement(statusDropdownRole);
            Thread.Sleep(500);

            //select specific Role
            selectElement = new SelectElement(statusDropdownRole);
            selectElement.SelectByValue("5");

            // Find and click the save button
            var saveButton = _wait.Until(driver => 
            {
                var element = driver.FindElement(By.CssSelector("input[type='submit']"));
                return (element.Displayed && element.Enabled) ? element : null;
            });
            ScrollToElement(saveButton);
            Thread.Sleep(500);
            saveButton.Click();
        }
    }
}
