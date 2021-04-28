using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Grains
{
	public interface IFlightConsumerGrain : IGrainWithGuidKey
	{
		Task CreateFlightAsync(string origin, string destination);
		Task ReserveSeatAsync(string seat, string passengerName);
	}
}
