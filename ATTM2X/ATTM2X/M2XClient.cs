using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Linq;

namespace ATTM2X
{

	public enum M2XVisibility
	{
		Public,
		Private
	}

	public enum M2XFeedType
	{
		Blueprint, 
		Batch,
		DataSource
	}

	public enum M2XFeedLocationDistanceUnitType
	{
		Mi,
		Miles,
		Km
	}

	/// <summary>
	/// Wrapper for AT&T M2X API v. 1.0
	/// https://m2x.att.com/developer/documentation/overview
	/// </summary>
	public sealed class M2XClient : M2XClientBase
	{
		public M2XClient(string apiKey)
			: base(apiKey)
		{}

		//Feeds:
		//http://api-m2x.att.com/v1/feeds
		//https://m2x.att.com/developer/documentation/feed
		public object GetFeeds(string q = null, M2XFeedType? type = null, int? page = null, int? limit = null,
			double? latitude = null, double? longitude = null, double? distance = null, M2XFeedLocationDistanceUnitType? distanceUnit = null)
		{
			return MakeRequest("/feeds", queryParams: new 
												{
													q,
													type = type == null? null : type.Value.ToString().ToLowerInvariant(),
													page,
													limit,
													latitude,
													longitude,
													distance,
													distance_unit = distanceUnit == null ? null : distanceUnit.Value.ToString().ToLowerInvariant()
												});
		}

		public M2XFeed GetFeed(string feedId)
		{
			return new M2XFeed(this.APIKey, feedId);
		}

		//Blueprints:
		//http://api-m2x.att.com/v1/blueprints
		//https://m2x.att.com/developer/documentation/datasource#List-Blueprints
		public object GetBlueprints()
		{
			return MakeRequest("/blueprints");
		}

		public object CreateBlueprint(string name, M2XVisibility visibility, string description = null, string tags = null)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Invalid Blueprint name - " + name);

			return MakeRequest("/blueprints", M2XClientMethod.POST, new {
					                                                        name,
																			visibility = visibility.ToString().ToLowerInvariant(),
																			description,
																			tags
				                                                        });
		}

		public M2XBlueprint GetBlueprint(string blueprintId)
		{
			return new M2XBlueprint(this.APIKey, blueprintId);
		}

		//Batches:
		//http://api-m2x.att.com/v1/batches
		//https://m2x.att.com/developer/documentation/datasource#List-Batches
		public object GetBatches()
		{
			return MakeRequest("/batches");
		}

		public object CreateBatch(string name, M2XVisibility visibility, string description = null, string tags = null)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Invalid Batch name - " + name);

			return MakeRequest("/batches", M2XClientMethod.POST, new {
					                                                    name,
																		visibility = visibility.ToString().ToLowerInvariant(),
																		description,
																		tags
				                                                    });
		}

		public M2XBatch GetBatch(string batchId)
		{
			return new M2XBatch(this.APIKey, batchId);
		}

		//Data Sources:
		//http://api-m2x.att.com/v1/datasources
		//https://m2x.att.com/developer/documentation/datasource#List-Data-Sources
		public object GetDataSources()
		{
			return MakeRequest("/datasources");
		}

		public object CreateDataSource(string name, M2XVisibility visibility, string description = null, string tags = null)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Invalid Data source name - " + name);

			return MakeRequest("/datasources", M2XClientMethod.POST, new {
					                                                        name,
																			visibility = visibility.ToString().ToLowerInvariant(),
																			description,
																			tags
				                                                        });
		}

		public M2XDataSource GetDataSource(string dataSourceId)
		{
			return new M2XDataSource(this.APIKey, dataSourceId);
		}

		//API Keys:
		//http://api-m2x.att.com/v1/keys
		//https://m2x.att.com/developer/documentation/keys
		public object GetKeys(string feedId = null)
		{
			return MakeRequest("/keys", queryParams: String.IsNullOrWhiteSpace(feedId) ? null : new { feed = feedId });
		}

		public object CreateKey(string name, IEnumerable<M2XClientMethod> permissions, string feedId = null, string streamName = null, DateTime? expiresAt = null)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Invalid API Key name - " + name);

			if (permissions == null)
				throw new ArgumentNullException("permissions");

			if (!permissions.Any())
				throw new ArgumentException("Invalid permissions");

			if (!String.IsNullOrWhiteSpace(streamName) && String.IsNullOrWhiteSpace(feedId))
				throw new ArgumentException("Feed id is required in case if stream name provided");

			return MakeRequest("/keys", M2XClientMethod.POST, new {
					                                                name,
																	permissions = permissions.Select(p => p.ToString()),
																	feed = feedId,
																	stream = streamName,
																	expires_at = expiresAt.HasValue ? expiresAt.ToString() : null
				                                                });
		}

		public M2XKey GetKey(string keyId)
		{
			return new M2XKey(this.APIKey, keyId);
		}
	}
}
