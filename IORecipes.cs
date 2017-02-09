using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;

//
// Copyright (c) 2013-2017, DreamTimeStudioZ, LLC
// All Rights Reserved
// Open Source License: MIT
//
// Updated 07Feb17 to enable appending writes
//

namespace DreamTimeStudioZ.Recipes
{
	public class IORecipes
	{
		//
		// Change this, as desired, to point to the base folder where your app stores it's data
		//
		// Note:    On Windows Phone, this (below) is your only option
		//
		//          On Windows 8, you can also use the folders in KnownFolders or create a
		//          arbrtrary StorageFolder object provided you request the appropriate capabilities
		//
		private static StorageFolder AppBaseFolder = ApplicationData.Current.LocalFolder;

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Create a file in the specified folder
		/// </summary>
		/// <param name="storageFolder">Name of folder in which file is to be created</param>
		/// <param name="fileName">Name of file to be created</param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<StorageFile> CreateFileInFolder(StorageFolder storageFolder, string fileName)
		{
			StorageFile newFile;
			try
			{
				newFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
			}
			catch (Exception ex)
			{
				newFile = null;
			}
			return newFile;
		}

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Delete the specified file from the specified folder
		/// </summary>
		/// <param name="storageFolder">Name of folder in which file is to be deleted</param>
		/// <param name="fileName">Name of file to be deleted</param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<bool> DeleteFileInFolder(StorageFolder storageFolder, string fileName)
		{
			bool deleteSuccessful = true;

			try
			{
				StorageFile fileToDelete = await storageFolder.GetFileAsync(fileName);
				await fileToDelete.DeleteAsync();
			}
			catch (Exception ex)
			{
				deleteSuccessful = false;
			}
			return deleteSuccessful;
		}

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Get the root folder for the current app.  This is used to retrieve data files
		/// you've bundled with your app as opposed to data files created by your user.
		/// </summary>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static StorageFolder GetAppInstallationFolder()
		{
			StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
			return folder;
		}

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Get the root folder for the current app.  This is used to retrieve data files
		/// you've bundled with your app as opposed to data files created by your user.
		/// </summary>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static StorageFolder GetAppStorageFolder()
		{
			return AppBaseFolder;
		}

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Create the specified subfolder in the app's root storage folder
		/// </summary>
		/// <param name="folderName">Name of folder to be created</param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<StorageFolder> CreateOrGetFolder(string folderName)
		{
			StorageFolder newFolder;
			try
			{
				StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

				if (folderName == "\\")
					return storageFolder;

				try
				{
					newFolder = await storageFolder.GetFolderAsync(folderName);
				}
				catch (Exception ex)
				{
					newFolder = null;
				}

				if (newFolder == null)
					newFolder = await storageFolder.CreateFolderAsync(folderName);
			}
			catch (Exception ex)
			{
				newFolder = null;
			}
			return newFolder;
		}


		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Get a StorageFile object for the specified file in the specified folder
		/// </summary>
		/// <param name="storageFolder">Name of the folder from which the file is to be retrieved</param>
		/// <param name="fileName">Name of the file to be retrieved</param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<StorageFile> GetFileInFolder(StorageFolder storageFolder, string fileName)
		{
			StorageFile newFile;
			try
			{
				newFile = await storageFolder.GetFileAsync(fileName);
			}
			catch (Exception ex)
			{
				newFile = null;
			}
			return newFile;

		}


		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Get the names of all the files in the default folder for this app
		/// </summary>
		/// <returns>List of all the files in this app's root folder</returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<List<string>> GetDocumentFiles()
		{
			List<string> results = new List<string>();

			try
			{
				StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
				StorageFileQueryResult queryResult = storageFolder.CreateFileQuery();
				var files = await queryResult.GetFilesAsync();
				foreach (StorageFile file in files)
				{
					results.Add(file.Name);
				}
			}
			catch (Exception ex)
			{
				results = null;
			}
			return results;
		}

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Get StorageFile objects for all the files in the specified folder
		/// </summary>
		/// <param name="storageFolder">Name of the folder for which the file objects are to be retrieved</param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<IReadOnlyList<StorageFile>> GetFilesInFolder(StorageFolder storageFolder)
		{
			IReadOnlyList<StorageFile> results = null;

			try
			{
				results = await storageFolder.GetFilesAsync();
			}
			catch (Exception ex)
			{
			}
			return results;
		}

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Write the specified string to a file overwriting all its previous contents
		/// </summary>
		/// <param name="f">StorageFile to which the data is to be written</param>
		/// <param name="data">String data to write to the file</param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<bool> WriteStringToFile(StorageFile f, string data)
		{
			return await WriteStringToFileWithAppendOption(f, data, false);
		}

		public static async Task<bool> WriteStringToFileWithAppendOption(StorageFile f, string data, bool append)
		{
			bool result = true;

			try
			{
				using (var stream = await f.OpenAsync(FileAccessMode.ReadWrite))
				{

					if (append)
					{
						ulong fileSize = stream.Size;

						stream.Seek(fileSize);
					}


					using (DataWriter dataWriter = new DataWriter(stream))
					{
						dataWriter.WriteString(data);
						await dataWriter.StoreAsync();
						await dataWriter.FlushAsync();
					}
				}
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}


		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Read the entire contents of the specified file and return it as a string
		/// </summary>
		/// <param name="f">StorageFile from which data is to be read</param>
		/// <returns>Entire contents of the file as a string</returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<string> ReadStringFromFile(StorageFile f)
		{
			string result = null;

			try
			{
				using (var stream = await f.OpenAsync(FileAccessMode.Read))
				{
					using (DataReader dataReader = new DataReader(stream))
					{
						uint numBytesLoaded = await dataReader.LoadAsync((uint)stream.Size);
						result = dataReader.ReadString(numBytesLoaded);
					}
				}
			}
			catch (Exception ex)
			{
				result = null;
			}

			return result;
		}


		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Get a list of names of files whose filename ends with the specified extension
		/// </summary>
		/// <param name="storageFolder">Folder to be searched</param>
		/// <param name="extension">File extension (e.g. .txt)</param>
		/// <returns>List of names of files in the specified folder which end with the specified extension</returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<List<string>> GetDocumentFilesOfTypeFromFolder(StorageFolder storageFolder, string extension)
		{
			List<string> results = new List<string>();

			try
			{
				StorageFileQueryResult queryResult = storageFolder.CreateFileQuery();
				var files = await queryResult.GetFilesAsync();
				foreach (StorageFile file in files)
				{
					if (file.Name.ToLower().EndsWith(extension.ToLower()))
						results.Add(file.Name);
				}
			}
			catch (Exception ex)
			{
				results = null;
			}
			return results;
		}


		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Serialize an arbitrary object to an XML string
		/// </summary>
		/// <param name="obj">Object to be serialized.  Must be serializable.</param>
		/// <returns>XML string representing object or null if object is not serializable</returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static string SerializeToString(object obj)
		{
			XmlSerializer serializer = new XmlSerializer(obj.GetType());

			try
			{
				using (StringWriter writer = new StringWriter())
				{
					serializer.Serialize(writer, obj);

					return writer.ToString();
				}
			}
			catch
			{
				return null;
			}
		}

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Deserialize an arbitrary object from an XML string
		/// </summary>
		/// <typeparam name="T">Type of object to be deserialized</typeparam>
		/// <param name="xml">XML serialized version of the object</param>
		/// <returns>Object with data as described by XML or default(T) if deserialization failed.</returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static T SerializeFromString<T>(string xml)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));

			try
			{
				using (StringReader reader = new StringReader(xml))
				{
					return (T)serializer.Deserialize(reader);
				}
			}
			catch
			{
				return default(T);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Load XML from a file into an XDocument structure
		/// </summary>
		/// <param name="storageFolder">Folder containing the file</param>
		/// <param name="filename">Name of file containing the desired XML</param>
		/// <returns>XDocument representing the XML or default(XDocument) if failure occured</returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task<XDocument> LoadXmlDocumentAsync(StorageFolder storageFolder, string filename)
		{

			StorageFile storageFile = await storageFolder.GetFileAsync(filename);

			try
			{
				IRandomAccessStream readStream = await storageFile.OpenAsync(FileAccessMode.Read);

				IInputStream inputStream = readStream.GetInputStreamAt(0);

				DataReader dataReader = new DataReader(inputStream);

				uint numBytesLoaded = await dataReader.LoadAsync((uint)readStream.Size);



				string s = dataReader.ReadString(numBytesLoaded);

				StringReader stringReader = new StringReader(s);

				return XDocument.Load(stringReader);
			}
			catch
			{
				return default(XDocument);
			}
		}


		///////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Write an XDocument to the specified file in the specified folder.
		/// </summary>
		/// <param name="xmlDoc">XDocument object containing the data to write as XML to the specified file</param>
		/// <param name="storageFolder">Folder where the file is to be written</param>
		/// <param name="filename">Name of the file to be written containing the XML representation of the XDocument's contents</param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////
		public static async Task SaveXmlDocumentAsync(XDocument xmlDoc, StorageFolder storageFolder, string filename)
		{

			StringBuilder sb = new StringBuilder();

			XmlWriterSettings xws = new XmlWriterSettings();

			xws.Indent = true;

			try
			{
				using (XmlWriter xw = XmlWriter.Create(sb, xws))
				{

					xmlDoc.Save(xw);

				}



				var storageFileResult = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
				var streamResult = await storageFileResult.OpenAsync(FileAccessMode.ReadWrite);
				var outStream = streamResult.GetOutputStreamAt(0);

				DataWriter dataWriter = new DataWriter(outStream);

				dataWriter.WriteString(sb.ToString());

				await dataWriter.StoreAsync();

				await outStream.FlushAsync();
			}
			catch
			{
			}
		}

	}
}
