
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
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class Settings : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.Settings);

			Button BackHome = FindViewById<Button> (Resource.Id.button1);
			BackHome.Click += delegate {
				//create an intent to go back to home
				Intent intent = new Intent(this, typeof(Home));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
				StartActivity(intent);
			};	

			//save name in preferences
			//first check if there is already one saved and if yes put it in the field
			//http://developer.android.com/training/basics/data-storage/shared-preferences.html
			ISharedPreferences sharedPref = GetSharedPreferences("com.FSoft.are_u_ok.PREFERENCES",FileCreationMode.Private);
			String savedName = sharedPref.GetString ("Name", "");
			EditText NameEdit = FindViewById<EditText> (Resource.Id.editText1);
			if (savedName.Length > 0)
				NameEdit.Text = savedName;

			NameEdit.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
				//if text is entered, save the name in the preferences
				ISharedPreferencesEditor editor = sharedPref.Edit();
				editor.PutString("Name", e.Text.ToString() );
				editor.Commit ();
			};
				

			//determine current language and set radio buttons accordingly
			RadioButton RadioGerman = FindViewById<RadioButton> (Resource.Id.radioButton1);
			RadioButton RadioEnglish = FindViewById<RadioButton> (Resource.Id.radioButton2);
			Android.Content.Res.Configuration conf = this.Resources.Configuration;
			//Debug output
			//				Console.WriteLine("Language: {0}", conf.Locale.Language.ToString());

			if (conf.Locale.Language == "de") {
				RadioGerman.Checked = true;
			} 
			else {
				
				RadioEnglish.Checked = true;
			}

			//change language with radio buttons
			RadioGerman.Click += delegate {
				//try the concept of a Toast
				Toast toast = Toast.MakeText(this, "Switching to German", ToastLength.Short);
				//Toast toast = Toast.MakeText(this, Resources.GetText (Resource.String.Standard_Name), ToastLength.Short);
				toast.SetGravity(GravityFlags.Center,0,0);
				toast.Show();

				//set language to German and restart activity to see the effect
				conf.Locale = new Java.Util.Locale("de");
				Android.Util.DisplayMetrics dm = this.Resources.DisplayMetrics;
				this.Resources.UpdateConfiguration (conf, dm);

				//save language settings
				ISharedPreferencesEditor editor = sharedPref.Edit();
				editor.PutString("Language", "de" );
				editor.Commit ();
				
				Intent intent = new Intent(this, typeof(Settings));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history 
				StartActivity(intent);
			};

			RadioEnglish.Click += delegate {
				Toast toast = Toast.MakeText(this, "Wechseln nach Englisch", ToastLength.Short);
				toast.SetGravity(GravityFlags.Center,0,0);
				toast.Show();

				//set language to German and restart activity to see the effect
				conf.Locale = new Java.Util.Locale("en");
				//conf.SetLocale(Java.Util.Locale.English); //only supported from API 17 on
				Android.Util.DisplayMetrics dm = this.Resources.DisplayMetrics;
				this.Resources.UpdateConfiguration (conf, dm);

				//save language settings
				ISharedPreferencesEditor editor = sharedPref.Edit();
				editor.PutString("Language", "en" );
				editor.Commit ();

				Intent intent = new Intent(this, typeof(Settings));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history 
				StartActivity(intent);
			};

			//start sending reminders after the button was pressed
			Button ReminderButton = FindViewById<Button> (Resource.Id.button2);
			ReminderButton.Click += delegate {
				//Call setAlarm in the Receiver class
				AlarmReceiverQuestionnaire temp = new AlarmReceiverQuestionnaire();
				temp.SetAlarm(this);
				//Feedback message
				Toast toast = Toast.MakeText (this, GetString (Resource.String.Done), ToastLength.Short);
				toast.SetGravity (GravityFlags.Center, 0, 0);
				toast.Show ();
			};	

			//cancel all alarms
			Button CancelAlarmsButton = FindViewById<Button> (Resource.Id.button3);
			CancelAlarmsButton.Click += delegate {
				AlarmManager alarmMgr = (AlarmManager)GetSystemService(Context.AlarmService);
				//First cancel the Invalid Alarm
				Intent intentTemp = new Intent(this, typeof(AlarmReceiverInvalid));
				PendingIntent alarmIntent = PendingIntent.GetBroadcast(this, 0, intentTemp, 0);
				alarmMgr.Cancel(alarmIntent);
				//now the other alarm
				intentTemp = new Intent(this, typeof(AlarmReceiverQuestionnaire));
				alarmIntent = PendingIntent.GetBroadcast(this, 0, intentTemp, 0);
				alarmMgr.Cancel(alarmIntent);
				//also reset the state of the questionnaire avaliabilty
				ISharedPreferencesEditor editor = sharedPref.Edit();
				editor.PutBoolean("QuestionnaireActive", false );
				editor.Commit ();
				//Feedback message
				Toast toast = Toast.MakeText (this, GetString (Resource.String.Done), ToastLength.Short);
				toast.SetGravity (GravityFlags.Center, 0, 0);
				toast.Show ();
			};	

			//show an example questionnaire to demonstrate the app
			Button ExampleButton = FindViewById<Button> (Resource.Id.button4);
			ExampleButton.Click += delegate {
				Intent intent = new Intent(this, typeof(MoodAssessmentExample));
				StartActivity(intent);
			};	
				

		}
	}
}

