using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SampleMVCTest.Controllers;
using SampleMVCTest.Models;
using System.Linq;
using System.Data.Entity;
using System.Web.Http.Results;
using NUnit.Framework.Legacy;
using System.Web.Mvc;

namespace SampleTestProject
{
    [TestFixture]
    public class ItemsAPIControllerTests
    {
        private Mock<ItemDbContext> _mockContext;
        private ItemsApiController _controller;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<ItemDbContext>();
            _controller = new ItemsApiController();//_mockContext.Object
        }

        private Mock<DbSet<Item>> CreateMockDbSetForGetAll(List<Item> items)
        {
            var queryableItems = items.AsQueryable();

            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(queryableItems.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(queryableItems.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(queryableItems.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(queryableItems.GetEnumerator());

            return mockSet;
        }

        // Helper method to create a mock DbSet
        private Mock<DbSet<Item>> CreateMockDbSet(List<Item> items)
        {
            var queryableItems = items.AsQueryable();

            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(queryableItems.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(queryableItems.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(queryableItems.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(queryableItems.GetEnumerator());

            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] ids) => items.FirstOrDefault(i => i.Id == (int)ids[0]));
            mockSet.Setup(m => m.Add(It.IsAny<Item>())).Callback<Item>(items.Add);

            return mockSet;
        }

        [Test]
        public void GetItems_ReturnsOkResult_WithItems()
        {
            // Arrange
            var items = new List<Item>
            {
                new Item { Id = 1, Name = "Item1" },
                new Item { Id = 2, Name = "Item2" }
            };
            var mockSet = CreateMockDbSetForGetAll(items);
            _mockContext.Setup(m=>m.Items).Returns(mockSet.Object);

            var field = typeof(ItemsApiController).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_controller, _mockContext.Object);

            //_controller = new ItemsApiController
            //{
            //    // Directly assign the DbContext to the controller field
            //    // since DI isn't being used
            //    _context = _mockContext.Object
            //};

            // Act
            var actionresult = _controller.GetItems();
            var result = actionresult as OkNegotiatedContentResult<List<Item>>;

            // Assert
            //Assert.That(result, Is.Not.Null);
            //Assert.That(result.Content.Count, Is.EqualTo(2));
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Content.Count());
        }

        [Test]
        public void GetItem_ReturnsOkResult_WithExistingItem()
        {
            // Arrange
            var items = new List<Item>
            {
                new Item { Id = 1, Name = "Item1" }
            };
            var mockSet = CreateMockDbSet(items);
            _mockContext.Setup(m => m.Items).Returns(mockSet.Object);

            var field = typeof(ItemsApiController).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_controller, _mockContext.Object);


            // Act
            var actionresult = _controller.GetItem(1);
            var result = actionresult as OkNegotiatedContentResult<Item>;
            


            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(items.FirstOrDefault(i => i.Id == 1), result.Content);
        }

        [Test]
        public void GetItem_ReturnsNotFound_WithNonExistingItem()
        {
            // Arrange
            var items = new List<Item>();
            var mockSet = CreateMockDbSet(items);
            _mockContext.Setup(m => m.Items).Returns(mockSet.Object);

            var field = typeof(ItemsApiController).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_controller, _mockContext.Object);


            // Act
            var result = _controller.GetItem(1);

            // Assert
            ClassicAssert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Create_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange: Set the model state to invalid
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act: Call the Create action with an invalid item
            var result = _controller.AddItem(new Item());

            var res= result as BadRequestResult;

            ClassicAssert.IsInstanceOf<BadRequestResult>(res);

            _controller.ModelState.AddModelError("Description", "The Description field is required.");
            var item = new Item { Id = 1, Name = "Item1" };
            var items = new List<Item> { item };
            var mockSet = CreateMockDbSet(items);
            _mockContext.Setup(m => m.Items).Returns(mockSet.Object);
            var field = typeof(ItemsApiController).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_controller, _mockContext.Object);

            var updresult = _controller.UpdateItem(1,new Item());

            var updres = updresult as BadRequestResult;

            ClassicAssert.IsInstanceOf<BadRequestResult>(res);

        }

        [Test]
        public void PostItem_ReturnsOkResult_WithValidItem()
        {
            // Arrange
            var item = new Item { Id = 1, Name = "Item1" };
            var items = new List<Item>();
            var mockSet = CreateMockDbSet(items);
            _mockContext.Setup(m => m.Items).Returns(mockSet.Object);

            var field = typeof(ItemsApiController).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_controller, _mockContext.Object);


            // Act
            var actionresult = _controller.AddItem(item);
            var result = actionresult as OkNegotiatedContentResult<Item>;

            // Act
            //var result = _controller.AddItem(item) as CreatedAtRouteNegotiatedContentResult<Item>;


            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(item, result.Content);
        }

        [Test]
        public void PutItem_ReturnsOkResult_WithValidItem()
        {
            // Arrange
            var item = new Item { Id = 1, Name = "Item1" };
            var items = new List<Item> { item };
            var mockSet = CreateMockDbSet(items);
            _mockContext.Setup(m => m.Items).Returns(mockSet.Object);
            var field = typeof(ItemsApiController).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_controller, _mockContext.Object);


            // Act
            var actionresult = _controller.UpdateItem(1, item);
            var result = actionresult as OkNegotiatedContentResult<Item>;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(item, result.Content);
        }

        [Test]
        public void DeleteItem_ReturnsOkResult_WithExistingItem()
        {
            // Arrange
            var item = new Item { Id = 1, Name = "Item1" };
            var items = new List<Item> { item };
            var mockSet = CreateMockDbSet(items);
            _mockContext.Setup(m => m.Items).Returns(mockSet.Object); 
            var field = typeof(ItemsApiController).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_controller, _mockContext.Object);

            // Act
            var result = _controller.DeleteItem(1);

            // Assert
            ClassicAssert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void DeleteItem_ReturnsNotFound_WithNonExistingItem()
        {
            // Arrange
            var items = new List<Item>();
            var mockSet = CreateMockDbSet(items);
            _mockContext.Setup(m => m.Items).Returns(mockSet.Object);
            var field = typeof(ItemsApiController).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_controller, _mockContext.Object);

            // Act
            var result = _controller.DeleteItem(1);


            // Assert
            ClassicAssert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}