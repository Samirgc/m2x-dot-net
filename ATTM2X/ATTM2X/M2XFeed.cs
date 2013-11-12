using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATTM2X
{
	public abstract class M2XFeedAsset : M2XClientBase
	{
		private readonly string feedId;

		internal M2XFeedAsset(string apiKey, string feedId)
			: base(apiKey)
		{
			if (String.IsNullOrWhiteSpace(feedId))
				throw new ArgumentException("Invalid feedId - " + feedId);

			this.feedId = feedId;
		}

		public string FeedId
		{
			get { return feedId; }
		}

		protected override string BuildUrl(string urlPath)
		{
			return base.BuildUrl("/feeds/" + HttpUtility.UrlPathEncode(feedId) + urlPath);
		}
	}

	//Feeds:
	//http://api-m2x.att.com/v1/feeds
	//https://m2x.att.com/developer/documentation/feed
	public sealed class M2XFeed : M2XFeedAsset
	{
		internal M2XFeed(string apiKey, string feedId)
			: base(apiKey, feedId)
		{}

		public dynamic Details()
		{
			return MakeRequest(String.Empty);
		}

		public dynamic GetLocation()
		{
			return MakeRequest("/location");
		}

		public void UpdateLocation(double latitude, double longitude, string name = null, double? elevation = null)
		{
			MakeRequest("/location", M2XClientMethod.PUT, new
				                                              {
					                                              name,
																  latitude,
																  longitude,
																  elevation
				                                              });
		}

		public dynamic GetStreams()
		{
			return MakeRequest("/streams");
		}

		public M2XStream GetStream(string streamName)
		{
			return new M2XStream(this.APIKey, this.FeedId, streamName);
		}

//TODO find out why it doesn't work
//http://m2x.att.citrusbyte.com/developer/documentation/feed#Post-Multiple-Values-to-Multiple-Streams
/*
 POST http://api-m2x.att.com/v1/feeds/2423e3adc7f8fa5824a7fa311cbc415a HTTP/1.1
Content-Type: application/json
X-M2X-KEY: 8593b7e8dd4081e71e63d5422ae8b791
User-Agent: m2x-dot-net-client
Host: api-m2x.att.com
Content-Length: 173

{"values":{"test1":[{"at":"2013-11-11T19:05:23Z","value":"15"}],"test2":[{"at":"2013-11-11T19:05:23Z","value":"117"}],"test3":[{"at":"2013-11-11T19:05:23Z","value":"756"}]}} 
 */
		public void PostValues(IEnumerable<M2XStreamPostedValues> postedValues)
		{
			MakeRequest(String.Empty, M2XClientMethod.POST, new
			{
				values = postedValues.ToDictionary(pair => pair.StreamName, 
								pair => pair.PostedValues.Select(v => new {
																	at = DateTimeToString(v.At),
																	value = v.Value
																}).ToArray())
			});
		}

		public dynamic Log()
		{
			return MakeRequest("/log");
		}
	}
}