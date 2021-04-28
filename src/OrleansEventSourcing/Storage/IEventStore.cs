using OrleansEventSourcing.Events;
using OrleansEventSourcing.Grains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Storage
{
	public interface IEventStore
	{
		Task<bool> ApplyUpdatesToStorage(string key, IReadOnlyList<EventBase> updates, int expectedversion);
		Task<KeyValuePair<int, FlightConsumerGrainState>> ReadStateFromStorage(string key);
	}
}
