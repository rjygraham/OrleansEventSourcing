using Newtonsoft.Json;
using System;

namespace OrleansEventSourcing.Events
{
	[Serializable]
	public abstract class EventBase
	{
		[JsonProperty("$type", Order = -10)]
		public string Type { get; set; }

		[JsonProperty("version")]
		public int Version { get; set; }
	}
}
