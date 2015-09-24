
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
	[Activity (Label =  "R-U-OK", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class MoodAssessment : Activity, SeekBar.IOnSeekBarChangeListener
	{
		SeekBar _seekBarHowAreYou;
		ImageView _sadHowAreYou;
		ImageView _happyHowAreYou;

		int nProgressHowAreYou; //saves the state of the progress bar for the How Are You question
		int nPeopleAround; //saves the state of the People Around question: -1 = not set, 0 = none, 1 = one, 2 = many
		int nWhere; //saves the state of the Where are you question: -1 = not set, 0 = away, 1 = home
		int nWhat; //saves the state of the What are you doing question: -1 = not set, 0 = Leisure, 1 = Eating, 2 = Work

		MoodDatabase dbMood;
		Android.Database.ICursor cursor;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.MoodAssessment);

			//CHECK HOW OFTEN WE ALREADY TESTED TODAY
			dbMood = new MoodDatabase(this);
			cursor = dbMood.ReadableDatabase.RawQuery("SELECT date FROM MoodData WHERE date = '" + DateTime.Now.ToString("dd.MM.yy") + "'", null); // cursor query
			if (cursor.Count >= 5) //IF ALREADY 5 OR MORE RETURN TO HOME
			{
				Toast toast = Toast.MakeText(this, "Already tested 5 times today", ToastLength.Long);
				toast.SetGravity(GravityFlags.Center,0,0);
				toast.Show();

				//create an intent to go back to the home screen
				Intent intent = new Intent(this, typeof(Home));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
				StartActivity(intent);	
			}

			//CHOOSE QUESTIONS TO ASK
			//Try to implement a scroll down option for the screen, use a scrollview: http://stackoverflow.com/questions/4055537/how-do-you-make-a-linearlayout-scrollable


			//SEEKBAR CODE
			_seekBarHowAreYou = FindViewById<SeekBar>(Resource.Id.seekBar1);
			_sadHowAreYou = FindViewById<ImageView> (Resource.Id.imageViewSadHowAreYou);
			_happyHowAreYou = FindViewById<ImageView> (Resource.Id.imageViewHappyHowAreYou);
			nProgressHowAreYou = _seekBarHowAreYou.Progress;

			// Assign this class as a listener for the SeekBar events
			// In order to be able to do so I have to put the ChangeListener into the definition of the class: extend Activity as well as the Listener
			// and implement all functions of the Listener, even if they do nothing
			_seekBarHowAreYou.SetOnSeekBarChangeListener(this);


			//PEOPLE AROUND CODE
			nPeopleAround = -1;

			Button ButtonNone = FindViewById<Button> (Resource.Id.buttonNone);
			Button ButtonOne = FindViewById<Button> (Resource.Id.buttonOne);
			Button ButtonMany = FindViewById<Button> (Resource.Id.buttonMany);

			ButtonNone.Click += delegate {
				ButtonOne.Background.ClearColorFilter();
				ButtonMany.Background.ClearColorFilter();
				ButtonNone.Background.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Darken);
				nPeopleAround = 0;
			};

			ButtonOne.Click += delegate {
				ButtonNone.Background.ClearColorFilter();
				ButtonMany.Background.ClearColorFilter();
				ButtonOne.Background.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Darken);
				nPeopleAround = 1;
			};

			ButtonMany.Click += delegate {
				ButtonMany.Background.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Darken);
				ButtonOne.Background.ClearColorFilter();
				ButtonNone.Background.ClearColorFilter();
				nPeopleAround = 2;
			};

			//WHAT ARE YOU DOING? CODE
			nWhat = -1;

			Button ButtonLeisure = FindViewById<Button> (Resource.Id.buttonLeisure);
			Button ButtonEating = FindViewById<Button> (Resource.Id.buttonEating);
			Button ButtonWork = FindViewById<Button> (Resource.Id.buttonWorking);

			ButtonLeisure.Click += delegate {
				ButtonEating.Background.ClearColorFilter();
				ButtonWork.Background.ClearColorFilter();
				ButtonLeisure.Background.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Darken);
				nWhat = 0;
			};

			ButtonEating.Click += delegate {
				ButtonLeisure.Background.ClearColorFilter();
				ButtonWork.Background.ClearColorFilter();
				ButtonEating.Background.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Darken);
				nWhat = 1;
			};

			ButtonWork.Click += delegate {
				ButtonEating.Background.ClearColorFilter();
				ButtonLeisure.Background.ClearColorFilter();
				ButtonWork.Background.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Darken);
				nWhat = 2;
			};

			nWhere = -1;

			Button ButtonAway = FindViewById<Button> (Resource.Id.buttonAway);
			Button ButtonHome = FindViewById<Button> (Resource.Id.buttonHome);

			ButtonAway.Click += delegate {
				ButtonHome.Background.ClearColorFilter();
				ButtonAway.Background.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Darken);
				nWhere = 0;
			};

			ButtonHome.Click += delegate {
				ButtonAway.Background.ClearColorFilter();
				ButtonHome.Background.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Darken);
				nWhere = 1;
			};


			//CONTINUE BUTTON CODE
			Button ContinueHome = FindViewById<Button> (Resource.Id.buttonContinue);
			ContinueHome.Click += delegate {
				//only possible if data has been correctly selected
				if ( (nPeopleAround == -1) || (nWhere == -1) || (nWhat == -1) ) {
					Toast toast = Toast.MakeText(this, Resource.String.AnswerQuestions, ToastLength.Short);
					toast.SetGravity(GravityFlags.Center,0,0);
					toast.Show();
				}
				else
				{
					//update database
					ContentValues insertValues = new ContentValues();
					insertValues.Put("date", DateTime.Now.ToString("dd.MM.yy"));
					insertValues.Put("time", DateTime.Now.ToString("HH:mm"));
					insertValues.Put("mood", nProgressHowAreYou);
					insertValues.Put("people", nPeopleAround);
					insertValues.Put("what", nWhat);
					insertValues.Put("location", nWhere);
					insertValues.Put("pos2", 2);

					dbMood.WritableDatabase.Insert ("MoodData", null, insertValues);

					//TEST CODE FOR QUERYING THE DB
					//cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData ORDER BY _id DESC LIMIT 2", null); // cursor query
					//select only these entries where mood = 4; later change to where date = today
					//cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData WHERE mood = 0 ORDER BY _id DESC LIMIT 3", null); // cursor query
					//cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData WHERE date = '24.09.15' ORDER BY _id DESC LIMIT 3", null); // cursor query
					//Select only entries from today
					//cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData WHERE date = '" + DateTime.Now.ToString("dd.MM.yy") + "' ORDER BY _id DESC", null); // cursor query
					//Select only entries for which the question pos2 has been answered
					cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData WHERE pos2 IS NOT NULL ORDER BY _id DESC LIMIT 3", null); // cursor query
					cursor.MoveToLast();
					Toast.MakeText(this, cursor.Count.ToString(), ToastLength.Short).Show();
					//Tutorial on SELECT: http://zetcode.com/db/sqlite/select/

					//HOW TO SELECT THE RANDOM QUESTIONS
					//if there is no entry with today's date we can select 2 random numbers between 1 and 5 to select one pos and one neg question
					//after that it depends on how many entries there are and we draw twice between 1 and (5 - #entries) 
					//we have to code for which questions have already been asked and which still are free
					//could do that by saving an integer and converting it to 10 single bit flags
					//test for the number of entries at the beginning and show a message if we already have 5 entries for today and return

					//create an intent to go to the next screen
					Intent intent = new Intent(this, typeof(Home));
					intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
					StartActivity(intent);
				}					
			};
		}

		public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
		{
			if (fromUser)
			{
				_sadHowAreYou.Alpha = (float)(seekBar.Max - seekBar.Progress) / seekBar.Max;
				_happyHowAreYou.Alpha = (float)seekBar.Progress / seekBar.Max;
				nProgressHowAreYou = seekBar.Progress;
			}
		}

		public void OnStartTrackingTouch(SeekBar seekBar)
		{
			//System.Diagnostics.Debug.WriteLine("Tracking changes.");
		}

		public void OnStopTrackingTouch(SeekBar seekBar)
		{
			//System.Diagnostics.Debug.WriteLine("Stopped tracking changes.");
		}
	}
}

