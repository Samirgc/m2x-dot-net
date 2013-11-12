using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;

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

		public dynamic Details()
		{
			return MakeRequest(String.Empty);
		}

		public dynamic GetValues(DateTime? startDate = null, DateTime? endDate = null, int? limit = null)
		{
			return MakeRequest("/values", queryParams: new
				                                          {
															  start = DateTimeToString(startDate), 
															  end = DateTimeToString(endDate),
															  limit,
				                                          });
		}

		public void PostValues(IEnumerable<M2XPostedValue> postedValues)
		{
			MakeRequest("/values", M2XClientMethod.POST, new
			{
				values = postedValues.Select(v => new { at = DateTimeToString(v.At), value = v.Value }).ToArray()
			});
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