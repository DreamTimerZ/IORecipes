# IORecipes
## Windows.Storage API for both Windows Phone 8 and Windows 8 developers

With the Windows.Storage namespace now available on both Windows Phone 8 and Windows 8, we created this library to provide a simple, straightforward async way to do common file operations without having to use the lower-level API calls everytime we wanted to open a file or look at the contents of a directory.

All the methods are static so simply include this using statement:

		using DreamTimeStudioZ.Recipes

and then call your desired methods.  Here is an example of how to read the contents of a data file written by your app, "myfile.txt", into a string variable:

	StorageFolder ourFolder = IORecipes.GetAppStorageFolder();
	StorageFile myFile = await IORecipes.GetFileInFolder(ourFolder,"myfile.txt");
	string myFileContents = await IORecipes.ReadStringFromFile(myFile);
[Follow us on Twitter](http://www.twitter.com/dreamtimerz)

This source code is distributed free of charge and is licensed under the MIT license.
* * *

**Create a file in the specified folder**

		public static async Task<StorageFile> CreateFileInFolder(StorageFolder storageFolder, string fileName)

**Create the specified subfolder in the app's root storage folder**

		public static async Task<StorageFolder> CreateOrGetFolder(string folderName)

**Delete the specified file from the specified folder**

		public static async Task<bool> DeleteFileInFolder(StorageFolder storageFolder, string fileName)

**Get the root folder for storing files created by your user**

		public static StorageFolder GetAppStorageFolder()

**Get the installation folder for the current app. This is used to retrieve data files you've bundled with your app as opposed to data files created by your user**.

		public static StorageFolder GetAppInstallationFolder()

**Get the names of all the files in the default folder for this app**

		public static async Task<List<string>> GetDocumentFiles()

**Get a list of names of files whose filename ends with the specified extension**

		 public static async Task<List<string>> GetDocumentFilesOfTypeFromFolder(StorageFolder storageFolder, string extension)
       
**Get a StorageFile object for the specified file in the specified folder**

	        public static async Task<StorageFile> GetFileInFolder(StorageFolder storageFolder, string fileName)


**Get StorageFile objects for all the files in the specified folder**

	        public static async Task<IReadOnlyList<StorageFile>> GetFilesInFolder(StorageFolder storageFolder)

**Load XML from a file into an XDocument structure**

	        public static async Task<XDocument> LoadXmlDocumentAsync(StorageFolder storageFolder, string filename)

**Read the entire contents of the specified file and return it as a string**

	        public static async Task<string> ReadStringFromFile(StorageFile f)

**Write an XDocument to the specified file in the specified folder.**

	        public static async Task SaveXmlDocumentAsync(XDocument xmlDoc, StorageFolder storageFolder, string filename)

**Deserialize an arbitrary object from an XML string**

	        public static T SerializeFromString<T>(string xml)

**Serialize an arbitrary object to an XML string**

	        public static string SerializeToString(object obj)

**Write the specified string to a file overwriting all its previous contents**

        public static async Task<bool> WriteStringToFile(StorageFile f, string data)

