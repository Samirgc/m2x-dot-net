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
			if (String.IsNullOrWhiteSpace(collectionId))
				throw new ArgumentException(String.Format("Invalid collectionId - {0}", collectionId));

			this.CollectionId = collectionId;
		}

		internal override string BuildPath(string path)
		{
			return String.Concat(M2XCollection.UrlPath, "/", WebUtility.UrlEncode(this.CollectionId), path);
		}
	}
}
