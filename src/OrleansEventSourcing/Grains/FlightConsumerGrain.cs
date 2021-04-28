using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Streams.Core;
using OrleansEventSourcing.Events;
using OrleansEventSourcing.Models;
using OrleansEventSourcing.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Grains
{
	[LogConsistencyProvider(ProviderName = "CustomStorage")]
	[ImplicitStreamSubscription("flights")]
	public class FlightConsumerGrain : 
		JournaledGrain<FlightConsumerGrainState, EventBase>, 
		IFlightConsumerGrain,
		ICustomStorageInterface<FlightConsumerGrainState, EventBase>,
		IStreamSubscriptionObserver,
		IAsyncObserver<ModelBase>
	{
		private readonly IEventStore eventStore;

		public FlightConsumerGrain(ISiloHost siloHost)
		{
			eventStore = siloHost.Services.GetRequiredServiceByName<IEventStore>(nameof(FlightConsumerGrain));
		}

		public override async Task OnActivateAsync()
		{
			await RefreshNow();
		}

		public async Task CreateFlightAsync(string origin, string destination)
		{
			RaiseEvent(new FlightCreated { Origin = origin, Destination = destination });
			await ConfirmEvents();
		}

		public async Task ReserveSeatAsync(string seat, string passengerName)
		{
			RaiseEvent(new SeatReserved { Seat = seat, PassengerName = passengerName });
			await ConfirmEvents();
		}

		public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
		{
			var handle = handleFactory.Create<ModelBase>();
			await handle.ResumeAsync(this);
		}

		public async Task OnNextAsync(ModelBase item, StreamSequenceToken token = null)
		{
			switch (item)
			{
				case CreateFlightModel createFlightModel:
					RaiseEvent(new FlightCreated { Origin = createFlightModel.Origin, Destination = createFlightModel.Destination });
					break;
				case ReserveSeatModel reserveSeatModel:
					RaiseEvent(new SeatReserved { Seat = reserveSeatModel.Seat, PassengerName = reserveSeatModel.PassengerName });
					break;
				default:
					break;
			}
			
			await ConfirmEvents();
		}

		public async Task OnCompletedAsync()
		{
			
		}

		public async Task OnErrorAsync(Exception ex)
		{
			
		}

		public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<EventBase> updates, int expectedversion)
		{
			return await eventStore.ApplyUpdatesToStorage(GrainReference.GrainIdentity.PrimaryKey.ToString(), updates, expectedversion);
		}

		public async Task<KeyValuePair<int, FlightConsumerGrainState>> ReadStateFromStorage()
		{
			return await eventStore.ReadStateFromStorage(GrainReference.GrainIdentity.PrimaryKey.ToString());
		}
	}
}
