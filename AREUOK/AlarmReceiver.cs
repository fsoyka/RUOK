
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
		public override void OnReceive (Context context, Intent intent)
		{
			//Toast.MakeText (context, "Received intent!", ToastLength.Short).Show ();
			var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);
			var notification = new Notification (Resource.Drawable.Icon, "Reminder R-U-OK");
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(Home)), PendingIntentFlags.UpdateCurrent);
			notification.SetLatestEventInfo (context, "Reminder R-U-OK", "Please fill out the questionnaire", pendingIntent);
			notification.Flags |= NotificationFlags.AutoCancel;
			nMgr.Notify (0, notification);
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

