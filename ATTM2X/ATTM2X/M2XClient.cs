using System;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X API
	/// https://m2x.att.com/developer/documentation/v2/overview
	/// </summary>
	public sealed class M2XClient
	{
		public const string ApiEndPointSecure = "https://api-m2x.att.com/v2";
		public const string ApiEndPoint = "http://api-m2x.att.com/v2";

		private static readonly string UserAgent;

		public readonly string APIKey;
		public readonly string EndPoint;

		/// <summary>
		/// The last API call response
		/// </summary>
		public M2XResponse LastResponse { get; private set; }

		static M2XClient()
		{
			string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			string langVersion = Environment.Version.ToString();
			string osVersion = Environment.OSVersion.ToString();
			UserAgent = String.Format("M2X-.NET/{0} .NETFramework/{1} ({2})", version, langVersion, osVersion);
		}

		public M2XClient(string apiKey, string m2xApiEndPoint = ApiEndPoint)
		{
			if (String.IsNullOrWhiteSpace(m2xApiEndPoint))
				throw new ArgumentException("Invalid API end point url");

			this.APIKey = apiKey;
			this.EndPoint = m2xApiEndPoint;
		}

		// Device API

		/// <summary>
		/// Search the catalog of public Devices.
		///
		/// This allows unauthenticated users to search Devices from other users
		/// that have been marked as public, allowing them to read public Device
		/// metadata, locations, streams list, and view each Devices' stream metadata
		/// and its values.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Search-Public-Devices-Catalog
		/// </summary>
		public M2XResponse DeviceCatalog(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath + "/catalog", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Retrieve the list of devices accessible by the authenticated API key that
		/// meet the search criteria.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Search-Devices
		/// </summary>
		public M2XResponse Devices(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// List Device Groups
		/// Retrieve the list of device groups for the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Device-Groups
		/// </summary>
		public M2XResponse DeviceGroups(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath + "/groups", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new device
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Device
		/// </summary>
		public M2XResponse CreateDevice(object parms)
		{
			return MakeRequest(M2XDevice.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing Device.
		/// </summary>
		public M2XDevice Device(string deviceId)
		{
			return new M2XDevice(this, deviceId);
		}

		// Distribution API

		/// <summary>
		/// Retrieve list of device distributions accessible by the authenticated API key.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#List-Distributions
		/// </summary>
		public M2XResponse Distributions(object parms = null)
		{
			return MakeRequest(M2XDistribution.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new device distribution
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#Create-Distribution
		/// </summary>
		public M2XResponse CreateDistribution(object parms)
		{
			return MakeRequest(M2XDistribution.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing device distribution.
		/// </summary>
		public M2XDistribution Distribution(string distributionId)
		{
			return new M2XDistribution(this, distributionId);
		}

		// Keys API

		/// <summary>
		/// Retrieve list of keys associated with the specified account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#List-Keys
		/// </summary>
		public M2XResponse Keys(object parms = null)
		{
			return MakeRequest(M2XKey.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new key associated with the specified account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#Create-Key
		/// </summary>
		public M2XResponse CreateKey(object parms)
		{
			return MakeRequest(M2XKey.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing key associated with the specified account.
		/// </summary>
		public M2XKey Key(string key)
		{
			return new M2XKey(this, key);
		}

		// Charts API

		/// <summary>
		/// Retrieve the list of charts that belong to the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#List-Charts
		/// </summary>
		public M2XResponse Charts(object parms = null)
		{
			return MakeRequest(M2XChart.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new chart associated with the authenticated account.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Create-Chart
		/// </summary>
		public M2XResponse CreateChart(object parms)
		{
			return MakeRequest(M2XChart.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing chart.
		/// </summary>
		public M2XChart Chart(string chartId)
		{
			return new M2XChart(this, chartId);
		}

		// Common

		public M2XResponse MakeRequest(string path, M2XClientMethod method = M2XClientMethod.GET, object parms = null)
		{
			string fullUrl = BuildUrl(path, method == M2XClientMethod.GET ? parms : null);
			HttpWebRequest request = WebRequest.CreateHttp(fullUrl);
			request.Method = method.ToString();
			if (!String.IsNullOrEmpty(this.APIKey))
				request.Headers["X-M2X-KEY"] = this.APIKey;
			request.UserAgent = UserAgent;
			string content = null;

			try
			{
				if (method != M2XClientMethod.GET)
				{
					request.ContentType = "application/json";
					if (parms == null)
						request.ContentLength = 0;
					else
					{
						content = SerializeData(parms);
						byte[] bytes = Encoding.UTF8.GetBytes(content);
						request.ContentLength = bytes.Length;
						using (var stream = request.GetRequestStream())
						{
							stream.Write(bytes, 0, bytes.Length);
						}
					}
				}

				using (var response = (HttpWebResponse)request.GetResponse())
				{
					this.LastResponse = new M2XResponse(request, content, null, response);
				}
			}
			catch (WebException ex)
			{
				this.LastResponse = new M2XResponse(request, content, ex, ex.Response as HttpWebResponse);
			}
			return this.LastResponse;
		}
		public string BuildUrl(string path, object parms = null)
		{
			string fullUrl = this.EndPoint + path;
			if (parms != null)
			{
				string queryString = ObjectToQueryString(parms);
				if (!String.IsNullOrEmpty(queryString))
					fullUrl += "?" + queryString;
			}
			return fullUrl;
		}

		private static string ObjectToQueryString(object queryParams)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var prop in queryParams.GetType().GetProperties())
			{
				object value = prop.GetValue(queryParams, null);
				if (value == null)
					continue;
				if (sb.Length > 0)
					sb.Append('&');
				sb.Append(prop.Name).Append('=').Append(HttpUtility.UrlEncode(value.ToString()));
			}
			return sb.ToString();
		}
		private static string SerializeData(object data)
		{
			string result = data as string;
			if (result != null)
				return result;
			var serializer = new JavaScriptSerializer();
			return serializer.Serialize(data);
		}

		/// <summary>
		/// Formats a DateTime value to an ISO8601 timestamp
		/// </summary>
		public static string DateTimeToString(DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
		}
	}
}
