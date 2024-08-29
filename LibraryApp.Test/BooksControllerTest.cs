using Library.API.Data.Models;
using Library.API.Data.Services;
using LibraryApp.Controllers;
using LibraryApp.Data.MockData;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LibraryApp.Test
{
    public class BooksControllerTest
    {
        [Fact]
        public void IndexUnitTest()
        {
            //arrange
            var mockRepo = new Mock<IBookService>();
            mockRepo.Setup(n => n.GetAll()).Returns(MockData.GetTestBookItems());
            var controller = new BooksController(mockRepo.Object);

            //act
            var result = controller.Index();

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewResultBooks = Assert.IsAssignableFrom<List<Book>>(viewResult.ViewData.Model);
            Assert.Equal(5, viewResultBooks.Count);
        }

        [Theory]
        [InlineData("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200", "ab2bd817-98cd-4cf3-a80a-53ea0cd9c123")]
        public void DetailsUnitTest(string validGuid, string invalidGuid)
        {
            //arrange
            var mockRepo = new Mock<IBookService>();
            var validItemGuid = new Guid(validGuid);
            mockRepo.Setup(n => n.GetById(validItemGuid))
                .Returns(MockData.GetTestBookItems().FirstOrDefault(x => x.Id == validItemGuid));
            var controller = new BooksController(mockRepo.Object);
            //act
            var result = controller.Details(validItemGuid);
            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewResultValue = Assert.IsAssignableFrom<Book>(viewResult.ViewData.Model);
            Assert.Equal("Managing Oneself", viewResultValue.Title);
            Assert.Equal("Peter Drucker", viewResultValue.Author);
            Assert.Equal(validItemGuid, viewResultValue.Id);

            //arrange
            var invalidItemGuid = new Guid(invalidGuid);
            mockRepo.Setup(n => n.GetById(invalidItemGuid))
                .Returns(MockData.GetTestBookItems().FirstOrDefault(x => x.Id == invalidItemGuid));
            //act
            var notFoundResult = controller.Details(invalidItemGuid);
            //assert
            Assert.IsType<NotFoundResult>(notFoundResult);
        }
    }
}
