.NET M2X API Client
========================

The AT&T M2X API provides all the needed operations to connect your device to AT&T's [M2X](http://m2x.att.com) service. 
This client provides an easy to use interface for [.NET](http://www.microsoft.com/net/).


Getting Started
==========================
1. Signup for an [M2X Account](https://m2x.att.com/signup).
2. Obtain your _Master Key_ from the Master Keys tab of your [Account Settings](https://m2x.att.com/account) screen.
2. Create your first [Data Source Blueprint](https://m2x.att.com/blueprints) and copy its _Feed ID_.
3. Review the [M2X API Documentation](https://m2x.att.com/developer/documentation/overview).

Please consult the [M2X glossary](https://m2x.att.com/developer/documentation/glossary) if you have questions about any M2X specific terms.


Installation
==========================
The M2X API .NET Client library is a regular MS VS 2012 Class Library. The only dependency is .NET Framework version 4.5 which can be downloaded at the following address - http://www.microsoft.com/en-us/download/details.aspx?id=30653. 
Just add it as an Existing Project into your VS solution or if you are using a different version of Visual Studio you can create a new class library project and include the content of [ATTM2X/ATTM2X](https://github.com/attm2x/m2x-dot-net/tree/master/ATTM2X/ATTM2X) folder into it. 
Here is the list of additional references you will need to add in this case:

* System.Web
* System.Web.Extensions

Besides the API Client library the solution also includes a console test app which contains multiple examples of library usage - see [ConsoleTest](https://github.com/attm2x/m2x-dot-net/tree/master/ConsoleTest) folder.

Library structure 
==========================

Currently, the client supports API v1 and all M2X API documents can be found at [M2X API Documentation](https://m2x.att.com/developer/documentation/overview).
All classes are located within ATTM2X namespace.

* M2XClient - this is the library main entry point. In order to communicate with the M2X API you need an instance of this class. The constructor signature includes 2 parameters:

 apiKey - mandatory parameter. You can find it on your M2X [Account page](https://m2x.att.com/account#master-keys-tab)
Read more about M2X API keys in the [API Keys](https://m2x.att.com/developer/documentation/overview#API-Keys) section of [M2X API Documentation](https://m2x.att.com/developer/documentation/overview).

 m2xApiEndPoint - optional parameter. You don't need to pass it unless you want to connect to a different API endpoint.

Client class provides access to API calls returning lists of the following API objects: blueprints, batches, data sources, keys, feeds. 

Get the list of all your keys: 

''
	var client = new M2XClient("[API Key]");
	var keys = client.GetKeys();
	Console.WriteLine("Number of keys = " + keys.keys.Count);
''

Also there are a number of methods allowing you to get an instance of individual API object by providing its id or name as a parameter:

Get an instance of a feed:

''
	M2XFeed feed = client.GetFeed("[Feed id]");
	
	//get feed location
	var location = feed.GetLocation();
''

* Blueprints

	Use GetBlueprints method of M2XClient to get the list of blueprints of your M2X account.
	Use GetBlueprint method of M2XClient to get an instance of M2XBlueprint class.
  
  - Creation

        var blueprintData = client.CreateBlueprint("Blueprint", M2XVisibility.Public, "Blueprint description");
		M2XBlueprint blueprint = m2x.GetBlueprint(blueprintData.id);

  - Update

		blueprint.Update("Blueprint updated", M2XVisibility.Private);

  - Removal

		blueprint.Delete();

  - Get blueprint details

		var blueprintData = blueprint.Details();

* Batches

	Use GetBatches method of M2XClient to get the list of batches of your M2X account.
	Use GetBatch method of M2XClient to get an instance of M2XBatch class.
  
  - Creation

        var batchData = client.CreateBatch("batch", M2XVisibility.Public, "batch description");
		M2XBatch batch = m2x.GetBatch(batchData.id);

  - Update

		batch.Update("batch updated", M2XVisibility.Private);

  - Removal

		batch.Delete();

  - Get batch details

		var batchData = batch.Details();

  - To get all the datasources in this batch use GetDataSources method
    
		var dataSources = batch.GetDataSources();
  
  - To create a data source associated with the batch use AddDataSource method

		var datasourceData = batch.AddDataSource("[data source serial number]");

* Data Sources

	Use GetDataSources method of M2XClient to get the list of data sources of your M2X account.
	Use GetDataSource method of M2XClient to get an instance of M2XDataSource class.
  
  - Creation

        var dsData = client.CreateDataSource("test data source", M2XVisibility.Public, "data source description");
		M2XDataSource dataSource = m2x.GetDataSource(dsData.id);

  - Update

		dataSource.Update("data source updated", M2XVisibility.Private);

  - Removal

		dataSource.Delete();

  - Get details

		var dsData = dataSource.Details();

* Keys

	Use GetKeys method of M2XClient to get the list of API keys of your M2X account.
	Use GetKey method of M2XClient to get an instance of M2XKey class.
  
  - Creation

        var keyData = client.CreateKey("test key", new[] { M2XClientMethod.POST, M2XClientMethod.GET });
		M2XKey key = m2x.GetKey(keyData.key);

  - Update

		key.Update(new { name = "updated name", permissions = new[] { "POST", "GET" } });

  - Removal

		key.Delete();

  - Get details

		var keyData = key.Details();

  - Regeneration. Method generates new key id.
		
		var keyData = key.Regenerate();

* Feeds

Use GetFeeds method of M2XClient to get the list of feeds of your M2X account.
Use GetFeed method of M2XClient to get an instance of M2XFeed class. 
Feeds creation is done when creating a DataSource, Blueprint or Batch.
Update and removal is not supported by the cloud API.

You can get a feed assiciated with a blueprint, a batch or a data source by calling GetFeed method of the corresponding class: M2XBlueprint, M2XBatch or M2XDataSource.

		M2XFeed feed = dataSource.GetFeed();

  - Feed location

    Get feed location

        var locationData = feed.GetLocation();

    Update feed location

		feed.UpdateLocation(-37.9788423562422, -57.5478776916862, "test location", 500);

  - Feed logs

		var logData = feed.Log();

  - Feed streams

	Get the list of feed streams:

		feed.GetStreams();

* Streams

Use GetStreams method of M2XFeed to get the list of streams of your feed.
Use GetSteam method of M2XFeed to get an instance of M2XStream class. 

  - Creation and update is implemented as one method

		M2XStream stream = feed.GetStream("[stream name]");
		stream.CreateOrUpdate(new { unit = new { label = "Celsius", symbol = "C" } });

  - Removal

		stream.Delete();

  - Get details

		var streamData = stream.Details();
	
  - Pull stream values

		var values = stream.GetValues();

  - Post values to stream
		
		stream.PostValues(new[] { new M2XPostedValue { At = DateTime.Now, Value = "36.6" } });

