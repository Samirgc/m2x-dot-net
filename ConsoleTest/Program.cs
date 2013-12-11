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
		private static void DeleteKey(M2XClient m2x, string key)
		{
			M2XKey k = m2x.GetKey(key);
			k.Delete();
		}

		private static void TestKeys(M2XClient m2x)
		{
			Console.WriteLine("Testing keys...");

			var keys = m2x.GetKeys();
			Console.WriteLine("Number of keys = " + keys.keys.Count);

			var keyName = "test-" + Guid.NewGuid();
			var keyData = m2x.CreateKey(keyName, new[] { M2XClientMethod.POST, M2XClientMethod.GET });
			Console.WriteLine("New key created - id = " + keyData.key);

			M2XKey key = m2x.GetKey(keyData.key);
			Console.WriteLine("Key name = " + key.Details().name);

			key.Regenerate();
			Console.WriteLine("Key regenerated. New id = " + key.Details().key);

			key.Update(new { name = keyName + "updated", permissions = new[] { "POST", "GET" } });
			Console.WriteLine("Key updated");

			key.Delete();
			Console.WriteLine("Key deleted");
		}

		private static void TestBlueprints(M2XClient m2x)
		{
			Console.WriteLine("Testing blueprints...");

			var blueprints = m2x.GetBlueprints();
			Console.WriteLine("Number of blueprints = " + blueprints.blueprints.Count);

			var blueprintName = "test-" + Guid.NewGuid();
			var blueprintData = m2x.CreateBlueprint(blueprintName, M2XVisibility.Public, "description");
			Console.WriteLine("New blueprint created - id = " + blueprintData.id);
			var key = blueprintData.key;

			M2XBlueprint blueprint = m2x.GetBlueprint(blueprintData.id);
			Console.WriteLine("Blueprint name = " + blueprint.Details().name);

			blueprint.Update(blueprintName + "updated", M2XVisibility.Public);
			Console.WriteLine("Blueprint updated");

			blueprint.Delete();
			Console.WriteLine("Blueprint deleted");
			DeleteKey(m2x, key);
		}

		private static void TestBatches(M2XClient m2x)
		{
			Console.WriteLine("Testing batches...");

			var batches = m2x.GetBatches();
			Console.WriteLine("Number of batches = " + batches.batches.Count);

			var batchName = "test-" + Guid.NewGuid();
			var batchData = m2x.CreateBatch(batchName, M2XVisibility.Public, "description");
			Console.WriteLine("New batch created - id = " + batchData.id);
			var bKey = batchData.key;

			M2XBatch batch = m2x.GetBatch(batchData.id);
			Console.WriteLine("Batch name = " + batch.Details().name);

			batch.Update(batchName + "updated", M2XVisibility.Public);
			Console.WriteLine("Batch updated");

			var dss = batch.GetDataSources();
			Console.WriteLine("Number of data sources in the batch = " + dss.datasources.Count);

			var dsSerial = "serial-" + Guid.NewGuid();
			var dsData = batch.AddDataSource(dsSerial);
			Console.WriteLine("New data source added - id = " + dsData.id);
			var dsKey = dsData.key;

			M2XDataSource ds = m2x.GetDataSource(dsData.id);
			ds.Delete();
			Console.WriteLine("Data source deleted");
			DeleteKey(m2x, dsKey);

			batch.Delete();
			Console.WriteLine("Batch deleted");
			DeleteKey(m2x, bKey);
		}

		private static void TestDatasources(M2XClient m2x)
		{
			Console.WriteLine("Testing data sources...");

			var dss = m2x.GetDataSources();
			Console.WriteLine("Number of data sources = " + dss.datasources.Count);

			var dsName = "test-" + Guid.NewGuid();
			var dsData = m2x.CreateDataSource(dsName, M2XVisibility.Public, "description");
			Console.WriteLine("New data source created - id = " + dsData.id);
			var key = dsData.key;

			M2XDataSource ds = m2x.GetDataSource(dsData.id);
			Console.WriteLine("Data source name = " + ds.Details().name);

			ds.Update(dsName + "updated", M2XVisibility.Public);
			Console.WriteLine("Data source updated");

			ds.Delete();
			Console.WriteLine("Data source deleted");

			DeleteKey(m2x, key);
		}

		private static void TestFeeds(M2XClient m2x)
		{
			Console.WriteLine("Testing feeds...");

			var feeds = m2x.GetFeeds();
			Console.WriteLine("Number of feeds = " + feeds.feeds.Count);

			feeds = m2x.GetFeeds(type: M2XFeedType.Blueprint);
			Console.WriteLine("Number of blueprint feeds = " + feeds.feeds.Count);

			var dsName = "test feed - " + Guid.NewGuid();
			var dsData = m2x.CreateBlueprint(dsName, M2XVisibility.Public, "test feed");
			Console.WriteLine("New blueprint feed created - id = " + dsData.id);
			var key = dsData.key;
			M2XBlueprint ds = m2x.GetBlueprint(dsData.id);
			M2XFeed feed = ds.GetFeed();

			Console.WriteLine("Feed name = " + feed.Details().name);

			feed.UpdateLocation(-37.9788423562422, -57.5478776916862, "test location", 500);
			Console.WriteLine("Feed location updated");

			var location = feed.GetLocation();
			Console.WriteLine("Feed location obtained. latitude = " + location.latitude + "; longitude = " + location.longitude + "; elevation = " + location.elevation);

			var s1 = feed.GetStream("test1");
			var s2 = feed.GetStream("test2");
			var s3 = feed.GetStream("test3");
			s1.CreateOrUpdate(new { unit = new { label = "random1", symbol = "R1" } });
			Console.WriteLine("Stream with name = test1 created.");
			s2.CreateOrUpdate(new { unit = new { label = "random2", symbol = "R2" } });
			Console.WriteLine("Stream with name = test2 created.");
			s3.CreateOrUpdate(new { unit = new { label = "random3", symbol = "R3" } });
			Console.WriteLine("Stream with name = test3 created.");

			Console.WriteLine("Number of streams = " + feed.GetStreams().streams.Count);

			Console.WriteLine("Started posting values to all three streams. Go ahead and check your feed on Web UI - https://m2x.att.com/blueprints. Press any key to break.");

			var r = new Random(1000);

			while (!Console.KeyAvailable)
			{
				s1.PostValues(new[] { new M2XPostedValue { At = DateTime.UtcNow, Value = r.Next(100).ToString() } });
				s2.PostValues(new[] { new M2XPostedValue { At = DateTime.UtcNow, Value = r.Next(500).ToString() } });
				s3.PostValues(new[] { new M2XPostedValue { At = DateTime.UtcNow, Value = r.Next(1000).ToString() } });
				Thread.Sleep(1000);
			}
			Console.WriteLine("Number of values in stream test1 = " + s1.GetValues().values.Count);
			s1.Delete();
			Console.WriteLine("Number of values in stream test2 = " + s2.GetValues().values.Count);
			s2.Delete();
			Console.WriteLine("Number of values in stream test3 = " + s3.GetValues().values.Count);
			s3.Delete();
			Console.WriteLine("Data streams deleted");
			Console.WriteLine("Number of streams = " + feed.GetStreams().streams.Count);

			Console.WriteLine("Number of records in log = " + feed.Log().requests.Count);

			ds.Delete();
			Console.WriteLine("Blueprint feed deleted");
			DeleteKey(m2x, key);

		}


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

			var m2x = new M2XClient(args[0]);

			TestKeys(m2x);
			Console.WriteLine();
			TestBlueprints(m2x);
			Console.WriteLine();
			TestBatches(m2x);
			Console.WriteLine();
			TestDatasources(m2x);
			Console.WriteLine();
			TestFeeds(m2x);
		}
	}
}
