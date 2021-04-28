using Microsoft.Azure.Cosmos;
using OrleansEventSourcing.Entities;
using OrleansEventSourcing.Events;
using OrleansEventSourcing.Grains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Storage
{
	public class CosmosDbEventStore : IEventStore
	{
		private readonly Container container;

		public CosmosDbEventStore(CosmosClient cosmosClient, CosmosDbEventStoreOptions options)
		{
			var database = cosmosClient.GetDatabase(options.DatabaseId);
			container = database.GetContainer(options.ContainerId);
		}

		public async Task<bool> ApplyUpdatesToStorage(string key, IReadOnlyList<EventBase> updates, int expectedversion)
		{
			var partitionKey = new PartitionKey(key);

			foreach (var update in updates)
			{
				update.Version = ++expectedversion;

				var entity = new EventEntity
				{
					Id = Guid.NewGuid().ToString(),
					PartitionKey = key,
					Event = update
				};

				try
				{
					await container.CreateItemAsync(entity, partitionKey);
				}
				catch (Exception ex)
				{
					// swallow
				}
			}

			return true;
		}

		public async Task<KeyValuePair<int, FlightConsumerGrainState>> ReadStateFromStorage(string key)
		{
			var query = new QueryDefinition("SELECT * FROM grains g where g.pk = @partitionKey").WithParameter("@partitionKey", key);

			var events = new List<EventEntity>();
			var iterator = container.GetItemQueryIterator<EventEntity>(query, requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(key) });
			
			while (iterator.HasMoreResults)
			{
				var response = await iterator.ReadNextAsync();
				events.AddRange(response);
			}

			var version = events.Count == 0
				? 0
				: events.Max(m => m.Event.Version);

			var state = new FlightConsumerGrainState(events.Select(s => s.Event));

			return new KeyValuePair<int, FlightConsumerGrainState>(version, state);
		}
	}
}
