using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Legacy;
using OpenQA.Selenium.Support.UI;

namespace SampleTestProject
{
    [TestFixture]
    public class ItemsControllerTests
    {
        private IWebDriver _driver; 
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl("https://localhost:44369/"); // Adjust to your local running URL
        }

        [Test]
        public void CreateItem_Should_Add_New_Item()
        {
            // Navigate to Create view
            _driver.Navigate().GoToUrl("https://localhost:44369/Items/Create");

            // Find the form fields and fill them
            var nameInput = _driver.FindElement(By.Id("txt_name"));
            var descriptionInput = _driver.FindElement(By.Id("txt_desc"));

            nameInput.SendKeys("New Item");
            descriptionInput.SendKeys("New Item Description");

            // Submit the form
            _driver.FindElement(By.CssSelector("input[type='submit']")).Click();

            // Wait for any alerts and handle them
            HandleAlerts();

            // Wait for the page to load
            //wait.Until(d => d.Url.Contains("/Items")); // Adjust this to your items list URL

            System.Threading.Thread.Sleep(3000);
           
            // Navigate to the Items list page
           _driver.Navigate().GoToUrl("https://localhost:44369/Items"); // Change this to your Items List URL
            _driver.Navigate().Refresh();
            // Find the table that contains the items
            //IWebElement itemsTable = _driver.FindElement(By.Id("itemsTable")); // Update with your table's actual ID
            //var rows = itemsTable.FindElements(By.TagName("tr"));

            //// Loop through each row and print the cell text
            //foreach (var row in rows)
            //{
            //    var cells = row.FindElements(By.TagName("td"));
            //    foreach (var cell in cells)
            //    {
            //        Console.Write(cell.Text + " "); // Print cell text
            //    }
            //    Console.WriteLine(); // New line after each row
            //}


            // Optionally, check if the newly created item is present in the table
            ClassicAssert.IsTrue(_driver.PageSource.Contains("New Item"), "The item was not successfully created in the table.");
            //System.Threading.Thread.Sleep(2000);
            // Assert: Check if we were redirected to the Index view
            ClassicAssert.AreEqual("https://localhost:44369/Items", _driver.Url);
        }
        private void HandleAlerts()
        {
            try
            {
                // Wait for the alert to be present
                //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());

                // Switch to alert and accept it
                IAlert alert = _driver.SwitchTo().Alert();
                Console.WriteLine("Alert appeared: " + alert.Text);
                alert.Accept();
            }
            catch (NoAlertPresentException)
            {
                // No alert was present, continue with the test
                Console.WriteLine("No alert present.");
            }
            catch (WebDriverException)
            {
                // Log any other WebDriver exceptions related to alerts
                Console.WriteLine("Failed to handle the alert.");
            }
        }

        [Test]
        public void ListItems_Should_Display_Items()
        {
            // Navigate to the Index page
            _driver.Navigate().GoToUrl("https://localhost:44369/Items");

            // Find the table that lists the items
            var itemsTable = _driver.FindElement(By.Id("itemsTable"));

            // Assert: Ensure that the table contains at least one item
            ClassicAssert.IsTrue(itemsTable.Text.Contains("Test"));
        }

        [Test]
        public void ViewItemDetails_Should_Display_Details()
        {
            // Navigate to the Details page for item with ID 1
            _driver.Navigate().GoToUrl("https://localhost:44369/Items");

            IList<IWebElement> links = _driver.FindElements(By.TagName("a"));
            links.First(element => element.Text == "View Details").Click();

            // Assert: Ensure that the details page displays the correct item
            var itemName = _driver.FindElement(By.Id("itemName"));
            ClassicAssert.AreEqual("Mech", itemName.Text);
        }

        [Test]
        public void EditItem_Should_Update_Item()
        {
            // Navigate to the Edit page for item with ID 1
            _driver.Navigate().GoToUrl("https://localhost:44369/Items/Edit/10");

            // Find the form fields and update the values
            var nameInput = _driver.FindElement(By.Id("updateName"));
            nameInput.Clear();
            nameInput.SendKeys("Updated Item");

            // Submit the form
            _driver.FindElement(By.CssSelector("input[type='submit']")).Click();

            // Assert: Check if we were redirected to the Index view
            ClassicAssert.AreEqual("https://localhost:44369/Items", _driver.Url);

            // Verify that the item was updated
            var itemsTable = _driver.FindElement(By.Id("itemsTable"));
            ClassicAssert.IsTrue(itemsTable.Text.Contains("Updated Item"));
        }

        [Test]
        public void DeleteItem_Should_Remove_Item()
        {
            // Navigate to the Delete page for item with ID 1
            _driver.Navigate().GoToUrl("https://localhost:44369/Items/Delete/10");

            // Confirm the deletion
            _driver.FindElement(By.Id("btnDelete")).Click();

            // Assert: Check if we were redirected to the Index view
            ClassicAssert.AreEqual("https://localhost:44369/Items", _driver.Url);

            // Verify that the item was removed from the list
            var itemsTable = _driver.FindElement(By.Id("itemsTable"));
            ClassicAssert.IsFalse(itemsTable.Text.Contains("Updated Item"));
        }

        [TearDown]
        public void Teardown()
        {
            _driver.Quit();
        }
    }
}
