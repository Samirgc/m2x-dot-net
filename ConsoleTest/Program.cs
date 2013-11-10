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
			var m2x = new ATTM2X.M2XClient("8593b7e8dd4081e71e63d5422ae8b791");

/*			var keys = m2x.GetKeys();

			var key = m2x.CreateKey("test", new[] { M2XClientMethod.POST, M2XClientMethod.GET });

			keys = m2x.GetKeys();

			var blueprints = m2x.GetBlueprints();
			*/
//			var feeds = m2x.GetFeeds();

//			feeds = m2x.GetFeeds(q: "DS1", latitude: -37.8, longitude: -57.54, distance: 100, distanceUnit: M2XFeedLocationDistanceUnitType.Miles);

//			var ds = m2x.GetDataSources();

			var feed = m2x.GetFeed("2423e3adc7f8fa5824a7fa311cbc415a");
			feed.UpdateLocation(-37.9788423562422, -57.5478776916862, elevation: 100.12);

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
			var s2 = feed.GetStream("test2");
			var s3 = feed.GetStream("test3");
			s1.Delete();
			s2.Delete();
			s3.Delete();
			s1.CreateOrUpdate(new { unit = new { label = "random1", symbol = "R1" } });
			s2.CreateOrUpdate(new { unit = new { label = "random2", symbol = "R2" } });
			s3.CreateOrUpdate(new { unit = new { label = "random3", symbol = "R3" } });

			var r = new Random(1000);

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
