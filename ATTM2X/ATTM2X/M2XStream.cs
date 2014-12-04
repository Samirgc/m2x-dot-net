using System;
using System.Web;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X Data Streams API
	/// https://m2x.att.com/developer/documentation/v2/device
	/// https://m2x.att.com/developer/documentation/v2/distribution
	/// </summary>
	public sealed class M2XStream : M2XClass
	{
		public const string UrlPath = "/streams";

		public readonly string StreamName;
		public readonly M2XDevice Device;
		public readonly M2XDistribution Distribution;

		private M2XStream(M2XClient client, string streamName)
			: base(client)
		{
			if (String.IsNullOrWhiteSpace(streamName))
				throw new ArgumentException(String.Format("Invalid streamName - {0}", streamName));

			this.StreamName = streamName;
		}
		internal M2XStream(M2XDevice device, string streamName)
			: this(device.Client, streamName)
		{
			this.Device = device;
		}
		internal M2XStream(M2XDistribution distribution, string streamName)
			: this(distribution.Client, streamName)
		{
			this.Distribution = distribution;
		}

		internal override string BuildPath(string path)
		{
			path = String.Concat(M2XStream.UrlPath, "/", HttpUtility.UrlPathEncode(this.StreamName), path);
			return this.Device == null
				? this.Distribution.BuildPath(path)
				: this.Device.BuildPath(path);
		}

		/// <summary>
		/// Update a data stream associated with the Device or specified distribution
		/// (if a stream with this name does not exist it gets created).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Update-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#Create-Update-Data-Stream
		/// </summary>
		public void CreateOrUpdate(object parms)
		{
			MakeRequest(null, M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Update the current value of the stream.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Update-Data-Stream-Value
		/// </summary>
		public void UpdateValue(object parms)
		{
			MakeRequest("/value", M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Get details of a specific data Stream associated with an existing device or distribution.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#View-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#View-Data-Stream
		/// </summary>
		public M2XResponse Details()
		{
			return MakeRequest();
		}

		/// <summary>
		/// List values from the stream, sorted in reverse chronological order
		/// (most recent values first).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Data-Stream-Values
		/// </summary>
		public M2XResponse Values(object parms = null)
		{
			return MakeRequest("/values", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Sample values from the stream, sorted in reverse chronological order
		/// (most recent values first).
		///
		/// This method only works for numeric streams
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Data-Stream-Sampling
		/// </summary>
		public M2XResponse Sampling(object parms)
		{
			return MakeRequest("/sampling", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Return count, min, max, average and standard deviation stats for the
		/// values of the stream.
		///
		/// This method only works for numeric streams
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Data-Stream-Stats
		/// </summary>
		public M2XResponse Stats(object parms = null)
		{
			return MakeRequest("/stats", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Post multiple values to the stream
		///
		/// The 'values' parameter is an array with the following format:
		///
		///  [
		///    { "timestamp": <Time in ISO8601>, "value": x },
		///    { "timestamp": <Time in ISO8601>, "value": y },
		///    [ ... ]
		///  ]
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Post-Data-Stream-Values
		/// </summary>
		public M2XResponse PostValues(object values)
		{
			return MakeRequest("/values", M2XClientMethod.POST, new { values });
		}

		/// <summary>
		/// Delete values in a stream by a date range
		///
		/// https://m2x.com/developer/documentation/v2/device#Delete-Data-Stream-Values
		/// </summary>
		public M2XResponse DeleteValues(object parms)
		{
			return MakeRequest("/values", M2XClientMethod.DELETE, parms);
		}

		/// <summary>
		/// Delete an existing data stream associated with a specific device or distribution.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Delete-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#Delete-Data-Stream
		/// </summary>
		public M2XResponse Delete()
		{
			return MakeRequest(null, M2XClientMethod.DELETE);
		}
	}
}