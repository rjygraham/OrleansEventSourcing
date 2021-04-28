using Newtonsoft.Json;
using System;

namespace OrleansEventSourcing.Events
{
	[Serializable]
	public class SeatReserved : EventBase
	{
		[JsonProperty("seat")]
		public string Seat { get; set; }

		[JsonProperty("passengerName")]
		public string PassengerName { get; set; }

		public SeatReserved()
		{
			Type = nameof(SeatReserved);
		}
	}
}
