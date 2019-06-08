using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Text.RegularExpressions;

namespace FortifySelenium
{
    class TestCase_1
    {
        IWebDriver driver;

        private static readonly string targetURL = "https://www.alza.cz";
        private static readonly string targetSiteTitle = "Alza.cz";
        private static readonly string targetCategoryPath = "#litp18852653 > div > a";
        private static readonly string hoverMenuId = "fb18852653";
        private static readonly string subCategoryPath = "#fb18852653 > div > div.c.c1 > div:nth-child(1) > div.cl > a";
        private static readonly string subCategoryText = "Alza počítače";
        private static readonly string orderById = "ui-id-4";
        private static readonly string orderByActivePath = "#tabs > ul > li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active > a";
        private static readonly string loadingIcon = "body > span.circle-loader-container";
        private static readonly string itemPricesPath = "#boxes > div:nth-child(n) > div.bottom > div.price.vkc > div > span.c2";
        private static readonly string addToBasketItem1Path = "#boxes > div.box.browsingitem.canBuy.otherAvailability.first.firstRow > div.bottom > div.price > span > a.btnk1";
        private static readonly string item1NamePath = "#boxes > div.box.browsingitem.canBuy.otherAvailability.first.firstRow > div.top > div.fb > a";
        private static readonly string itemAddedPath = "#csinfo";
        private static readonly string backBtnPath = "#content0 > div.obuttons.crossbuttons.crossTopbuttons > a.btnx.normal.grey.arrowedLeft.floatLeft";
        private static readonly string addToBasketItemsPath = "#boxes > div:nth-child(n) > div.bottom > div.price.vkc > span > a.btnk1";
        private static readonly string itemNamesPath = "#boxes > div:nth-child(n) > div.top > div.fb > a";
        private static readonly string itemNotAvailableClass = "watchDog";

        [SetUp]
        public void StartBrowser()
        {
            driver = new ChromeDriver("C:\\chromedriver\\chromedriver_win32\\");
        }

        [Test]
        public void AddToCart()
        {
            Actions action = new Actions(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Regex pricesRgx = new Regex(@"(\d{ 1, 6 })");
            string itemTemp;
            string item1Name;
            string item2Name = "";
            string addedItemName;

            driver.Url = targetURL;
            Assert.IsTrue(driver.Title.Contains(targetSiteTitle));
            IWebElement category = driver.FindElement(By.CssSelector(targetCategoryPath));
            action.MoveToElement(category).Perform();
            IWebElement hoverMenu = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(hoverMenuId)));
            IWebElement subCategory = driver.FindElement(By.CssSelector(subCategoryPath));
            subCategory.Click();
            wait.Until(ExpectedConditions.TitleContains(subCategoryText));
            IWebElement orderBy = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(orderById)));
            orderBy.Click();
            IWebElement orderByActive = driver.FindElement(By.CssSelector(orderByActivePath));
            Assert.IsTrue(orderBy.GetAttribute("id").Equals(orderByActive.GetAttribute("id")));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(loadingIcon)));
            var itemPrices = driver.FindElements(By.CssSelector(itemPricesPath));
            int itemCount = itemPrices.Count;
            int loopCount = 0;
            while (true)
            {
                if (loopCount >= (itemCount - 1))
                {
                    loopCount = 0;
                    break;
                }
                itemTemp = pricesRgx.Matches(itemPrices[loopCount].Text).ToString();
                Console.WriteLine(itemTemp);
                Int32.TryParse(itemTemp, out int item1Price);
                itemTemp = pricesRgx.Matches(itemPrices[loopCount + 1].Text).ToString();
                Int32.TryParse(itemTemp, out int item2Price);
                Assert.IsTrue(item1Price >= item2Price);
                loopCount++;
            }
            IWebElement addToBasketItem1 = driver.FindElement(By.CssSelector(addToBasketItem1Path));
            item1Name = driver.FindElement(By.CssSelector(item1NamePath)).Text;
            addToBasketItem1.Click();
            IWebElement itemAdded = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(itemAddedPath)));
            addedItemName = itemAdded.Text;
            Assert.IsTrue(addedItemName.Contains(item1Name));
            IWebElement backBtn = driver.FindElement(By.CssSelector(backBtnPath));
            backBtn.Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(loadingIcon)));
            var availableItems = driver.FindElements(By.CssSelector(addToBasketItemsPath));
            var availableItemNames = driver.FindElements(By.CssSelector(itemNamesPath));
            while (true)
            {
                if (availableItems[loopCount].GetAttribute("Class").Contains(itemNotAvailableClass))
                {
                    loopCount++;
                }
                else
                {
                    item2Name = availableItemNames[loopCount].Text;
                    availableItems[loopCount].Click();
                    break;
                }
                itemAdded = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(itemAddedPath)));
                addedItemName = itemAdded.Text;
                Assert.IsTrue(addedItemName.Contains(item2Name));
            }

        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
        }
    }
}
