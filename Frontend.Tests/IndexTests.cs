using Microsoft.Playwright;
public class IndexTests
{
  [Fact]
  public async void TestHomePage()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5103");
        var title = await page.TitleAsync();
        Assert.Equal("Home", title);
    }
}
