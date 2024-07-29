using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SoPro24Team03.E2ETests
{
    // Made by Fabio
    public class TaskOverviewTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private string _url;

        public TaskOverviewTests()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("no-sandbox", "allow-insecure-localhost", "ignore-certificate-errors", "incognito");
            chromeOptions.AddArguments("start-maximized");
            chromeOptions.AddArguments("headless", "disable-gpu");
            chromeOptions.AddArguments("--disable-dev-shm-usage");

            _driver = new ChromeDriver(chromeOptions);
            _url = "http://localhost:5000";
        }

        [Fact]
        public void A_UserTaskOverviewIsOverdue()
        {
            _driver.Navigate().GoToUrl($"{_url}/Authentication");
            Thread.Sleep(1000);

            _driver.FindElement(By.Id("userName")).SendKeys("mustmax");
            _driver.FindElement(By.Id("password")).SendKeys("Passwort");
            _driver.FindElement(By.Id("loginButton")).Click();

            Thread.Sleep(1000);

            _driver.Navigate().GoToUrl($"{_url}/TaskInst?TaskName=&ProcedureId=&level=1");
            Thread.Sleep(1000);
            Assert.Equal("Aufgaben - Replay", _driver.Title);

            var taskRows = _driver.FindElements(By.Id("taskRow"));
            var specificTask = taskRows.ToList().Find(e => e.FindElement(By.Id("taskName")).Text == "Nichts tun");
            
            Assert.Equal("rgba(255, 0, 0, 1)", specificTask?.GetCssValue("color"));
        }

        [Fact]
        public void A_UserTaskOverviewStatusBacklog()
        {
            _driver.Navigate().GoToUrl($"{_url}/Authentication");
            Thread.Sleep(1000);

            _driver.FindElement(By.Id("userName")).SendKeys("mustmax");
            _driver.FindElement(By.Id("password")).SendKeys("Passwort");
            _driver.FindElement(By.Id("loginButton")).Click();

            Thread.Sleep(1000);

            _driver.Navigate().GoToUrl($"{_url}/TaskInst?TaskName=&ProcedureId=&level=1");
            Thread.Sleep(1000);
            Assert.Equal("Aufgaben - Replay", _driver.Title);

            var taskRows = _driver.FindElements(By.Id("taskRow"));
            var specificTask = taskRows.ToList().Find(e => e.FindElement(By.Id("taskName")).Text == "TestTaskTempl1");

            Assert.Contains("In Bearbeitung", specificTask?.FindElement(By.Id("taskStatus")).Text);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}