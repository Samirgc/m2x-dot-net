using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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
			path = String.Concat(M2XStream.UrlPath, "/", WebUtility.UrlEncode(this.StreamName), path);
			return this.Device == null
				? this.Distribution.BuildPath(path)
				: this.Device.BuildPath(path);
		}

		/// <summary>
		/// Update the current value of the stream.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Update-Data-Stream-Value
		/// </summary>
		public Task<M2XResponse> UpdateValue(object parms)
		{
			return MakeRequest("/value", M2XClientMethod.PUT, parms);
		}
		/// <summary>
		/// Update the current value of the stream.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Update-Data-Stream-Value
		/// </summary>
		public Task<M2XResponse> UpdateValue(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, "/value", M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// List values from the stream, sorted in reverse chronological order
		/// (most recent values first).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Data-Stream-Values
		/// </summary>
		public Task<M2XResponse> Values(object parms = null)
		{
			return MakeRequest("/values", M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// List values from the stream, sorted in reverse chronological order
		/// (most recent values first).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Data-Stream-Values
		/// </summary>
		public Task<M2XResponse> Values(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, "/values", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Sample values from the stream, sorted in reverse chronological order
		/// (most recent values first).
		///
		/// This method only works for numeric streams
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Data-Stream-Sampling
		/// </summary>
		public Task<M2XResponse> Sampling(object parms)
		{
			return MakeRequest("/sampling", M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Sample values from the stream, sorted in reverse chronological order
		/// (most recent values first).
		///
		/// This method only works for numeric streams
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Data-Stream-Sampling
		/// </summary>
		public Task<M2XResponse> Sampling(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, "/sampling", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Return count, min, max, average and standard deviation stats for the
		/// values of the stream.
		///
		/// This method only works for numeric streams
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Data-Stream-Stats
		/// </summary>
		public Task<M2XResponse> Stats(object parms = null)
		{
			return MakeRequest("/stats", M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Return count, min, max, average and standard deviation stats for the
		/// values of the stream.
		///
		/// This method only works for numeric streams
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Data-Stream-Stats
		/// </summary>
		public Task<M2XResponse> Stats(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, "/stats", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Post timestamped values to an existing data stream.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Post-Data-Stream-Values
		/// </summary>
		public Task<M2XResponse> PostValues(object parms)
		{
			return MakeRequest("/values", M2XClientMethod.POST, parms);
		}
		/// <summary>
		/// Post timestamped values to an existing data stream.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Post-Data-Stream-Values
		/// </summary>
		public Task<M2XResponse> PostValues(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, "/values", M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Delete values in a stream by a date range
		///
		/// https://m2x.com/developer/documentation/v2/device#Delete-Data-Stream-Values
		/// </summary>
		public Task<M2XResponse> DeleteValues(object parms)
		{
			return MakeRequest("/values", M2XClientMethod.DELETE, parms);
		}
		/// <summary>
		/// Delete values in a stream by a date range
		///
		/// https://m2x.com/developer/documentation/v2/device#Delete-Data-Stream-Values
		/// </summary>
		public Task<M2XResponse> DeleteValues(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, "/values", M2XClientMethod.DELETE, parms);
		}
	}
}