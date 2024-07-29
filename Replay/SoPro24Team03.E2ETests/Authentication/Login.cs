using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using Xunit;
using Xunit.Abstractions;
using System.Globalization;


namespace SoPro24Team03.E2ETests;


// Made by Daniel Habenicht, Daniel Albert
public class Login : TestBase
{
    public Login(ITestOutputHelper output) : base(output)
    { }

    [Fact]
    public void LoginView()
    {
        _output.WriteLine("E2ETests: Login: LoginView(): start");

        //Navigate to Loing Page
        _driver.Navigate().GoToUrl(url);
        _output.WriteLine("E2ETests: Login: LoginView(): navigated to Login Page. Checking title...");

        //Check
        waitUntilLoaded();
        Assert.Equal("Login - Replay", _driver.Title);
        _output.WriteLine("E2ETests: Login: LoginView(): end");
    }

    [Fact]
    public void DashboardView()
    {
        _output.WriteLine("E2ETests: Login: DashboardView(): start");

        _driver.Navigate().GoToUrl(url);
        _output.WriteLine("E2ETests: Login: DashboardView(): navigated to Login Page. Checking title...");
        waitUntilLoaded();
        Assert.Equal("Login - Replay", _driver.Title);

        //input Felder für Login finden
        var userNameField = _driver.FindElement(By.Id("userName"));
        var inputPasswordField = _driver.FindElement(By.Id("password"));
        var loginButton = _driver.FindElement(By.Id("loginButton"));

        //Felder ausfüllen
        _output.WriteLine("E2ETests: Login: DashboardView(): entering credentials");
        userNameField.SendKeys("mustmax");
        inputPasswordField.SendKeys("Passwort");

        //Login durchführen
        _output.WriteLine("E2ETests: Login: DashboardView(): clicking button");
        loginButton.Click();

        //Check
        _output.WriteLine("E2ETests: Login: DashboardView(): logged in. checking title.");
        waitUntilLoaded();
        Assert.Equal("Dashboard - Replay", _driver.Title);

        _output.WriteLine("E2ETests: Login: DashboardView(): end.");
    }
}
