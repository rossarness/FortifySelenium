using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortifySelenium
{
    class TestCase_2
    {
        IWebDriver driver;

        private static readonly string targetURL = "https://www.alza.cz";
        private static readonly string targetSiteTitle = "Alza.cz";
        private static readonly string searchBoxId = "edtSearch";
        private static readonly string searchText = "Core i7";
        private static readonly string firstItemPath = "#boxes > div.box.browsingitem.canBuy.inStockAvailability.first.firstRow";
        private static readonly string allItemsPath = "#boxes > div:nth-child(n) > div.top > div.fb > a";
        private static readonly string itemNamePath = "#h1c > h1";
        private static readonly string[] languages = new string[2] { ".language > .langEN", ".langCZ" };
        private static readonly string langSelectorPath = "#languageSwitch > div.selector";
        private static readonly string searchBtnId = "btnSearch";
        private static readonly string[] searchBtnText = new string[2] { "Search", "Hledat" };
        private static readonly string langTestCategoryPath = "#headertop_rptSatellitesBeta_li_4 > .tab > .tabText";
        private static readonly string[] langTestCategoryText = new string[2] { "Media & Entertainment", "Media a zábava" };
        private static readonly string myAlzaId = "lblMujUcet";
        private static readonly string[] myAlzaText = new string[2] { "My Alza", "Moje Alza" };
        private static readonly string subCategoryTestPath = "#litp18842920 > .bx";
        private static readonly string testElementPath = "#besti > h4";
        private static readonly string[] testElementText = new string[2] { "Best Deals", "Nejprodávanější" };
        private static readonly string backToMain = "#logo > a";

        [SetUp]
        public void StartBrowser()
        {
            driver = new ChromeDriver("C:\\chromedriver\\chromedriver_win32\\")
            {
                Url = targetURL
            };
            Assert.IsTrue(driver.Title.Contains(targetSiteTitle));
        }

        [Test]
        public void SearchBox()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Random random = new Random();

            IWebElement searchBox = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(searchBoxId)));
            searchBox.SendKeys(searchText);
            searchBox.SendKeys(Keys.Return);
            IWebElement firstItem = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(firstItemPath)));
            Assert.IsTrue(firstItem.Text.Contains(searchText));
            var searchResults = driver.FindElements(By.CssSelector(allItemsPath));
            foreach(IWebElement result in searchResults)
            {
                Assert.IsTrue(result.Text.Contains(searchText));
            }
            int item = random.Next(searchResults.Count);
            string elementText = searchResults[item].Text;
            searchResults[item].Click();
            IWebElement selectedItem = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(itemNamePath)));
            Assert.IsTrue(selectedItem.Text.Equals(elementText));
        }

        [Test]
        public void LanguageSelector()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            for (int i = 0; i < languages.Count(); i++)
            {
                IWebElement langSelector = driver.FindElement(By.CssSelector(langSelectorPath));
                langSelector.Click();
                IWebElement language = driver.FindElement(By.CssSelector(languages[i]));
                language.Click();
                IWebElement searchBtn = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(searchBtnId)));
                Assert.IsTrue(searchBtn.Text.Contains(searchBtnText[i]));
                IWebElement langTestCategory = driver.FindElement(By.CssSelector(langTestCategoryPath));
                Assert.IsTrue(langTestCategory.Text.Contains(langTestCategoryText[i]));
                IWebElement myAlza = driver.FindElement(By.Id(myAlzaId));
                Assert.IsTrue(myAlza.Text.Contains(myAlzaText[i]));
                driver.FindElement(By.CssSelector(subCategoryTestPath)).Click();
                IWebElement subCategoryTitle = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(testElementPath)));
                Assert.IsTrue(subCategoryTitle.Text.Contains(testElementText[i]));
                driver.FindElement(By.CssSelector(backToMain)).Click();

            }
            driver.FindElement(By.CssSelector(backToMain)).Click();
        }
        
        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
        }
    }
}
