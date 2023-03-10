using AutoFixture;
using BlazorStrap;
using Bunit;
using CrewLog.Client.Pages.Shifts;
using CrewLog.Client.Stores;
using CrewLog.Shared.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewLog.Client.Tests.Pages.Shifts
{
    public class DeleteShiftTests
    {
        private readonly Guid TestId = new("72965CE9-4793-4B48-B27F-603B7732F583");
        private readonly Fixture Fixture = new();

        [Fact]
        public void RendersSuccessfully()
        {
            using var context = new TestContext();

            var testShift = Fixture.Build<Shift>()
                .With(s => s.Id, TestId)
                .With(s => s.Date, new DateOnly(2022, 10, 11))
                .With(s => s.StartTime, new TimeOnly(10, 00))
                .With(s => s.EndTime, new TimeOnly(19, 00))
                .Create();

            var mockStore = new Mock<IStore<Shift>>();
            mockStore.Setup(s => s.IsLoading).Returns(false);
            mockStore.Setup(s => s.IsLoadingItem(It.IsAny<Guid>())).Returns(false);
            mockStore.Setup(s => s.Data).Returns(new Dictionary<Guid, Shift> { { TestId, testShift } });

            var mockNavigation = new Mock<NavigationManager>();

            context.Services.AddSingleton(mockNavigation.Object);
            context.Services.AddSingleton(mockStore.Object);
            context.Services.AddBlazorStrap();

            var component = context.RenderComponent<Delete>(p =>
                p.Add(c => c.Id, TestId)
            );

            component.Find("dd").TextContent.Should().Be(testShift.Name);
            mockStore.Verify(s => s.LoadItems(new[] { TestId }, false, default));
        }
    }
}
