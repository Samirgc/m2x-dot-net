using System;
using System.Web;

namespace ATTM2X
{
	//Data Sources:
	//http://api-m2x.att.com/v1/datasources
	//https://m2x.att.com/developer/documentation/datasource#List-Data-Sources
	public sealed class M2XDataSource : M2XClientBase
	{
		private readonly string dataSourceId;

		internal M2XDataSource(string apiKey, string dataSourceId)
			: base(apiKey)
		{
			if (String.IsNullOrWhiteSpace(dataSourceId))
				throw new ArgumentException("Invalid dataSourceId - " + dataSourceId);

			this.dataSourceId = dataSourceId;
		}

		public string DataSourceId
		{
			get { return this.dataSourceId; }
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
			return base.BuildUrl("/datasources/" + HttpUtility.UrlPathEncode(dataSourceId) + urlPath);
		}
	}
}