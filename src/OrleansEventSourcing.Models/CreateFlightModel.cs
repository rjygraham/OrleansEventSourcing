using Newtonsoft.Json;

namespace OrleansEventSourcing.Models
{
	public class CreateFlightModel : ModelBase
	{
		[JsonProperty("origin")]
		public string Origin { get; set; }

		[JsonProperty("destination")]
		public string Destination { get; set; }
	}
}
