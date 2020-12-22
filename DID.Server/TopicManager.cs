namespace DID.Server
{
	/// <summary>
	/// Provides topic registration, dumping 
	/// </summary>
	public class TopicManager
	{
		/// <summary>
		/// Remembers new topic to be published
		/// </summary>
		/// <param name="topic"></param>
		void RegisterTopic( Topic topic ) {}
		
		/// <summary>
		/// Forgets	a topic (will be unpublished)
		/// </summary>
		/// <param name="topic"></param>
		void UnregisterTopic( Topic topic ) {}
		
		/// <summary>
		/// Invokes a dump function on all topics belonging to given dumnp group
		/// </summary>
		/// <param name="dumpGroup"></param>
		void Dump( string dumpGroup ) {}
	}

}
