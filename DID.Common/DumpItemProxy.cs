namespace DID.Common
{
	/// <summary>
	/// A shadow-reference used for publish-trees on the server.
	/// Proxy to a full dump item.
	/// </summary>
	public class DumpItemProxy : Disposable, IDumpItem
	{
		public IDumpItem Ref;
		// TODO: implement all interfaces as a proxy to original
	}
}
