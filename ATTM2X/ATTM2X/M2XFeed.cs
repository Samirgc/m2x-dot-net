using System;
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

		public object Details()
		{
			return MakeRequest(String.Empty);
		}

		public object GetLocation()
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

		public object GetStreams()
		{
			return MakeRequest("/streams");
		}

		public M2XStream GetStream(string streamName)
		{
			return new M2XStream(this.APIKey, this.FeedId, streamName);
		}

		public void PostValues(object data)
		{
			MakeRequest(String.Empty, M2XClientMethod.POST, data);
		}

		public object Log()
		{
			return MakeRequest("/log");
		}
	}
}