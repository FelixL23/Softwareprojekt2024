using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace SoPro24Team03.E2ETests.TaskInst;

public class EditTaskInst : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private readonly string _url;

    public EditTaskInst(ITestOutputHelper output)
    {
        _output = output;

        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArguments("no-sandbox", "allow-insecure-localhost", "ignore-certificate-errors", "incognito");
        chromeOptions.AddArguments("--start-maximized");
        chromeOptions.AddArguments("--headless", "--disable-gpu");
        chromeOptions.AddArguments("--disable-dev-shm-usage");
        _driver = new ChromeDriver(chromeOptions);
        _ = _driver.Manage().Timeouts().ImplicitWait;

        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        _url = "http://localhost:5000";
        //_url = "https://localhost:5001";
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    [Fact]
    public void TaskInstView()
    {
        // TODO: login

        /*
        _driver.Navigate().GoToUrl(_url + "/TaskInst");
        Assert.Equal("Aufgaben - Replay", _driver.Title);
        */
    }
}