using System;
using System.Web;

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
			return String.Concat(M2XDevice.UrlPath, "/", HttpUtility.UrlPathEncode(this.DeviceId), path);
		}

		/// <summary>
		/// Update an existing Device's information.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Update-Device-Details
		/// </summary>
		public M2XResponse Update(object parms)
		{
			return MakeRequest(null, M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Get details of an existing Device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#View-Device-Details
		/// </summary>
		public M2XResponse Details()
		{
			return MakeRequest();
		}

		/// <summary>
		/// Get location details of an existing Device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Read-Device-Location
		/// </summary>
		public M2XResponse Location()
		{
			return MakeRequest("/location");
		}

		/// <summary>
		/// Update the current location of the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Read-Device-Location
		/// </summary>
		public M2XResponse UpdateLocation(object parms)
		{
			return MakeRequest("/location", M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Retrieve list of data streams associated with the device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Data-Streams
		/// </summary>
		public M2XResponse Streams(object parms = null)
		{
			return MakeRequest(M2XStream.UrlPath, M2XClientMethod.GET, parms);
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
		public M2XResponse PostUpdates(object parms)
		{
			return MakeRequest("/updates", M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Retrieve list of triggers associated with the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Triggers
		/// </summary>
		public M2XResponse Triggers(object parms = null)
		{
			return MakeRequest(M2XTrigger.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new trigger associated with the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Trigger
		/// </summary>
		public M2XResponse CreateTrigger(object parms)
		{
			return MakeRequest(M2XTrigger.UrlPath, M2XClientMethod.POST, parms);
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
		public M2XResponse Log(object parms = null)
		{
			return MakeRequest("/log", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Delete an existing device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Delete-Device
		/// </summary>
		public M2XResponse Delete()
		{
			return MakeRequest(null, M2XClientMethod.DELETE);
		}
	}
}
