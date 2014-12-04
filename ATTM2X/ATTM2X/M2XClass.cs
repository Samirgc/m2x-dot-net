
namespace ATTM2X
{
	public abstract class M2XClass
	{
		public readonly M2XClient Client;

		internal M2XClass(M2XClient client)
		{
			this.Client = client;
		}

		internal abstract string BuildPath(string path);

		public M2XResponse MakeRequest(string path = null, M2XClientMethod method = M2XClientMethod.GET, object parms = null)
		{
			return this.Client.MakeRequest(BuildPath(path), method, parms);
		}
	}
}
