using System;
using System.Net;
using System.Threading.Tasks;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X Collections API
	/// https://m2x.att.com/developer/documentation/v2/collections
	/// </summary>
	public sealed class M2XCollection : M2XClassWithMetadata
	{
		public const string UrlPath = "/collections";

		public readonly string CollectionId;

		internal M2XCollection(M2XClient client, string collectionId)
			: base(client)
		{
			if (string.IsNullOrWhiteSpace(collectionId))
				throw new ArgumentException(string.Format("Invalid collectionId - {0}", collectionId));

			this.CollectionId = collectionId;
		}

		internal override string BuildPath(string path)
		{
			var pathContainsId = !string.IsNullOrWhiteSpace(path) && path.Contains(UrlPath) && !string.IsNullOrWhiteSpace(CollectionId) && path.Contains(CollectionId);
			return string.Concat(pathContainsId ? string.Empty : $"{M2XCollection.UrlPath}/{WebUtility.UrlEncode(CollectionId)}", path);
		}

		/// <summary>
		/// Retrieve a list of collections accessible by the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/collections#List-collections
		/// </summary>
		public Task<M2XResponse> Collections(object parms = null)
		{
			return MakeRequest(UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new collection.
		/// 
		/// https://m2x.att.com/developer/documentation/v2/collections#Create-Collection
		/// </summary>
		public Task<M2XResponse> CreateCollection(object parms)
		{
			return MakeRequest(UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Add an existing Device to the current Collection
		///
		/// https://m2x.att.com/developer/documentation/v2/collections#Add-device-to-collection
		/// </summary>
		public Task<M2XResponse> AddDevice(string deviceId)
		{
			var path = BuildPath($"{M2XDevice.UrlPath}/{deviceId}");
			return MakeRequest(path, M2XClientMethod.PUT);
		}

		/// <summary>
		/// Remove a Device fro the current Collection
		///
		/// https://m2x.att.com/developer/documentation/v2/collections#Remove-device-from-collection
		/// </summary>
		public Task<M2XResponse> RemoveDevice(string deviceId)
		{
			var path = BuildPath($"{M2XDevice.UrlPath}/{deviceId}");
			return MakeRequest(path, M2XClientMethod.DELETE);
		}
	}
}