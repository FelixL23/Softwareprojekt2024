using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SoPro24Team03.E2ETests
{
    // Made by Fabio
    public class ProcedureOverviewTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private string _url;

        public ProcedureOverviewTest()
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
        public void ProcedureOverview()
        {
            _driver.Navigate().GoToUrl($"{_url}/Authentication");
            Thread.Sleep(1000);

            _driver.FindElement(By.Id("userName")).SendKeys("mustmax");
            _driver.FindElement(By.Id("password")).SendKeys("Passwort");
            _driver.FindElement(By.Id("loginButton")).Click();

            Thread.Sleep(1000);

            _driver.Navigate().GoToUrl($"{_url}/Procedure");
            Thread.Sleep(1000);
            Assert.Equal("Vorgänge - Replay", _driver.Title);

            var taskCounts = _driver.FindElements(By.Id("taskCount"));
            Console.WriteLine("Debug TaskCount: ", taskCounts.Count);

            Assert.Contains("0 / 1", taskCounts.First().Text);

            var nums = taskCounts.ElementAt(1).Text.Split("/");
            Assert.True(int.Parse(nums[0]) < int.Parse(nums[1]));
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}