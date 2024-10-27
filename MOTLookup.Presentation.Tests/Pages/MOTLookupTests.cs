using Moq;
using MOTLookup.Models.Responses;
using MOTLookup.Models.Shared;
using MOTLookup.Presentation.Pages;
using MOTLookup.Service.IServices;
using System;
using System.Threading.Tasks;

namespace MOTLookup.Tests.Pages
{
    public class MOTLookupTests : TestContext
    {
        private readonly Mock<IMOTLookupService> _motServiceMock;

        public MOTLookupTests()
        {
            _motServiceMock = new Mock<IMOTLookupService>();
            Services.AddSingleton(_motServiceMock.Object);
        }

        [Fact]
        public void MOTLookupShowsErrorMessageForInvalidRegistration()
        {
            // Arrange
            var cut = RenderComponent<Presentation.Pages.MOTLookup>();
            var input = cut.Find("#registration");
            var button = cut.Find("button");

            // Act
            input.Change("INVALID");
            button.Click();

            // Assert
            cut.Find(".alert-danger").MarkupMatches("<div class=\"alert alert-danger mt-2\">Please enter a valid registration number.</div>");
        }

        [Fact]
        public async Task MOTLookupFetchesVehicleDataForValidRegistration()
        {
            // Arrange
            var vehicleResponse = new VehicleResponse("Ford", "Fiesta", "Green", new DateTime(2023, 12, 31), "15000");
            var result = new Result<VehicleResponse> { IsSuccess = true, Data = vehicleResponse };
            _motServiceMock.Setup(service => service.GetVehicleData(It.IsAny<string>())).ReturnsAsync(result);

            var cut = RenderComponent<Presentation.Pages.MOTLookup>();
            var input = cut.Find("#registration");
            var button = cut.Find("button");

            // Act
            input.Change("ABC123");
            button.Click();

            // Assert
            cut.WaitForState(() => cut.FindComponent<VehicleDetails>() != null);
            var vehicleDetails = cut.FindComponent<VehicleDetails>();
            Assert.NotNull(vehicleDetails);
        }
    }
}
