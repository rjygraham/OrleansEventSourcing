using Newtonsoft.Json;
using System;

namespace OrleansEventSourcing.Models
{
	public class ReserveSeatModel : ModelBase
	{
		[JsonProperty("id")]
		public Guid Id { get; set; }

		[JsonProperty("seat")]
		public string Seat { get; set; }

		[JsonProperty("passengerName")]
		public string PassengerName { get; set; }
	}
}
