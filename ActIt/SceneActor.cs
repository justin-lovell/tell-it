﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActIt
{
    public sealed class SceneActor
    {
        private readonly StoryContext _context;
        private readonly IEnumerable<Listener> _listeners;

        internal SceneActor(IEnumerable<Listener> listeners, StoryContext context)
        {
            _listeners = listeners;
            _context = context;
        }

        public T Context<T>() where T : new()
        {
            return _context.GetCurrentInstanceOrCreateNew<T>();
        }

        public Task Interrupt<TEvent>(TEvent theEvent, Action<ReplayNotificationHub> tapCallback)
        {
            if (tapCallback == null)
            {
                return Interrupt(theEvent);
            }

            var historicalEvents = new List<object>();
            var listeners = PipeEventsToHistoryRecounter(theEvent, historicalEvents);

            var nestedScene = new SceneActor(listeners, _context);

            return nestedScene.Interrupt(theEvent)
                              .ContinueWith(task =>
                              {
                                  var replayHub = new ReplayNotificationHub(historicalEvents);
                                  tapCallback(replayHub);
                              });
        }

        private IEnumerable<Listener> PipeEventsToHistoryRecounter<TEvent>(
            TEvent theEvent,
            List<object> eventsThatOccurred)
        {
            Listener temporaryListener = (@event, actor) =>
            {
                if (!ReferenceEquals(theEvent, @event))
                {
                    eventsThatOccurred.Add(@event);
                }

                return TaskEx.IntoTaskResult<object>(null);
            };
            var listeners = _listeners.Concat(new[] {temporaryListener});
            return listeners;
        }

        public Task Interrupt<TEvent>(TEvent theEvent)
        {
            var tasks = from listener in _listeners
                        select listener(theEvent, this);

            return TaskEx.WhenAll(tasks);
        }
    }
}
