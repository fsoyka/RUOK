
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
	public class AlarmReceiverQuestionnaire : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent) {
	
			PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
			PowerManager.WakeLock w1 = pm.NewWakeLock (WakeLockFlags.Full | WakeLockFlags.AcquireCausesWakeup | WakeLockFlags.OnAfterRelease, "NotificationReceiver");
			w1.Acquire ();

			//Toast.MakeText (context, "Received intent!", ToastLength.Short).Show ();
			var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);
			var notification = new Notification (Resource.Drawable.Icon, context.Resources.GetString(Resource.String.ReminderTitle));
			//Clicking the pending intent does not go to the Home Activity Screen, but to the last activity that was active before leaving the app
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(Home)), PendingIntentFlags.UpdateCurrent);
			//Notification should be language specific
			notification.SetLatestEventInfo (context, context.Resources.GetString(Resource.String.ReminderTitle), context.Resources.GetString(Resource.String.ReminderText), pendingIntent);
			notification.Flags |= NotificationFlags.AutoCancel;
			nMgr.Notify (0, notification);

			Vibrator vibrator = (Vibrator) context.GetSystemService(Context.VibratorService);            
			if (vibrator != null)
				vibrator.Vibrate(400);

			//change shared preferences such that the questionnaire button can change its availability
			ISharedPreferences sharedPref = context.GetSharedPreferences("com.FSoft.are_u_ok.PREFERENCES",FileCreationMode.Private);
			ISharedPreferencesEditor editor = sharedPref.Edit();
			editor.PutBoolean("QuestionnaireActive", true );
			editor.Commit ();

			//start an new alarm here for the invalidation period
			//Call setAlarm in the Receiver class
			AlarmReceiverInvalid temp = new AlarmReceiverInvalid();
			temp.SetAlarm (context); //call it with the context of the activity

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
			//here I have to figure out what time it is now and what would be an appropriate time for the new alarm
			//in which time window are we now? set an alarm in the next one (random). Have it go off at least 11 minutes
			//before the next window to ensure that we will end up here again even if the invalidation timer goes off as well
			//use five 2.5 h windows starting from 9 and ending at 21.30
			//This returns the total amount of hours since midnight as a fraction, meaning that 16:30 is 16.5:
			//DateTime.Now.TimeOfDay.TotalHours
			double tempNow = DateTime.Now.TimeOfDay.TotalHours;
			double timeLeftTillNextWindow = 0;
			if ((tempNow >= 0f) & (tempNow < 9f))
				timeLeftTillNextWindow = 9f - tempNow;
			if ((tempNow >= 9f) & (tempNow < 11.5f))
				timeLeftTillNextWindow = 11.5f - tempNow;
			if ((tempNow >= 11.5f) & (tempNow < 14f))
				timeLeftTillNextWindow = 14f - tempNow;
			if ((tempNow >= 14f) & (tempNow < 16.5f))
				timeLeftTillNextWindow = 16.5f - tempNow;
			if ((tempNow >= 16.5f) & (tempNow < 19f))
				timeLeftTillNextWindow = 19f - tempNow;
			if ((tempNow >= 19f) & (tempNow < 24f))
				timeLeftTillNextWindow = 24f - tempNow + 9f; //wait till next morning
			//add a random amount between 5 minutes and (2.5 hours - 11 minutes = 150 - 11 = 139 minutes)
			Random rnd = new Random(); //generator is seeded each time it is initialized
			double offset = (double) rnd.Next(5, 139);
			//add the times
			offset += timeLeftTillNextWindow * 60; //times 60 to convert from hours to minutes
			//truncated by converting to int
			long offsetLong = (int)offset;
			//System.Console.WriteLine ("Time Left: " + timeLeftTillNextWindow.ToString () + " Random + Time: " + offsetLong.ToString ());
			alarmMgr.Set(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + offsetLong * 60 * 1000, alarmIntent);
		}
	}
}

