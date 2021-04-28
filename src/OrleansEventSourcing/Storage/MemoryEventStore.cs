using OrleansEventSourcing.Events;
using OrleansEventSourcing.Grains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Storage
{
	public class MemoryEventStore : IEventStore
	{
		private readonly IDictionary<string, IList<EventBase>> store;

		public MemoryEventStore()
		{
			store = new Dictionary<string, IList<EventBase>>();
		}

		public Task<bool> ApplyUpdatesToStorage(string key, IReadOnlyList<EventBase> updates, int expectedversion)
		{
			IList<EventBase> events;

			if (!store.TryGetValue(key, out events))
			{
				events = new List<EventBase>();
				store.Add(key, events);
			}

			if (events.Count > 0 && events.Max(m => m.Version) != expectedversion)
			{
				return Task.FromResult(false);
			}

			foreach (var update in updates)
			{
				update.Version = ++expectedversion;
				events.Add(update);
			}

			return Task.FromResult(true);
		}

		public Task<KeyValuePair<int, FlightConsumerGrainState>> ReadStateFromStorage(string key)
		{
			var version = 0;
			IList<EventBase> events;

			if (!store.TryGetValue(key, out events))
			{
				events = new List<EventBase>();
				store.Add(key, events);
			}

			if (events.Count > 0)
			{
				version = events.Max(m => m.Version);
			}

			return Task.FromResult(new KeyValuePair<int, FlightConsumerGrainState>(version, new FlightConsumerGrainState(events)));
		}
	}
}
