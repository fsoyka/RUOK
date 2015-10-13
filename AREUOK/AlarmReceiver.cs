
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
	public class AlarmReceiver : BroadcastReceiver
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
			//set the alarm for 5 seconds from now
			alarmMgr.Set(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + 5 * 1000, alarmIntent);
		}
	}
}

