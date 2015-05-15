﻿using System.Threading.Tasks;
using NUnit.Framework;

namespace ActIt
{
    [TestFixture]
    public class GivenBuilderForFactoriesTests
    {
        private class TheEvent
        {
        }

        [Test]
        public async Task WhenCreatedTheListenersAreImmutableWhenInstanceWasCreated()
        {
            // track
            var wasCalled = false;

            // arrange
            var builder = new PlotBuilder();
            var story = builder.GenerateStory();

            // act
            builder.Listen<TheEvent>((@event, busSchedule) => wasCalled = true);
            await story.EncounterAsync(new TheEvent());

            // assert
            Assert.That(wasCalled, Is.False);
        }
    }
}
