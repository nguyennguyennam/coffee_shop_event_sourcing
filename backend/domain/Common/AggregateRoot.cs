using System;
using System.Collections.Generic;
using System.Reflection;
namespace backend.domain.Common.Event
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _events = new();
        public Guid AggregateId { get; protected set; }
        public int AggregateVersion { get; protected set; } = 0;

        public IEnumerable<Event> GetUncommitedChanges()
        {
            return _events.AsReadOnly();
        }

        public void MarkChangesAsCommited()
        {
            _events.Clear();
        }

        protected void RaiseDomainEvent(Event @event)
        {
            ApplyChange(@event, true);
            _events.Add(@event);
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            var method = this.GetType()
                             .GetMethod(
                                 "Apply",
                                 BindingFlags.Instance 
                                 | BindingFlags.Public 
                                 | BindingFlags.NonPublic,
                                 null,
                                 new Type[] { @event.GetType() },
                                 null);
            if (method == null)
                throw new InvalidOperationException($"The Apply method was not found for {@event.GetType().Name}");

            method.Invoke(this, new object[] { @event });

            if (isNew)
            {
                AggregateVersion++;
                @event.EventVersion = AggregateVersion;
                _events.Add(@event);
            }
        }

        protected void ApplyChange(Event e)
        {
            ApplyChange(e, true);
        }

        public void LoadFromHistory(IEnumerable<Event> history)
        {
            foreach (Event @event in history)
            {
                ApplyChange(@event, false);
                AggregateVersion = @event.EventVersion;
            }
        }
    }
}