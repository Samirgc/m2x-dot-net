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


Installation and System Requirements
==========================
The M2X API .NET Client library is a regular MS VS 2012 Class Library. The only dependency is .NET Framework version 4.5 which can be downloaded at the following address - http://www.microsoft.com/en-us/download/details.aspx?id=30653. 

Just add it as an Existing Project into your VS solution or if you are using a different version of Visual Studio you can create a new class library project and include the content of [ATTM2X/ATTM2X](https://github.com/attm2x/m2x-dot-net/tree/master/ATTM2X/ATTM2X) folder into it. 
Here is the list of additional references you will need to add in this case:

* System.Web
* System.Web.Extensions

Besides the API Client library the solution also includes a console test app which contains multiple examples of library usage - see [ConsoleTest](https://github.com/attm2x/m2x-dot-net/tree/master/ConsoleTest) folder.

System requirements match those for .NET Framework 4.5.  

 - Supported Operating System:

		Windows 7 Service Pack 1, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Vista Service Pack 2
		Windows Vista SP2 (x86 and x64)
		Windows 7 SP1 (x86 and x64)
		Windows Server 2008 R2 SP1 (x64)
		Windows Server 2008 SP2 (x86 and x64)

 - Hardware Requirements:

		1 GHz or faster processor
		512 MB of RAM
		850 MB of available hard disk space (x86)
		2 GB hard drive (x64)

Note: Windows 8 and Windows Server 2012 include the .NET Framework 4.5. Therefore, you don't have to install this software on those operating systems.    

Library structure 
==========================

Currently, the client supports API v1 and all M2X API documents can be found at [M2X API Documentation](https://m2x.att.com/developer/documentation/overview).
All classes are located within ATTM2X namespace.

* [M2XClient](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XClient.cs) - this is the library's main entry point. In order to communicate with the M2X API you need an instance of this class. The constructor signature includes 2 parameters:

 apiKey - mandatory parameter. You can find it on your M2X [Account page](https://m2x.att.com/account#master-keys-tab)
Read more about M2X API keys in the [API Keys](https://m2x.att.com/developer/documentation/overview#API-Keys) section of [M2X API Documentation](https://m2x.att.com/developer/documentation/overview).

 m2xApiEndPoint - optional parameter. You don't need to pass it unless you want to connect to a different API endpoint.

 Client class provides access to API calls returning lists of the following API objects: blueprints, batches, data sources, keys, feeds. 

 - Get the list of all your keys: 

			var client = new M2XClient("[API Key]");
			var keys = client.GetKeys();
			Console.WriteLine("Number of keys = " + keys.keys.Count);

 Also there are a number of methods allowing you to get an instance of individual API object by providing its id or name as a parameter:

 - Get an instance of a feed:

			M2XFeed feed = client.GetFeed("[Feed id]");
			var location = feed.GetLocation();

* [Blueprints](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XBlueprint.cs)

	Use GetBlueprints method of M2XClient to get the list of blueprints of your M2X account. [API spec](https://m2x.att.com/developer/documentation/datasource#List-Blueprints)

	Use GetBlueprint method of M2XClient to get an instance of M2XBlueprint class.
  
  - Creation [API spec](https://m2x.att.com/developer/documentation/datasource#Create-Blueprint)

			var blueprintData = client.CreateBlueprint("Blueprint", M2XVisibility.Public, "Blueprint description"); 
			M2XBlueprint blueprint = m2x.GetBlueprint(blueprintData.id);

  - Update [API spec](https://m2x.att.com/developer/documentation/datasource#Update-Blueprint-Details)

			blueprint.Update("Blueprint updated", M2XVisibility.Private);

  - Removal [API spec](https://m2x.att.com/developer/documentation/datasource#Delete-Blueprint)

			blueprint.Delete();

  - Get blueprint details [API spec](https://m2x.att.com/developer/documentation/datasource#View-Blueprint-Details)

			var blueprintData = blueprint.Details();

* [Batches](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XBatch.cs)

	Use GetBatches method of M2XClient to get the list of batches of your M2X account. [API spec](https://m2x.att.com/developer/documentation/datasource#List-Batches)

	Use GetBatch method of M2XClient to get an instance of M2XBatch class.
  
  - Creation [API spec](https://m2x.att.com/developer/documentation/datasource#Create-Batch)

			var batchData = client.CreateBatch("batch", M2XVisibility.Public, "batch description");
			M2XBatch batch = m2x.GetBatch(batchData.id);

  - Update [API spec](https://m2x.att.com/developer/documentation/datasource#Update-Batch-Details)

			batch.Update("batch updated", M2XVisibility.Private);

  - Removal [API spec](https://m2x.att.com/developer/documentation/datasource#Delete-Batch)

			batch.Delete();

  - Get batch details [API spec](https://m2x.att.com/developer/documentation/datasource#View-Batch-Details)

			var batchData = batch.Details();

  - To get all the datasources in this batch use GetDataSources method [API spec](https://m2x.att.com/developer/documentation/datasource#List-Data-Sources-from-a-Batch)
    
			var dataSources = batch.GetDataSources();
  
  - To create a data source associated with the batch use AddDataSource method [API spec](https://m2x.att.com/developer/documentation/datasource#Add-Data-Source-to-an-existing-Batch)

			var datasourceData = batch.AddDataSource("[data source serial number]");

* [Data Sources](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XDataSource.cs)

	Use GetDataSources method of M2XClient to get the list of data sources of your M2X account. [API spec](https://m2x.att.com/developer/documentation/datasource#List-Data-Sources)

	Use GetDataSource method of M2XClient to get an instance of M2XDataSource class.
  
  - Creation [API spec](https://m2x.att.com/developer/documentation/datasource#Create-Data-Source)

			var dsData = client.CreateDataSource("test data source", M2XVisibility.Public, "data source description");
			M2XDataSource dataSource = m2x.GetDataSource(dsData.id);

  - Update [API spec](https://m2x.att.com/developer/documentation/datasource#Update-Data-Source-Details)

			dataSource.Update("data source updated", M2XVisibility.Private);

  - Removal [API spec](https://m2x.att.com/developer/documentation/datasource#Delete-Data-Source)

			dataSource.Delete();

  - Get details [API spec](https://m2x.att.com/developer/documentation/datasource#View-Data-Source-Details)

			var dsData = dataSource.Details();

* [Keys](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XKey.cs)

	Use GetKeys method of M2XClient to get the list of API keys of your M2X account. [API spec](https://m2x.att.com/developer/documentation/keys#List-Keys)

	Use GetKey method of M2XClient to get an instance of M2XKey class.
  
  - Creation [API spec](https://m2x.att.com/developer/documentation/keys#Create-Key)

			var keyData = client.CreateKey("test key", new[] { M2XClientMethod.POST, M2XClientMethod.GET });
			M2XKey key = m2x.GetKey(keyData.key);

  - Update [API spec](https://m2x.att.com/developer/documentation/keys#Update-Key)

			key.Update(new { name = "updated name", permissions = new[] { "POST", "GET" } });

  - Removal [API spec](https://m2x.att.com/developer/documentation/keys#Delete-Key)

			key.Delete();

  - Get details [API spec](https://m2x.att.com/developer/documentation/keys#View-Key-Details)

			var keyData = key.Details();

  - Regeneration. Method generates new key id. [API spec](https://m2x.att.com/developer/documentation/keys#Regenerate-Key)
		
			var keyData = key.Regenerate();


* [Feeds](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XFeed.cs)

	Use GetFeeds method of M2XClient to get the list of feeds of your M2X account. [API spec](https://m2x.att.com/developer/documentation/feed#List-Search-Feeds)

	Use GetFeed method of M2XClient to get an instance of M2XFeed class. 
	Feeds creation is done when creating a DataSource, Blueprint or Batch.
	Update and removal is not supported by the cloud API.

	You can get a feed associated with a blueprint, a batch or a data source by calling GetFeed method of the corresponding class: M2XBlueprint, M2XBatch or M2XDataSource.

			M2XFeed feed = dataSource.GetFeed();

  - Feed location

    Get feed location  [API spec](https://m2x.att.com/developer/documentation/feed#Read-Datasource-Location)

			var locationData = feed.GetLocation();

    Update feed location  [API spec](https://m2x.att.com/developer/documentation/feed#Update-Datasource-Location)

			feed.UpdateLocation(-37.9788423562422, -57.5478776916862, "test location", 500);

  - Feed logs  [API spec](https://m2x.att.com/developer/documentation/feed#View-Request-Log)

			var logData = feed.Log();

  - Feed streams

	Get the list of feed streams: [API spec](https://m2x.att.com/developer/documentation/feed#List-Data-Streams)

			feed.GetStreams();

* [Streams](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XStream.cs)

	Use GetStreams method of M2XFeed to get the list of streams of your feed. [API spec](https://m2x.att.com/developer/documentation/feed#List-Data-Streams)

	Use GetSteam method of M2XFeed to get an instance of M2XStream class. 

  - Creation and update is implemented as one method [API spec](https://m2x.att.com/developer/documentation/feed#Create-Update-Data-Stream)

			M2XStream stream = feed.GetStream("[stream name]");
			stream.CreateOrUpdate(new { unit = new { label = "Celsius", symbol = "C" } });

  - Removal [API spec](https://m2x.att.com/developer/documentation/feed#Delete-Data-Stream)

			stream.Delete();

  - Get details [API spec](https://m2x.att.com/developer/documentation/feed#View-Data-Stream)

			var streamData = stream.Details();
	
  - Pull stream values [API spec](https://m2x.att.com/developer/documentation/feed#List-Data-Stream-Values)

			var values = stream.GetValues();

  - Post values to stream [API spec](https://m2x.att.com/developer/documentation/feed#Post-Data-Stream-Values)
		
			stream.PostValues(new[] { new M2XPostedValue { At = DateTime.Now, Value = "36.6" } });


* [M2XAPIException](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XClientBase.cs#L20)
 
 All API errors are wrapped in M2XAPIException object which has the following properties:
 - Message - error message.
 - Url - the URL of API call which caused this error.
 - StatusCode - HTTP status code.
 - ValidationErrors object - returned by some API methods. Contains additional error details.
 

 - Example. The following API response:

			422 Unprocessable Entity
			{ "message": "Validation Failed",
			"errors": { "latitude": ["not_present"], "longitude": ["not_valid"] } }

		gets converted into the following M2XAPIException instance:
			
			Message = "M2X API error code 422: Validation Failed"
			Url = "[API call URL]"
			StatusCode = 422
			ValidationErrors = { "latitude": ["not_present"], "longitude": ["not_valid"] }


ConsoleTest application 
==========================
 The [console test application](https://github.com/attm2x/m2x-dot-net/blob/master/ConsoleTest/Program.cs) included into ATTM2X solution has a lot of examples for the most of M2X API methods.

 Usage: 
		
		ConsoleTest.exe [APIKey]

 Replace [APIKey] with Master Key of your M2X account.