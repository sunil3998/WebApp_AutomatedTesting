using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApp.Controllers;
using WebApp.Models;
using WebApp.Repository.Interface;

namespace WebApp.test.Controllers
{
    public class HomeControllerTest
    {
        private readonly HomeController controller;
        private readonly Mock<ICreditCardsRepository> repositoryMock;
        private readonly Mock<ILogger<HomeController>> loggerMock;
        private readonly List<CreditCardsModel> seededCards;

        public HomeControllerTest()
        {
            // Arrange: Set up mock repository and logger
            seededCards = new List<CreditCardsModel>
            {
                new CreditCardsModel { Id = 1, CardType = "MoneyBack", CreditLimit = 50000, AnnualCharge = 5000 },
                new CreditCardsModel { Id = 2, CardType = "Platinum", CreditLimit = 6000, AnnualCharge = 900 }
            };

            repositoryMock = new Mock<ICreditCardsRepository>();
            loggerMock = new Mock<ILogger<HomeController>>();

            repositoryMock.Setup(r => r.GetAllCreditCards()).Returns(seededCards);

            controller = new HomeController(loggerMock.Object, repositoryMock.Object);
            // Add this to prevent NullReferenceException in Error()
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void HomeController_Index_ValidResult()
        {
            // Act
            var result = controller.Index() as ViewResult;
            var model = result?.Model as IEnumerable<CreditCardsModel>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(seededCards.Count, model.Count());
            Assert.True(seededCards.SequenceEqual(model, new CreditCardsModelComparer()));
        }
        [Fact]
        public void HomeController_Privacy_ReturnsView()
        {
            /// Arrange
           string expectedMessage = "I am doing testing";

            /// Act
            var result = controller.Privacy() as ViewResult;

            /// Assert
            Assert.Equal(result?.Model,expectedMessage);
        }

        [Fact]
        public void HomeController_Create_Get_ReturnsViewWithModel()
        {
            var result = controller.Create() as ViewResult;
            Assert.NotNull(result);
            Assert.IsType<CreditCardsModel>(result.Model);
        }

        [Fact]
        public void HomeController_Create_Post_ValidModel_RedirectsToIndex()
        {
            var newCard = new CreditCardsModel { Id = 3, CardType = "Gold", CreditLimit = 10000, AnnualCharge = 1000 };
            repositoryMock.Setup(r => r.AddCreditCard(It.IsAny<CreditCardsModel>())).Verifiable();
            controller.ModelState.Clear();

            var result = controller.Create(newCard) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            repositoryMock.Verify(r => r.AddCreditCard(It.IsAny<CreditCardsModel>()), Times.Once);
        }

        [Fact]
        public void HomeController_Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            controller.ModelState.AddModelError("CardType", "Required");
            var invalidCard = new CreditCardsModel();
            var result = controller.Create(invalidCard) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(invalidCard, result.Model);
        }

        [Fact]
        public void HomeController_Edit_Get_ValidId_ReturnsViewWithModel()
        {
            var card = seededCards.First();
            repositoryMock.Setup(r => r.GetCreditCard(card.Id)).Returns(card);

            var result = controller.Edit(card.Id) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(card, result.Model);
        }

        [Fact]
        public void HomeController_Edit_Get_InvalidId_ReturnsBadRequest()
        {
            var result = controller.Edit(0);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void HomeController_Edit_Get_NotFound_ReturnsNotFound()
        {
            repositoryMock.Setup(r => r.GetCreditCard(99)).Returns((CreditCardsModel)null);
            var result = controller.Edit(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void HomeController_Edit_Post_ValidModel_RedirectsToIndex()
        {
            var card = seededCards.First();
            controller.ModelState.Clear();
            repositoryMock.Setup(r => r.UpdateCreditCard(card)).Verifiable();

            var result = controller.Edit(card) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            repositoryMock.Verify(r => r.UpdateCreditCard(card), Times.Once);
        }

        [Fact]
        public void HomeController_Edit_Post_InvalidModel_ReturnsViewWithModel()
        {
            controller.ModelState.AddModelError("CardType", "Required");
            var card = seededCards.First();
            var result = controller.Edit(card) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(card, result.Model);
        }

        [Fact]
        public void HomeController_Details_ValidId_ReturnsViewWithModel()
        {
            var card = seededCards.First();
            repositoryMock.Setup(r => r.GetCreditCard(card.Id)).Returns(card);

            var result = controller.Details(card.Id) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(card, result.Model);
        }

        [Fact]
        public void HomeController_Details_InvalidId_ReturnsBadRequest()
        {
            var result = controller.Details(0);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void HomeController_Details_NotFound_ReturnsNotFound()
        {
            repositoryMock.Setup(r => r.GetCreditCard(99)).Returns((CreditCardsModel)null);
            var result = controller.Details(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void HomeController_Delete_Get_ValidId_ReturnsViewWithModel()
        {
            var card = seededCards.First();
            repositoryMock.Setup(r => r.GetCreditCard(card.Id)).Returns(card);

            var result = controller.Delete(card.Id) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(card, result.Model);
        }

        [Fact]
        public void HomeController_Delete_Get_InvalidId_ReturnsBadRequest()
        {
            var result = controller.Delete(0);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void HomeController_Delete_Get_NotFound_ReturnsNotFound()
        {
            repositoryMock.Setup(r => r.GetCreditCard(99)).Returns((CreditCardsModel)null);
            var result = controller.Delete(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void HomeController_DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            var card = seededCards.First();
            repositoryMock.Setup(r => r.GetCreditCard(card.Id)).Returns(card);
            repositoryMock.Setup(r => r.DeleteCreditCard(card.Id)).Verifiable();

            var result = controller.DeleteConfirmed(card.Id) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            repositoryMock.Verify(r => r.DeleteCreditCard(card.Id), Times.Once);
        }

        [Fact]
        public void HomeController_DeleteConfirmed_InvalidId_ReturnsBadRequest()
        {
            var result = controller.DeleteConfirmed(0);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void HomeController_DeleteConfirmed_NotFound_RedirectsToIndex()
        {
            repositoryMock.Setup(r => r.GetCreditCard(99)).Returns((CreditCardsModel)null);
            var result = controller.DeleteConfirmed(99) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public void HomeController_Error_ReturnsViewWithErrorViewModel()
        {
            // Act
            var result = controller.Error() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsType<ErrorViewModel>(result.Model);
        }

        private class CreditCardsModelComparer : IEqualityComparer<CreditCardsModel>
        {
            public bool Equals(CreditCardsModel x, CreditCardsModel y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.Id == y.Id &&
                       x.CardType == y.CardType &&
                       x.CreditLimit == y.CreditLimit &&
                       x.AnnualCharge == y.AnnualCharge;
            }

            public int GetHashCode(CreditCardsModel obj)
            {
                return HashCode.Combine(obj.Id, obj.CardType, obj.CreditLimit, obj.AnnualCharge);
            }
        }
    }
}
