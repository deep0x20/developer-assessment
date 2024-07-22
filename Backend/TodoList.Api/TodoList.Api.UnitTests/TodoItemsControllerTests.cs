using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Api.Controllers;
using TodoList.Api.DAL;
using Xunit;
using Xunit.Abstractions;

namespace TodoList.Api.UnitTests
{
    public class TodoItemsControllerTests
    {
        private bool disposedValue;

        private TodoContext setupContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("TestTodoItemsDB").Options;
            var context = new TodoContext(options);
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        #region Get
        [Fact]
        public void Get_ShouldntReturnCompletedItems()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();

            repoMock.Setup(r => r.Get()).Returns(
                new List<TodoItem>() { 
                    new TodoItem { Description = "test", IsCompleted = true } 
                });

            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);
            var result = controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var todoItems = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.False(todoItems.Any());
        }

        [Fact]
        public void Get_ReturnsUncompletedItems()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();

            repoMock.Setup(r => r.Get()).Returns(
                new List<TodoItem>() {
                    new TodoItem { Description = "test", IsCompleted = false },
                     new TodoItem { Description = "test", IsCompleted = true }
               });

            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);

            var result = controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var todoItems = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Single(todoItems);
            Assert.False(todoItems.First().IsCompleted);

        }

        [Fact]
        public void Get_ReturnsOK()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();
            var item = new TodoItem { Description = "test", IsCompleted = false };

            repoMock.Setup(r => r.Get(item.Id)).Returns(item);

            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);

            var result = controller.Get(item.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Get_ReturnsNotFound_WhenNoItem()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();
            var item = new TodoItem { Description = "test", IsCompleted = false };

            repoMock.Setup(r => r.Get(item.Id)).Returns(value: null);

            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);

            var result = controller.Get(item.Id);
            Assert.IsType<NotFoundResult>(result);
        }
        #endregion

        #region put
        [Fact]
        public void Put_ReturnsBadRequest_MismatchedIds()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();
            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);

            var result = controller.Put(Guid.NewGuid(), new TodoItem());
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Put_ReturnsNotFound()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();
            var item = new TodoItem { Description = "test", IsCompleted = false };

            repoMock.Setup(r => r.Update(It.IsAny<TodoItem>())).Throws(new DbUpdateConcurrencyException());
            repoMock.Setup(r => r.Get(item.Id)).Returns(value: null);

            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);
            var result = controller.Put(item.Id, item);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Put_ReturnsNoContent_OnSuccess()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();
            var item = new TodoItem { Description = "test", IsCompleted = false };

            repoMock.Setup(r => r.Update(It.IsAny<TodoItem>())).Returns(item);
            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);
            var result = controller.Put(item.Id, item);

            Assert.IsType<NoContentResult>(result);
        }

        #endregion
        #region Post
        [Fact]
        public void Post_ReturnsBadRequest_NoDescription()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();
            var item = new TodoItem();

            repoMock.Setup(r => r.Add(It.IsAny<TodoItem>())).Returns(item);
            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);
            var result = controller.Post(item);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Post_ReturnsBadRequest_DuplicateDescription()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();
            var item = new TodoItem { Description = "test", IsCompleted = false };
            var item2 = new TodoItem { Description = "test", IsCompleted = false };

            repoMock.Setup(r => r.Get()).Returns(new List<TodoItem>() { item });
            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);
            var result = controller.Post(item2);

            Assert.IsType<BadRequestObjectResult>(result);
            
        }
        [Fact]
        public void Post_ReturnsCreatedAtAction_OnSuccess()
        {
            var loggerMock = new Mock<ILogger<TodoItemsController>>();
            var repoMock = new Mock<IRepository<TodoItem>>();
            var item = new TodoItem { Description = "test", IsCompleted = false };

            repoMock.Setup(r => r.Add(It.IsAny<TodoItem>())).Returns(item);
            var controller = new TodoItemsController(repoMock.Object, loggerMock.Object);

            var result = controller.Post(item);

            Assert.IsType<CreatedAtActionResult>(result);
        }
        #endregion
    }
}
