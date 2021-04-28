using Newtonsoft.Json;
using OrleansEventSourcing.Events;
using System;

namespace OrleansEventSourcing.Entities
{
	[Serializable]
	public class EventEntity
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("pk")]
		public string PartitionKey { get; set; }

		[JsonProperty("event")]
		public EventBase Event { get; set; }
	}
}
