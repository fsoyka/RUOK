using System;
using Android.Database.Sqlite; 
using Android.Content;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace AREUOK
{
	class ZIPHelper
	{

		public void Save(string extPath)
		{
			// https://forums.xamarin.com/discussion/7499/android-content-getexternalfilesdir-is-it-available
			Java.IO.File sd = Android.OS.Environment.ExternalStorageDirectory;
			//FileStream fsOut = File.Create(sd.AbsolutePath + "/Android/data/com.FSoft.are_u_ok_/files/MoodData.zip");
			FileStream fsOut = File.Create(extPath + "/MoodData.zip");
			//https://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples
			ZipOutputStream zipStream = new ZipOutputStream(fsOut);

			zipStream.SetLevel (3); //0-9, 9 being the highest level of compression
			zipStream.Password = "Br1g1tte";  // optional. Null is the same as not setting. Required if using AES.
			ZipEntry newEntry = new ZipEntry ("Mood.csv");
			newEntry.IsCrypted = true;
			zipStream.PutNextEntry (newEntry);
			// Zip the file in buffered chunks
			// the "using" will close the stream even if an exception occurs
			byte[ ] buffer = new byte[4096];
			string filename = extPath + "/MoodData.csv";
			using (FileStream streamReader = File.OpenRead(filename)) {
				StreamUtils.Copy(streamReader, zipStream, buffer);
			}

			zipStream.CloseEntry ();
			
			zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
			zipStream.Close ();
		}

	}
}

