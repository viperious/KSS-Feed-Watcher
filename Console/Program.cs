namespace Commpoint.Utility
{
	class Program
	{
		static void Main(string[] args)
		{
			RssFeedLogic logic = new RssFeedLogic();
			logic.StartFeedWatcher();
		}
	}
}
