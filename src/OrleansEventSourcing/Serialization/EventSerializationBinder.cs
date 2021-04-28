using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace OrleansEventSourcing.Serialization
{
	public class EventSerializationBinder : ISerializationBinder
	{
		private readonly Assembly eventsAssembly;

		public EventSerializationBinder()
		{
			eventsAssembly = typeof(EventSerializationBinder).Assembly;
		}

		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			_ = serializedType ?? throw new ArgumentNullException(nameof(serializedType));

			typeName = serializedType.Name;
			assemblyName = "";
		}

		public Type BindToType(string assemblyName, string typeName)
		{
			return eventsAssembly.GetType($"OrleansEventSourcing.Events.{typeName}");
		}
	}
}
