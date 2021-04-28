using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Orleans.Hosting;
using Orleans.Runtime;
using OrleansEventSourcing.Grains;
using OrleansEventSourcing.Serialization;
using OrleansEventSourcing.Storage;
using System;

await Host.CreateDefaultBuilder(args)
	.ConfigureAppConfiguration((ctx, builder) =>
	{
		if (ctx.HostingEnvironment.IsDevelopment())
		{
			builder.AddUserSecrets<FlightConsumerGrain>();
		}
	})
	.UseOrleans((ctx, siloBuilder) =>
	{
		siloBuilder
			.UseLocalhostClustering(serviceId: "es", clusterId: "es")
			.AddAzureTableGrainStorage("PubSubStore", options => options.ConnectionString = ctx.Configuration.GetSection("STORAGE_CONNECTIONSTRING").Value)
			.AddEventHubStreams("az-event-hubs", b =>
			{
				b.ConfigureStreamPubSub(Orleans.Streams.StreamPubSubType.ImplicitOnly);
				
				b.ConfigureEventHub(ob => ob.Configure(options =>
				{
					options.ConnectionString = ctx.Configuration.GetSection("EVENTHUB_CONNECTIONSTRING").Value;
					options.ConsumerGroup = "es";
					options.Path = "es";
				}));

				b.UseAzureTableCheckpointer(ob => ob.Configure(options =>
				{
					options.ConnectionString = ctx.Configuration.GetSection("STORAGE_CONNECTIONSTRING").Value;
					options.PersistInterval = TimeSpan.FromSeconds(5);
					
				}));
			})
			.AddCustomStorageBasedLogConsistencyProvider("CustomStorage")
			.ConfigureServices(s =>
			{
				var options = new CosmosDbEventStoreOptions
				{
					AccountEndpoint = ctx.Configuration.GetSection("COSMOSDB_ACCOUNTENDPOINT").Value,
					AccountKey = ctx.Configuration.GetSection("COSMOSDB_ACCOUNTKEY").Value,
					DatabaseId = "orleans",
					ContainerId = "events"
				};

				var jsonSerializerSettings = new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore,
					TypeNameHandling = TypeNameHandling.Auto,
					SerializationBinder = new EventSerializationBinder()
				};

				var cosmosOptions = new CosmosClientOptions
				{ 
					Serializer = new NewtonsoftJsonCosmosSerializer(jsonSerializerSettings),
					EnableContentResponseOnWrite = false 
				};

				var client = new CosmosClient(options.AccountEndpoint, options.AccountKey, cosmosOptions);

				s.AddSingletonNamedService<IEventStore>(nameof(FlightConsumerGrain), (sp, n) => new CosmosDbEventStore(client, options));
			});
	})
	.ConfigureWebHostDefaults(webBuilder =>
	{
		webBuilder
			.ConfigureServices(services => services.AddControllers())
			.Configure((ctx, app) =>
			{
				if (ctx.HostingEnvironment.IsDevelopment())
				{
					app.UseDeveloperExceptionPage();
				}

				app.UseRouting();
				app.UseAuthorization();
				app.UseEndpoints(endpoints =>
				{
					endpoints.MapControllers();
				});
			});
	})
	.ConfigureServices(services => { })
	.RunConsoleAsync();
