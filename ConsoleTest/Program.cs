using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ATTM2X;

namespace ConsoleTest
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: ConsoleTest [APIKey]");
				Console.ReadKey();
				return;
			}

			Console.WriteLine("Press Any Key to start...");
			Console.ReadKey();
			Console.Clear();

			var m2x = new ATTM2X.M2XClient(args[0]);

/*			var keys = m2x.GetKeys();

			var key = m2x.CreateKey("test", new[] { M2XClientMethod.POST, M2XClientMethod.GET });

			keys = m2x.GetKeys();

			var blueprints = m2x.GetBlueprints();
			*/
			var feeds = m2x.GetFeeds();

			if (feeds.feeds.Count == 0)
			{
				Console.WriteLine("No feeds found for the account provided. Create at least one feed first via UI - https://m2x.att.com/blueprints");
				Console.ReadKey();
				return;
			}
				
			string feedId = feeds.feeds[0].id;

			Console.WriteLine("Feed with id = " + feedId + " and name = " + feeds.feeds[0].name + " found.");

//			feeds = m2x.GetFeeds(q: "DS1", latitude: -37.8, longitude: -57.54, distance: 100, distanceUnit: M2XFeedLocationDistanceUnitType.Miles);

//			var ds = m2x.GetDataSources();

			var feed = m2x.GetFeed(feedId);
//			feed.UpdateLocation(-37.9788423562422, -57.5478776916862, elevation: 100.12);

			var test = feed.Details();

			var logs = feed.Log();

/*			var s = feed.GetStream("test_stream");

			s.PostValues(new
				             {
					             values = new[]
						                      {
							                      new { at = "2013-09-09T19:15:00Z", value = "32" },
												  new { at = "2013-09-09T20:15:00Z", value = "16" },
												  new { at = "2013-09-09T21:10:00Z", value = "15" }
						                      }
				             });

			var v = s.GetValues();*/

			var s1 = feed.GetStream("test1");
			Console.WriteLine("Stream with name = test1 created.");
			var s2 = feed.GetStream("test2");
			Console.WriteLine("Stream with name = test2 created.");
			var s3 = feed.GetStream("test3");
			Console.WriteLine("Stream with name = test3 created.");
			s1.CreateOrUpdate(new { unit = new { label = "random1", symbol = "R1" } });
			s2.CreateOrUpdate(new { unit = new { label = "random2", symbol = "R2" } });
			s3.CreateOrUpdate(new { unit = new { label = "random3", symbol = "R3" } });

			var r = new Random(1000);

			Console.WriteLine("Started posting values to all three streams. Go and check your feed on Web UI - https://m2x.att.com/blueprints. Press Ctrl+C to break.");

			while(true)
			{
				s1.PostValues( new { values = new[] { new { at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"), value = r.Next(100) }}});
				s2.PostValues( new { values = new[] { new { at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"), value = r.Next(500) }}});
				s3.PostValues( new { values = new[] { new { at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"), value = r.Next(1000) }}});
				Thread.Sleep(1000);
			}
			
		}
	}
}
