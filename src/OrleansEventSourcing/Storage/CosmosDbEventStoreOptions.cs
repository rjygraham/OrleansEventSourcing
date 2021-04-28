using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansEventSourcing.Storage
{
	public class CosmosDbEventStoreOptions
	{
		public string AccountEndpoint { get; set; }
		public string AccountKey { get; set; }
		public string DatabaseId { get; set; }
		public string ContainerId { get; set; }
	}
}
