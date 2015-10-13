
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AREUOK
{
	[BroadcastReceiver]
	public class AlarmReceiverInvalid : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent) {
	
			PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
			PowerManager.WakeLock w1 = pm.NewWakeLock (WakeLockFlags.Full | WakeLockFlags.AcquireCausesWakeup | WakeLockFlags.OnAfterRelease, "NotificationReceiver");
			w1.Acquire ();

			//Toast.MakeText (context, "Received intent!", ToastLength.Short).Show ();
			var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);
			//var notification = new Notification (Resource.Drawable.Icon, context.Resources.GetString(Resource.String.ReminderTitle));
			var notification = new Notification (Resource.Drawable.Icon, context.Resources.GetString(Resource.String.ReminderTitle));
			//Clicking the pending intent does not go to the Home Activity Screen, but to the last activity that was active before leaving the app
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(Home)), PendingIntentFlags.UpdateCurrent);
			//Notification should be language specific
			notification.SetLatestEventInfo (context, context.Resources.GetString(Resource.String.ReminderTitle), context.Resources.GetString(Resource.String.InvalidationText), pendingIntent);
			notification.Flags |= NotificationFlags.AutoCancel;
			nMgr.Notify (0, notification);

//			Vibrator vibrator = (Vibrator) context.GetSystemService(Context.VibratorService);            
//			if (vibrator != null)
//				vibrator.Vibrate(400);

			//change shared preferences such that the questionnaire button can change its availability
			ISharedPreferences sharedPref = context.GetSharedPreferences("com.FSoft.are_u_ok.PREFERENCES",FileCreationMode.Private);
			ISharedPreferencesEditor editor = sharedPref.Edit();
			editor.PutBoolean("QuestionnaireActive", false );
			editor.Commit ();

			//insert a line of -1 into some DB values to indicate that the questions have not been answered at the scheduled time
			MoodDatabase dbMood = new MoodDatabase(context);
			ContentValues insertValues = new ContentValues();
			insertValues.Put("date", DateTime.Now.ToString("dd.MM.yy"));
			insertValues.Put("time", DateTime.Now.ToString("HH:mm"));
			insertValues.Put("mood", -1);
			insertValues.Put("people", -1);
			insertValues.Put("what", -1);
			insertValues.Put("location", -1);
			//use the old value of questionFlags
			Android.Database.ICursor cursor;
			cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, QuestionFlags FROM MoodData WHERE date = '" + DateTime.Now.ToString("dd.MM.yy") + "'", null); // cursor query
			int alreadyAsked = 0; //default value: no questions have been asked yet
			if (cursor.Count > 0) { //data was already saved today and questions have been asked, so retrieve which ones have been asked
				cursor.MoveToLast (); //take the last entry of today
				alreadyAsked = cursor.GetInt(cursor.GetColumnIndex("QuestionFlags")); //retrieve value from last entry in db column QuestionFlags
			}
			insertValues.Put("QuestionFlags", alreadyAsked);
			dbMood.WritableDatabase.Insert ("MoodData", null, insertValues);

			//set the new alarm
			AlarmReceiverQuestionnaire temp = new AlarmReceiverQuestionnaire();
			temp.SetAlarm(context);

			w1.Release ();

			//check these pages for really waking up the device
			// http://stackoverflow.com/questions/6864712/android-alarmmanager-not-waking-phone-up
			// https://forums.xamarin.com/discussion/7490/alarm-manager

			//it's good to use the alarm manager for tasks that should last even days:
			// http://stackoverflow.com/questions/14376470/scheduling-recurring-task-in-android/
		}

		public void SetAlarm(Context context){
			AlarmManager alarmMgr = (AlarmManager)context.GetSystemService(Context.AlarmService);
			Intent intent = new Intent(context, this.Class);
			PendingIntent alarmIntent = PendingIntent.GetBroadcast(context, 0, intent, 0);
			//how many seconds should we wait before the reminder becomes invalid? Maybe 10 minutes
			alarmMgr.Set(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + 10 * 60 * 1000, alarmIntent);
		}
	}
}

