using Bogus;
using Orleans;
using Orleans.Streams;
using OrleansEventSourcing.Models;
using System;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Grains
{
	public class FlightProducerGrain : Grain, IFlightProducerGrain
	{
		private IAsyncStream<ModelBase> stream;
		private IDisposable timer;
		private Faker<ReserveSeatModel> faker;

		public override Task OnActivateAsync()
		{
			faker = GetFaker(GrainReference.GrainIdentity.PrimaryKey);

			// Get the stream
			this.stream = base
				.GetStreamProvider("az-event-hubs")
				.GetStream<ModelBase>(GrainReference.GrainIdentity.PrimaryKey, "flights");

			return Task.CompletedTask;
		}

		public async Task CreateFlightAsync(CreateFlightModel model)
		{
			if (this.timer != null)
			{
				throw new InvalidOperationException("This grain is already producing events");
			}

			await stream.OnNextAsync(model);

			var period = TimeSpan.FromSeconds(1);
			this.timer = RegisterTimer(TimerTick, null, period, period);
		}

		public async Task ReserveSeatAsync(ReserveSeatModel model)
		{
			await stream.OnNextAsync(model);
		}

		public Task StopRervationsAsync()
		{
			if (this.stream != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}

			return Task.CompletedTask;
		}

		private async Task TimerTick(object _)
		{
			var reservation = faker.Generate();
			await this.stream.OnNextAsync(reservation);
		}

		private static Faker<ReserveSeatModel> GetFaker(Guid id)
		{
			return new Faker<ReserveSeatModel>()
			.RuleFor(r => r.Id, (f, u) => id)
			.RuleFor(r => r.PassengerName, (f, u) => f.Name.FullName())
			.RuleFor(r => r.Seat, (f, u) => $"{f.Random.Number(1, 34)}{f.Random.Char('A', 'F')}");
		}
	}
}
