using System;
using System.Web;

namespace ATTM2X
{
	//Blueprints:
	//http://api-m2x.att.com/v1/blueprints
	//https://m2x.att.com/developer/documentation/datasource#List-Blueprints
	public sealed class M2XBlueprint : M2XClientBase
	{
		private readonly string blueprintId;

		internal M2XBlueprint(string apiKey, string blueprintId)
			: base(apiKey)
		{
			if (String.IsNullOrWhiteSpace(blueprintId))
				throw new ArgumentException("Invalid blueprintId - " + blueprintId);

			this.blueprintId = blueprintId;
		}

		public string BlueprintId
		{
			get { return this.blueprintId; }
		}

		public M2XFeed GetFeed()
		{
			return new M2XFeed(this.APIKey, this.BlueprintId);
		}

		public dynamic Details()
		{
			return MakeRequest(String.Empty);
		}

		public void Update(string name, M2XVisibility visibility, string description = null)
		{
			MakeRequest(String.Empty, M2XClientMethod.PUT, new {
					                                                name,
																	visibility = visibility.ToString().ToLowerInvariant(),
																	description
				                                                });
		}

		public void Delete()
		{
			MakeRequest(String.Empty, M2XClientMethod.DELETE);
		}

		protected override string BuildUrl(string urlPath)
		{
			return base.BuildUrl("/blueprints/" + HttpUtility.UrlPathEncode(blueprintId) + urlPath);
		}
	}
}