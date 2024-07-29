using System;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace SoPro24Team03.E2ETests
{
    public class CreateProcedureTest : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly string _url;

        //Made by Celina
        public CreateProcedureTest(ITestOutputHelper output)
        {
            _output = output;

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("no-sandbox", "allow-insecure-localhost", "ignore-certificate-errors", "incognito");
            chromeOptions.AddArguments("start-maximized", "lang=de-de");
            chromeOptions.AddArguments("headless", "disable-gpu");
            chromeOptions.AddArguments("--disable-dev-shm-usage");
            _driver = new ChromeDriver(chromeOptions);
            _ = _driver.Manage().Timeouts().ImplicitWait;

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            CultureInfo ci = new CultureInfo("de-de");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            _url = "http://localhost:5000";
        }

        private void waitUntilLoaded()
        {
            Thread.Sleep(1000);
            _wait.Until(driver => {
                var exec = (IJavaScriptExecutor) driver;
                return exec.ExecuteScript("return document.readyState")
                    .Equals("complete");
            });
        }

        private void scrollTo(IWebElement element)
        {
            Actions a = new Actions(_driver);
            a.MoveToElement(element);
            a.Perform();
        }

        [Fact]
        public void CreateProcedure_ForExistingProcess_Success()
        {
            // 1. Navigate to the login page and login
            _driver.Navigate().GoToUrl(_url + "/Authentication");
            waitUntilLoaded();
            Assert.Equal("Login - Replay", _driver.Title);

            _driver.FindElement(By.Id("userName")).SendKeys("yardbil");
            _driver.FindElement(By.Id("password")).SendKeys("B1llard!");
            _driver.FindElement(By.Id("loginButton")).Click();

            // 2. Navigate to the procedure creation page
            _driver.Navigate().GoToUrl(_url + "/Procedure/Create/1");
            waitUntilLoaded();
            Assert.Equal("Vorgang anlegen - Replay", _driver.Title);

            // 3. Fill out the form
            _driver.FindElement(By.Id("Name")).SendKeys("Neuer Vorgang");
            _driver.FindElement(By.Id("ReferId")).SendKeys("1"); 
            _driver.FindElement(By.Id("RespId")).SendKeys("2"); 
            //_driver.FindElement(By.Id("TargetDate")).SendKeys("31122024");    // something wrong in pipeline
            _driver.FindElement(By.Id("ContractType")).SendKeys("Festanstellung");
            _driver.FindElement(By.Id("FutureDepartmentId")).SendKeys("3");

            Thread.Sleep(1000);

            // 4. Submit the form
            var submitButton = _driver.FindElement(By.CssSelector("input[type='submit']"));
            scrollTo(submitButton);
            submitButton.Click();

            waitUntilLoaded();
            Assert.Equal("Vorgang bearbeiten - Replay", _driver.Title);

            // 5. Verify the new procedure is listed
            _driver.Navigate().GoToUrl(_url + "/Procedure");
            waitUntilLoaded();
            Assert.Equal("Vorgänge - Replay", _driver.Title);
            var proceduresTable = _driver.FindElement(By.CssSelector("table"));
            Assert.Contains("Neuer Vorgang", proceduresTable.Text);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}