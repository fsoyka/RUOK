using System;
using Android.Database.Sqlite; 
using Android.Content;

namespace AREUOK
{
	class MoodDatabase : SQLiteOpenHelper
	{
		//see http://developer.xamarin.com/guides/android/user_interface/working_with_listviews_and_adapters/part_4_-_using_cursoradapters/
		public static readonly string create_table_sql =
			"CREATE TABLE [MoodData] ([_id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [date] TEXT NOT NULL, [time] TEXT NOT NULL, [mood] INT NOT NULL, [people] INT NOT NULL, [what] INT NOT NULL, [location] INT NOT NULL, [pos1] INT, [pos2] INT , [pos3] INT, [pos4] INT, [pos5] INT, [neg1] INT, [neg2] INT , [neg3] INT, [neg4] INT, [neg5] INT)";
		public static readonly string DatabaseName = "MoodData.db";
		public static readonly int DatabaseVersion = 1;

		//constructor
		public MoodDatabase (Context context) : base(context, DatabaseName, null, DatabaseVersion) { }

		public override void OnCreate(SQLiteDatabase db)
		{
			db.ExecSQL(create_table_sql);
		}
		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
		{   // not required until second version :)
			throw new NotImplementedException();
		}

	}
}

