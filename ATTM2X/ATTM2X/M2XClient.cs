using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X API
	/// https://m2x.att.com/developer/documentation/v2/overview
	/// </summary>
	public sealed class M2XClient : IDisposable
	{
		public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
		public const string ApiEndPointSecure = "https://api-m2x.att.com/v2";
		public const string ApiEndPoint = "http://api-m2x.att.com/v2";

		private static readonly string UserAgent;

		public readonly string APIKey;
		public readonly string EndPoint;

		private HttpClient client = new HttpClient();

		private volatile M2XResponse lastResponse;
		/// <summary>
		/// The last API call response
		/// </summary>
		public M2XResponse LastResponse { get { return this.lastResponse; } }

		static M2XClient()
		{
			string version = typeof(M2XClient).GetTypeInfo().Assembly.GetName().Version.ToString();
			string langVersion = "4.5";//Environment.Version.ToString();
			string osVersion = "unknown";//Environment.OSVersion.ToString();
			UserAgent = String.Format("M2X-.NET/{0} .NETFramework/{1} ({2})", version, langVersion, osVersion);
		}

		public M2XClient(string apiKey, string m2xApiEndPoint = ApiEndPoint)
		{
			if (String.IsNullOrWhiteSpace(m2xApiEndPoint))
				throw new ArgumentException("Invalid API end point url");

			this.APIKey = apiKey;
			this.EndPoint = m2xApiEndPoint;

			client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
			if (!String.IsNullOrEmpty(this.APIKey))
				client.DefaultRequestHeaders.Add("X-M2X-KEY", this.APIKey);
		}

		public void Dispose()
		{
			if (this.client != null)
			{
				this.client.Dispose();
				this.client = null;
			}
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
		public Task<M2XResponse> DeviceCatalog(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath + "/catalog", M2XClientMethod.GET, parms);
		}
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
		public Task<M2XResponse> DeviceCatalog(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, M2XDevice.UrlPath + "/catalog", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Retrieve the list of devices accessible by the authenticated API key that
		/// meet the search criteria.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Search-Devices
		/// </summary>
		public Task<M2XResponse> Devices(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath, M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Retrieve the list of devices accessible by the authenticated API key that
		/// meet the search criteria.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Search-Devices
		/// </summary>
		public Task<M2XResponse> Devices(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, M2XDevice.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// List Device Groups
		/// Retrieve the list of device groups for the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Device-Groups
		/// </summary>
		public Task<M2XResponse> DeviceGroups(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath + "/groups", M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// List Device Groups
		/// Retrieve the list of device groups for the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Device-Groups
		/// </summary>
		public Task<M2XResponse> DeviceGroups(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, M2XDevice.UrlPath + "/groups", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new device
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Device
		/// </summary>
		public Task<M2XResponse> CreateDevice(object parms)
		{
			return MakeRequest(M2XDevice.UrlPath, M2XClientMethod.POST, parms);
		}
		/// <summary>
		/// Create a new device
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Device
		/// </summary>
		public Task<M2XResponse> CreateDevice(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, M2XDevice.UrlPath, M2XClientMethod.POST, parms);
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
		public Task<M2XResponse> Distributions(object parms = null)
		{
			return MakeRequest(M2XDistribution.UrlPath, M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Retrieve list of device distributions accessible by the authenticated API key.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#List-Distributions
		/// </summary>
		public Task<M2XResponse> Distributions(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, M2XDistribution.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new device distribution
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#Create-Distribution
		/// </summary>
		public Task<M2XResponse> CreateDistribution(object parms)
		{
			return MakeRequest(M2XDistribution.UrlPath, M2XClientMethod.POST, parms);
		}
		/// <summary>
		/// Create a new device distribution
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#Create-Distribution
		/// </summary>
		public Task<M2XResponse> CreateDistribution(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, M2XDistribution.UrlPath, M2XClientMethod.POST, parms);
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
		public Task<M2XResponse> Keys(object parms = null)
		{
			return MakeRequest(M2XKey.UrlPath, M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Retrieve list of keys associated with the specified account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#List-Keys
		/// </summary>
		public Task<M2XResponse> Keys(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, M2XKey.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new key associated with the specified account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#Create-Key
		/// </summary>
		public Task<M2XResponse> CreateKey(object parms)
		{
			return MakeRequest(M2XKey.UrlPath, M2XClientMethod.POST, parms);
		}
		/// <summary>
		/// Create a new key associated with the specified account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#Create-Key
		/// </summary>
		public Task<M2XResponse> CreateKey(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, M2XKey.UrlPath, M2XClientMethod.POST, parms);
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
		public Task<M2XResponse> Charts(object parms = null)
		{
			return MakeRequest(M2XChart.UrlPath, M2XClientMethod.GET, parms);
		}
		/// <summary>
		/// Retrieve the list of charts that belong to the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#List-Charts
		/// </summary>
		public Task<M2XResponse> Charts(CancellationToken cancellationToken, object parms = null)
		{
			return MakeRequest(cancellationToken, M2XChart.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new chart associated with the authenticated account.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Create-Chart
		/// </summary>
		public Task<M2XResponse> CreateChart(object parms)
		{
			return MakeRequest(M2XChart.UrlPath, M2XClientMethod.POST, parms);
		}
		/// <summary>
		/// Create a new chart associated with the authenticated account.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Create-Chart
		/// </summary>
		public Task<M2XResponse> CreateChart(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, M2XChart.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing chart.
		/// </summary>
		public M2XChart Chart(string chartId)
		{
			return new M2XChart(this, chartId);
		}

		// Common

		/// <summary>
		/// Formats a DateTime value to an ISO8601 timestamp
		/// </summary>
		public static string DateTimeToString(DateTime dateTime)
		{
			return dateTime.ToString(DateTimeFormat);
		}

		/// <summary>
		/// Builds url to AT&T M2X API with optional query parameters
		/// </summary>
		/// <param name="path">AT&T M2X API url path</param>
		/// <param name="parms">parameters to build url query</param>
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

		public async Task<M2XResponse> MakeRequest(
			string path, M2XClientMethod method = M2XClientMethod.GET, object parms = null)
		{
			M2XResponse result = CreateResponse(path, method, parms);
			try
			{
				HttpResponseMessage responseMessage;
				switch (method)
				{
					case M2XClientMethod.POST:
						responseMessage = await this.client.PostAsync(result.RequestUri, result.GetContent());
						break;
					case M2XClientMethod.PUT:
						responseMessage = await this.client.PutAsync(result.RequestUri, result.GetContent());
						break;
					case M2XClientMethod.DELETE:
						responseMessage = await this.client.DeleteAsync(result.RequestUri);
						break;
					default:
						responseMessage = await this.client.GetAsync(result.RequestUri);
						break;
				}
				result.SetResponse(responseMessage);
			}
			catch (Exception ex)
			{
				result.WebError = ex;
			}
			this.lastResponse = result;
			return result;
		}
		public async Task<M2XResponse> MakeRequest(CancellationToken cancellationToken,
			string path, M2XClientMethod method = M2XClientMethod.GET, object parms = null)
		{
			M2XResponse result = CreateResponse(path, method, parms);
			try
			{
				HttpResponseMessage responseMessage;
				switch (method)
				{
					case M2XClientMethod.POST:
						responseMessage = await this.client.PostAsync(result.RequestUri, result.GetContent(), cancellationToken);
						break;
					case M2XClientMethod.PUT:
						responseMessage = await this.client.PutAsync(result.RequestUri, result.GetContent(), cancellationToken);
						break;
					case M2XClientMethod.DELETE:
						responseMessage = await this.client.DeleteAsync(result.RequestUri, cancellationToken);
						break;
					default:
						responseMessage = await this.client.GetAsync(result.RequestUri, cancellationToken);
						break;
				}
				result.SetResponse(responseMessage);
			}
			catch (Exception ex)
			{
				result.WebError = ex;
			}
			this.lastResponse = result;
			return result;
		}

		private M2XResponse CreateResponse(string path, M2XClientMethod method, object parms)
		{
			bool isGetOrDelete = method == M2XClientMethod.GET || method == M2XClientMethod.DELETE;
			string url = BuildUrl(path, isGetOrDelete ? parms : null);
			string content = isGetOrDelete ? null : SerializeData(parms);
			return new M2XResponse(new Uri(url), method, content);
		}

		public static string ObjectToQueryString(object queryParams)
		{
			StringBuilder sb = new StringBuilder();
			IEnumerable<FieldInfo> fields = queryParams.GetType().GetFields();
			foreach (var prop in fields)
			{
				if (prop.IsStatic || !prop.IsPublic || prop.FieldType.IsArray)
					continue;
				object value = prop.GetValue(queryParams);
				AppendQuery(sb, prop.Name, value);
			}
			IEnumerable<PropertyInfo> props = queryParams.GetType().GetProperties();
			foreach (var prop in props)
			{
				if (!prop.CanRead || prop.PropertyType.IsArray)
					continue;
				object value = prop.GetValue(queryParams, null);
				AppendQuery(sb, prop.Name, value);
			}
			return sb.ToString();
		}
		private static void AppendQuery(StringBuilder sb, string name, object value)
		{
			if (value == null)
				return;
			if (sb.Length > 0)
				sb.Append('&');
			sb.Append(name).Append('=').Append(WebUtility.UrlEncode(value.ToString()));
		}

		public static string SerializeData(object data)
		{
			if (data == null)
				return null;
			string result = data as string;
			if (result != null)
				return result;

			var serializer = new DataContractJsonSerializer(data.GetType());
			using (var stream = new MemoryStream())
			{
				serializer.WriteObject(stream, data);
				stream.Position = 0;
				return new StreamReader(stream).ReadToEnd();
			}
		}
	}
}
