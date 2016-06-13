using System;
using System.Net;
using System.Threading.Tasks;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X Device API
	/// https://m2x.att.com/developer/documentation/v2/device
	/// </summary>
	public sealed class M2XDevice : M2XClassWithMetadata
	{
		public const string UrlPath = "/devices";

		public readonly string DeviceId;
		public readonly string Serial;

		internal M2XDevice(M2XClient client, string deviceId, string serial)
			: base(client)
		{
			if (String.IsNullOrWhiteSpace(deviceId) && String.IsNullOrWhiteSpace(serial))
				throw new ArgumentException(String.Format("Invalid deviceId - {0}", deviceId));

			this.DeviceId = deviceId;
			this.Serial = serial;
		}

		internal override string BuildPath(string path)
		{
			return String.IsNullOrWhiteSpace(this.DeviceId)
				? String.Concat(M2XDevice.UrlPath, "/serial/", WebUtility.UrlEncode(this.Serial), path)
				: String.Concat(M2XDevice.UrlPath, "/", WebUtility.UrlEncode(this.DeviceId), path);
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
		/// Update the current location of the specified device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Read-Device-Location
		/// </summary>
		public Task<M2XResponse> UpdateLocation(object parms)
		{
			return MakeRequest("/location", M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Get location details of an existing Device.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Read-Device-Location
		/// </summary>
		public Task<M2XResponse> LocationHistory()
		{
			return MakeRequest("/location/waypoints");
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
		/// Get a wrapper to access a data stream associated with the specified Device.
		/// </summary>
		public M2XStream Stream(string streamName)
		{
			return new M2XStream(this, streamName);
		}

		/// <summary>
		/// List values from all data streams associated with a specific device, sorted in reverse chronological order (most recent values first).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Values-from-all-Data-Streams-of-a-Device
		/// </summary>
		public Task<M2XResponse> Values(object parms = null, string format = null)
		{
			string path = "/values";
			if (!String.IsNullOrEmpty(format))
				path += "." + format;
			return MakeRequest(path, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Search and list values from all data streams associated with a specific device, sorted in reverse chronological order.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Search-Values-from-all-Data-Streams-of-a-Device
		/// </summary>
		public Task<M2XResponse> SearchValues(object parms, string format = null)
		{
			string path = "/values/search";
			if (!String.IsNullOrEmpty(format))
				path += "." + format;
			return MakeRequest(path, M2XClientMethod.POST, null, parms);
		}

		/// <summary>
		/// Export all values from all or selected data streams associated with a specific device, sorted in reverse chronological order (most recent values first).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Export-Values-from-all-Data-Streams-of-a-Device
		/// </summary>
		public Task<M2XResponse> ExportValues(object parms = null)
		{
			return MakeRequest("/values/export.csv", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Posts single values to multiple streams at once.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Post-Device-Update--Single-Values-to-Multiple-Streams-
		/// </summary>
		public Task<M2XResponse> PostUpdate(object parms)
		{
			return MakeRequest("/update", M2XClientMethod.POST, parms);
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
		/// Retrieve list of HTTP requests received lately by the specified device (up to 100 entries).
		///
		/// https://m2x.att.com/developer/documentation/v2/device#View-Request-Log
		/// </summary>
		public Task<M2XResponse> Log(object parms = null)
		{
			return MakeRequest("/log", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Retrieve the list of recent commands sent to the current device (as given by the API key).
		///
		/// https://m2x.att.com/developer/documentation/v2/commands#Device-s-List-of-Received-Commands
		/// </summary>
		public Task<M2XResponse> Commands(object parms = null)
		{
			return MakeRequest("/commands", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Get details of a received command including the delivery information for this device.
		///
		/// https://m2x.att.com/developer/documentation/v2/commands#Device-s-View-of-Command-Details
		/// </summary>
		public Task<M2XResponse> CommandDetails(string commandId)
		{
			return MakeRequest("/commands/" + WebUtility.UrlEncode(commandId));
		}

		/// <summary>
		/// Mark the given command as processed by the device, changing the status from "sent" to "processed".
		///
		/// https://m2x.att.com/developer/documentation/v2/commands#Device-Marks-a-Command-as-Processed
		/// </summary>
		public Task<M2XResponse> ProcessCommand(string commandId, object parms = null)
		{
			return MakeRequest(String.Concat("/commands/", WebUtility.UrlEncode(commandId), "/process"), M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Mark the given command as rejected by the device, changing the status from "sent" to "rejected".
		///
		/// https://m2x.att.com/developer/documentation/v2/commands#Device-Marks-a-Command-as-Rejected
		/// </summary>
		public Task<M2XResponse> RejectCommand(string commandId, object parms = null)
		{
			return MakeRequest(String.Concat("/commands/", WebUtility.UrlEncode(commandId), "/reject"), M2XClientMethod.POST, parms);
		}
	}
}
