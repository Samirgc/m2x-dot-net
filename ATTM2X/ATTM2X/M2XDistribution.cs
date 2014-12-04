using System;
using System.Web;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X Distribution API
	/// https://m2x.att.com/developer/documentation/v2/distribution
	/// </summary>
	public sealed class M2XDistribution : M2XClass
	{
		public const string UrlPath = "/distributions";

		public readonly string DistributionId;

		internal M2XDistribution(M2XClient client, string distributionId)
			: base(client)
		{
			if (String.IsNullOrWhiteSpace(distributionId))
				throw new ArgumentException(String.Format("Invalid distributionId - {0}", distributionId));

			this.DistributionId = distributionId;
		}

		internal override string BuildPath(string path)
		{
			return String.Concat(M2XDistribution.UrlPath, "/", HttpUtility.UrlPathEncode(this.DistributionId), path);
		}

		/// <summary>
		/// Retrieve information about an existing device distribution.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#View-Distribution-Details
		/// </summary>
		public M2XResponse Details()
		{
			return MakeRequest();
		}

		/// <summary>
		/// Update an existing device distribution's information.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#Update-Distribution-Details
		/// </summary>
		public M2XResponse Update(object parms)
		{
			return MakeRequest(null, M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Retrieve list of devices added to the specified distribution.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#List-Devices-from-an-existing-Distribution
		/// </summary>
		public M2XResponse Devices(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Add a new device to an existing distribution
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#Add-Device-to-an-existing-Distribution
		/// </summary>
		public M2XResponse AddDevice(object parms)
		{
			return MakeRequest(M2XDevice.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Delete an existing device distribution.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#Delete-Distribution
		/// </summary>
		public M2XResponse Delete()
		{
			return MakeRequest(null, M2XClientMethod.DELETE);
		}

		/// <summary>
		/// Retrieve list of data streams associated with the specified distribution.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#List-Data-Streams
		/// </summary>
		public M2XResponse Streams(object parms = null)
		{
			return MakeRequest(M2XStream.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Get a wrapper to access a data stream associated with the specified distribution
		/// </summary>
		public M2XStream Stream(string streamName)
		{
			return new M2XStream(this, streamName);
		}

		/// <summary>
		/// Retrieve list of triggers associated with the specified distribution.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#List-Triggers
		/// </summary>
		public M2XResponse Triggers(object parms = null)
		{
			return MakeRequest(M2XTrigger.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new trigger associated with the specified distribution.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#Create-Trigger
		/// </summary>
		public M2XResponse CreateTrigger(object parms)
		{
			return MakeRequest(M2XTrigger.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access a trigger associated with the specified distribution.
		/// </summary>
		public M2XTrigger Trigger(string triggerId)
		{
			return new M2XTrigger(this, triggerId);
		}
	}
}
