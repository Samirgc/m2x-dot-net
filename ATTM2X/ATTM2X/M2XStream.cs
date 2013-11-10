using System;
using System.Web;

namespace ATTM2X
{
	//Data Streams:
	//http://api-m2x.att.com/v1/feeds/{id}/streams
	//http://m2x.att.citrusbyte.com/developer/documentation/feed#List-Data-Streams
	public sealed class M2XStream : M2XFeedAsset
	{
		private readonly string streamName;

		internal M2XStream(string apiKey, string feedId, string streamName)
			: base(apiKey, feedId)
		{
			if (String.IsNullOrWhiteSpace(streamName))
				throw new ArgumentException("Invalid streamName - " + streamName);

			this.streamName = streamName;
		}

		public string StreamName
		{
			get { return this.streamName; }
		}

		public void CreateOrUpdate(object data)
		{
			MakeRequest(String.Empty, M2XClientMethod.PUT, data);
		}

		public object Details()
		{
			return MakeRequest(String.Empty);
		}

		public object GetValues(DateTime? startDate = null, DateTime? endDate = null, int? limit = null)
		{
			return MakeRequest("/values", queryParams: new
				                                          {
															  start = startDate, 
															  end = endDate,
															  limit,
				                                          });
		}

		public void PostValues(object data)
		{
			MakeRequest("/values", M2XClientMethod.POST, data);
		}

		public void Delete()
		{
			MakeRequest(String.Empty, M2XClientMethod.DELETE);
		}

		protected override string BuildUrl(string urlPath)
		{
			return base.BuildUrl("/streams/" + HttpUtility.UrlPathEncode(streamName) + urlPath);
		}
	}
}