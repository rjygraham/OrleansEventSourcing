using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Events
{
	[Serializable]
	public class FlightCreated : EventBase
	{
		[JsonProperty("origin")]
		public string Origin { get; set; }

		[JsonProperty("destination")]
		public string Destination { get; set; }

		public FlightCreated()
		{
			Type = nameof(FlightCreated);
		}
	}
}
