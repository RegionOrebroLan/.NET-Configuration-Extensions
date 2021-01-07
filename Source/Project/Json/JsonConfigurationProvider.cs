using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.Json;

namespace RegionOrebroLan.Configuration.Json
{
	[CLSCompliant(false)]
	public class JsonConfigurationProvider : Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider
	{
		#region Constructors

		public JsonConfigurationProvider() : this(new JsonConfigurationSource()) { }
		public JsonConfigurationProvider(JsonConfigurationSource source) : base(source) { }

		#endregion

		#region Properties

		public new virtual IDictionary<string, string> Data => base.Data;

		#endregion
	}
}