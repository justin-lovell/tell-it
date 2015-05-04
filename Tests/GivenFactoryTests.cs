﻿using NUnit.Framework;

namespace TellIt
{
    [TestFixture]
    public class GivenFactoryTests
    {
        private class TheEvent
        {
        }

        [Test]
        public void WhenCreatingNestedBuilderItWillFirePreviousSubscriptions()
        {
            // track
            var wasCalled1 = false;

            // arrange
            var builder = new PlotBuilder();
            builder.Listen<TheEvent>((@event, s) => wasCalled1 = true);

            var factory = builder.GenerateStory();

            var builder2 = factory.CreateNestedBuilder();
            var factory2 = builder2.GenerateStory();

            // act
            var schedule = factory2.CreateNewSchedule();
            schedule.Encounter(new TheEvent());

            // assert
            Assert.That(wasCalled1, Is.True);
        }
    }
}
