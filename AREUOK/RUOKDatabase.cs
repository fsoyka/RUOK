using System;
using Android.Database.Sqlite; 
using Android.Content;

namespace AREUOK
{
	class RUOKDatabase : SQLiteOpenHelper
	{
		//see http://developer.xamarin.com/guides/android/user_interface/working_with_listviews_and_adapters/part_4_-_using_cursoradapters/
		public static readonly string create_table_sql =
			"CREATE TABLE [RUOKData] ([_id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [date_time] TEXT NOT NULL, [ergebnis] INT NOT NULL)";
		public static readonly string DatabaseName = "RUOKData.db";
		public static readonly int DatabaseVersion = 1;

		//constructor
		public RUOKDatabase (Context context) : base(context, DatabaseName, null, DatabaseVersion) { }

		public override void OnCreate(SQLiteDatabase db)
		{
			db.ExecSQL(create_table_sql);
			// seed with data
//			db.ExecSQL("INSERT INTO RUOKData (date_time, ergebnis) VALUES ('11.06.82 13:30', 25)");
//			db.ExecSQL("INSERT INTO RUOKData (date_time, ergebnis) VALUES ('11.06.82 13:30', 25)");
//			db.ExecSQL("INSERT INTO RUOKData (date_time, ergebnis) VALUES ('11.06.82 13:30', 25)");
//			db.ExecSQL("INSERT INTO RUOKData (date_time, ergebnis) VALUES ('11.06.82 13:30', 25)");
//			db.ExecSQL("INSERT INTO RUOKData (date_time, ergebnis) VALUES ('11.06.82 13:30', 25)");
//			db.ExecSQL("INSERT INTO RUOKData (date_time, ergebnis) VALUES ('11.06.82 13:30', 25)");
//			db.ExecSQL("INSERT INTO RUOKData (date_time, ergebnis) VALUES ('11.06.82 13:30', 25)");
//			db.ExecSQL("INSERT INTO RUOKData (date_time, ergebnis) VALUES ('11.06.82 13:30', 25)");
		}
		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
		{   // not required until second version :)
			throw new NotImplementedException();
		}

	}
}

