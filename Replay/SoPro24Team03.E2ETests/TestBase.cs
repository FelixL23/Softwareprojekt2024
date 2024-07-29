using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using Xunit;
using Xunit.Abstractions;


namespace SoPro24Team03.E2ETests;


// Made by Daniel Albert
public class TestBase : IDisposable
{
    protected readonly ITestOutputHelper _output;
    protected readonly IWebDriver _driver;
    protected readonly WebDriverWait _wait;
    protected readonly string url;

    // Made by Daniel Albert
    public TestBase(ITestOutputHelper output, bool headless = true)
    {
        url = "http://localhost:5000";

        _output = output;

        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArguments("no-sandbox", "allow-insecure-localhost", "ignore-certificate-errors", "incognito");
        chromeOptions.AddArguments("start-maximized", "lang=de-de");
        chromeOptions.AddArguments("--disable-dev-shm-usage");
        if (headless)
            chromeOptions.AddArguments("headless", "disable-gpu");
        _driver = new ChromeDriver(chromeOptions);

        _ = _driver.Manage().Timeouts().ImplicitWait;
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        CultureInfo ci = new CultureInfo("de-de");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;

        _output.WriteLine("TestBase: constructor finished.");
    }

    // Made by Daniel Albert
    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    // Made by Daniel Albert
    protected void waitUntilLoaded()
    {
        Thread.Sleep(1000);
        _wait.Until(driver => {
            var exec = (IJavaScriptExecutor) driver;
            return exec
                .ExecuteScript("return document.readyState")
                .Equals("complete");
        });
    }

    // Made by Daniel Albert
    protected void scrollTo(IWebElement element)
    {
        Actions a = new Actions(_driver);
        a.MoveToElement(element);
        a.Perform();
    }

    // Made by Daniel Albert
    protected void login(string username = "mustmax", string password = "Passwort")
    {
        // navigate to login page
        _driver.Navigate().GoToUrl(url);
        waitUntilLoaded();
        Assert.Equal("Login - Replay", _driver.Title);

        // enter user credentials
        _driver.FindElement(By.Id("userName")).SendKeys(username);
        _driver.FindElement(By.Id("password")).SendKeys(password);

        // submit
        _driver.FindElement(By.Id("loginButton")).Click();

        // on success: redirect to Dashboard
        waitUntilLoaded();
        Assert.Equal("Dashboard - Replay", _driver.Title);
    }
}
