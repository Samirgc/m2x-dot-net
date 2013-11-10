using System;
using System.Web;

namespace ATTM2X
{
	//Batches:
	//http://api-m2x.att.com/v1/batches
	//https://m2x.att.com/developer/documentation/datasource#List-Batches
	public sealed class M2XBatch : M2XClientBase
	{
		private readonly string batchId;

		internal M2XBatch(string apiKey, string batchId)
			: base(apiKey)
		{
			if (String.IsNullOrWhiteSpace(batchId))
				throw new ArgumentException("Invalid batchId - " + batchId);

			this.batchId = batchId;
		}

		public string BatchId
		{
			get { return this.batchId; }
		}

		public dynamic Details()
		{
			return MakeRequest(String.Empty);
		}

		public void Update(string name, M2XVisibility visibility, string description = null, string tags = null)
		{
			MakeRequest(String.Empty, M2XClientMethod.PUT, new {
					                                                name,
																	visibility = visibility.ToString().ToLowerInvariant(),
																	description,
																	tags
				                                                });
		}

		public dynamic GetDataSources()
		{
			return MakeRequest("/datasources");
		}

		public dynamic AddDataSource(string dataSourceSerialNumber)
		{
			return MakeRequest("/datasources", M2XClientMethod.POST, new { serial = dataSourceSerialNumber });
		}

		public void Delete()
		{
			MakeRequest(String.Empty, M2XClientMethod.DELETE);
		}

		protected override string BuildUrl(string urlPath)
		{
			return base.BuildUrl("/batches/" + HttpUtility.UrlPathEncode(batchId) + urlPath);
		}
	}
}