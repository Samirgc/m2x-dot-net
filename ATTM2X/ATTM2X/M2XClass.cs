using System;
using System.Threading;
using System.Threading.Tasks;

namespace ATTM2X
{
	public abstract class M2XClass
	{
		public M2XClient Client { get; private set; }

		internal M2XClass(M2XClient client)
		{
			this.Client = client;
		}

		internal abstract string BuildPath(string path);

		public Task<M2XResponse> MakeRequest(
			string path = null, M2XClientMethod method = M2XClientMethod.GET, object parms = null)
		{
			return this.Client.MakeRequest(BuildPath(path), method, parms);
		}
		public Task<M2XResponse> MakeRequest(CancellationToken cancellationToken,
			string path = null, M2XClientMethod method = M2XClientMethod.GET, object parms = null)
		{
			return this.Client.MakeRequest(cancellationToken, BuildPath(path), method, parms);
		}

		/// <summary>
		/// Get details of an existing entity.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#View-Chart-Details
		/// https://m2x.att.com/developer/documentation/v2/device#View-Device-Details
		/// https://m2x.att.com/developer/documentation/v2/distribution#View-Distribution-Details
		/// https://m2x.att.com/developer/documentation/v2/keys#View-Key-Details
		/// https://m2x.att.com/developer/documentation/v2/device#View-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#View-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/device#View-Trigger
		/// https://m2x.att.com/developer/documentation/v2/distribution#View-Trigger
		/// </summary>
		public Task<M2XResponse> Details()
		{
			return MakeRequest();
		}
		/// <summary>
		/// Get details of an existing entity.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#View-Chart-Details
		/// https://m2x.att.com/developer/documentation/v2/device#View-Device-Details
		/// https://m2x.att.com/developer/documentation/v2/distribution#View-Distribution-Details
		/// https://m2x.att.com/developer/documentation/v2/keys#View-Key-Details
		/// https://m2x.att.com/developer/documentation/v2/device#View-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#View-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/device#View-Trigger
		/// https://m2x.att.com/developer/documentation/v2/distribution#View-Trigger
		/// </summary>
		public Task<M2XResponse> Details(CancellationToken cancellationToken)
		{
			return MakeRequest(cancellationToken);
		}

		/// <summary>
		/// Update an existing entity.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Update-Chart
		/// https://m2x.att.com/developer/documentation/v2/device#Update-Device-Details
		/// https://m2x.att.com/developer/documentation/v2/distribution#Update-Distribution-Details
		/// https://m2x.att.com/developer/documentation/v2/keys#Update-Key
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Update-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#Create-Update-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/device#Update-Trigger
		/// https://m2x.att.com/developer/documentation/v2/distribution#Update-Trigger
		/// </summary>
		public Task<M2XResponse> Update(object parms)
		{
			return MakeRequest(null, M2XClientMethod.PUT, parms);
		}
		/// <summary>
		/// Update an existing entity.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Update-Chart
		/// https://m2x.att.com/developer/documentation/v2/device#Update-Device-Details
		/// https://m2x.att.com/developer/documentation/v2/distribution#Update-Distribution-Details
		/// https://m2x.att.com/developer/documentation/v2/keys#Update-Key
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Update-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#Create-Update-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/device#Update-Trigger
		/// https://m2x.att.com/developer/documentation/v2/distribution#Update-Trigger
		/// </summary>
		public Task<M2XResponse> Update(CancellationToken cancellationToken, object parms)
		{
			return MakeRequest(cancellationToken, null, M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Delete an existing entity.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Delete-Chart
		/// https://m2x.att.com/developer/documentation/v2/device#Delete-Device
		/// https://m2x.att.com/developer/documentation/v2/distribution#Delete-Distribution
		/// https://m2x.att.com/developer/documentation/v2/keys#Delete-Key
		/// https://m2x.att.com/developer/documentation/v2/device#Delete-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#Delete-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/device#Delete-Trigger
		/// https://m2x.att.com/developer/documentation/v2/distribution#Delete-Trigger
		/// </summary>
		public Task<M2XResponse> Delete()
		{
			return MakeRequest(null, M2XClientMethod.DELETE);
		}
		/// <summary>
		/// Delete an existing entity.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Delete-Chart
		/// https://m2x.att.com/developer/documentation/v2/device#Delete-Device
		/// https://m2x.att.com/developer/documentation/v2/distribution#Delete-Distribution
		/// https://m2x.att.com/developer/documentation/v2/keys#Delete-Key
		/// https://m2x.att.com/developer/documentation/v2/device#Delete-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/distribution#Delete-Data-Stream
		/// https://m2x.att.com/developer/documentation/v2/device#Delete-Trigger
		/// https://m2x.att.com/developer/documentation/v2/distribution#Delete-Trigger
		/// </summary>
		public Task<M2XResponse> Delete(CancellationToken cancellationToken)
		{
			return MakeRequest(cancellationToken, null, M2XClientMethod.DELETE);
		}
	}
}
