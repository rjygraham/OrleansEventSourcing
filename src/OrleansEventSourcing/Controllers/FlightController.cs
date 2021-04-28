using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using OrleansEventSourcing.Grains;
using OrleansEventSourcing.Models;
using System;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class FlightController : ControllerBase
	{
		private readonly IGrainFactory grainFactory;
		private readonly ILogger<FlightController> logger;

		public FlightController(IGrainFactory grainFactory, ILogger<FlightController> logger)
		{
			this.grainFactory = grainFactory;
			this.logger = logger;
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] CreateFlightModel model)
		{
			// Validate model.

			// Get Grain and process event.
			var id = Guid.NewGuid();

			var flight = grainFactory.GetGrain<IFlightProducerGrain>(id);
			await flight.CreateFlightAsync(model);

			return CreatedAtRoute(new { id = id }, null);
		}

		[HttpPost("{id}/reserve")]
		public async Task<IActionResult> PostReserveSeat(Guid id, [FromBody] ReserveSeatModel model)
		{
			// Validate model.

			var flight = grainFactory.GetGrain<IFlightProducerGrain>(id);
			await flight.ReserveSeatAsync(model);

			return Ok();
		}

		[HttpPost("{id}/stop")]
		public async Task<IActionResult> PostStop(Guid id)
		{
			// Validate model.

			var flight = grainFactory.GetGrain<IFlightProducerGrain>(id);
			await flight.StopRervationsAsync();

			return Ok();
		}
	}
}
