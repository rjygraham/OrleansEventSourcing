using Orleans;
using OrleansEventSourcing.Models;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Grains
{
	public interface IFlightProducerGrain : IGrainWithGuidKey
	{
		Task CreateFlightAsync(CreateFlightModel model);

		Task ReserveSeatAsync(ReserveSeatModel model);

		Task StopRervationsAsync();
	}
}