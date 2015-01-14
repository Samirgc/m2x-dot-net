using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X Device API
	/// https://m2x.att.com/developer/documentation/v2/device
	/// </summary>
	public sealed class M2XDevice : M2XClass
	{
		public const string UrlPath = "/devices";

		public readonly string DeviceId;

		internal M2XDevice(M2XClient client, string deviceId)
			: base(client)
		{
			if (String.IsNullOrWhiteSpace(deviceId))
				throw new ArgumentException(String.Format("Invalid deviceId - {0}", deviceId));

			this.DeviceId = deviceId;
		}

		internal override string BuildPath(string path)
		{
			return String.Concat(M2XDevice.UrlPath, "/", WebUtility.UrlEncode(this.DeviceId), path);
		}

		/// <summary>
		/// Get location details of an existing Device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Read-Device-Location
		/// </summary>
		public Task<M2XResponse> Location()
		{
			return MakeRequest("/location");
		}
		/// <summary>
		/// Get location details of an existing Device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Read-Device-Location
		/// </summary>
		public Task<M2XResponse> Location(CancellationToken cancellationToken)
		{
			return MakeRequest(cancellationToken, "/location");
		}

		/// <summary>
		/// Update the current location of the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Read-Device-Location
		/// </summary>
		public Task<M2XResponse> UpdateLocation(object parms)
		{
			return MakeRequest("/location", M2XClientMethod.PUT, parms);
		}
		/// <summary>
		/// Update the current location of the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Read-Device-Location
		/// </summary>
		public Task<M2XResponse> UpdateLocation(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, "/location", M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Retrieve list of data streams associated with the device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Data-Streams
		/// </summary>
		public Task<M2XResponse> Streams(object parms = null)
		{
			return MakeRequest(M2XStream.UrlPath, M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Retrieve list of data streams associated with the device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Data-Streams
		/// </summary>
		public Task<M2XResponse> Streams(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, M2XStream.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Get a wrapper to access a data stream associated with the specified Device.
		/// </summary>
		public M2XStream Stream(string streamName)
		{
			return new M2XStream(this, streamName);
		}

		/// <summary>
		/// Post values to multiple streams at once.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Post-Device-Updates--Multiple-Values-to-Multiple-Streams-
		/// </summary>
		public Task<M2XResponse> PostUpdates(object parms)
		{
			return MakeRequest("/updates", M2XClientMethod.POST, parms);
		}
		/// <summary>
		/// Post values to multiple streams at once.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Post-Device-Updates--Multiple-Values-to-Multiple-Streams-
		/// </summary>
		public Task<M2XResponse> PostUpdates(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, "/updates", M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Retrieve list of triggers associated with the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Triggers
		/// </summary>
		public Task<M2XResponse> Triggers(object parms = null)
		{
			return MakeRequest(M2XTrigger.UrlPath, M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Retrieve list of triggers associated with the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Triggers
		/// </summary>
		public Task<M2XResponse> Triggers(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, M2XTrigger.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new trigger associated with the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Trigger
		/// </summary>
		public Task<M2XResponse> CreateTrigger(object parms)
		{
			return MakeRequest(M2XTrigger.UrlPath, M2XClientMethod.POST, parms);
		}
		/// <summary>
		/// Create a new trigger associated with the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Trigger
		/// </summary>
		public Task<M2XResponse> CreateTrigger(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, M2XTrigger.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access a trigger associated with the specified Device.
		/// </summary>
		public M2XTrigger Trigger(string triggerId)
		{
			return new M2XTrigger(this, triggerId);
		}

		/// <summary>
		/// Retrieve list of HTTP requests received lately by the specified device (up to 100 entries).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#View-Request-Log
		/// </summary>
		public Task<M2XResponse> Log(object parms = null)
		{
			return MakeRequest("/log", M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Retrieve list of HTTP requests received lately by the specified device (up to 100 entries).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#View-Request-Log
		/// </summary>
		public Task<M2XResponse> Log(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, "/log", M2XClientMethod.GET, parms);
		}
	}
}
