using OrleansEventSourcing.Events;
using System;
using System.Collections.Generic;

namespace OrleansEventSourcing.Grains
{
	[Serializable]
	public class FlightConsumerGrainState
	{
		public string Origin { get; set; }
		public string Destination { get; set; }
		public HashSet<SeatReserved> SeatReservations { get; set; } = new HashSet<SeatReserved>();

		public FlightConsumerGrainState()
		{
		}

		public FlightConsumerGrainState(IEnumerable<EventBase> previousEvents)
		{
			foreach (var previousEvent in previousEvents)
			{
				switch (previousEvent)
				{
					case FlightCreated flightCreated:
						Apply(flightCreated);
						break;
					case SeatReserved seatReserved:
						Apply(seatReserved);
						break;
					default:
						break;
				}
			}
		}

		public void Apply(FlightCreated @event)
		{
			Origin = @event.Origin;
			Destination = @event.Destination;
		}

		public void Apply(SeatReserved @event)
		{
			SeatReservations.Add(@event);
		}
	}
}
