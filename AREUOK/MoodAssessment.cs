
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
		SeekBar _seekBarPosAffect;
		ImageView _LeftPosAffect;
		ImageView _RightPosAffect;
		SeekBar _seekBarNegAffect;
		ImageView _LeftNegAffect;
		ImageView _RightNegAffect;

		int nProgressHowAreYou; //saves the state of the progress bar for the How Are You question
		int nProgressPosAffect; //saves the state of the progress bar for the positive Affect question
		int nProgressNegAffect; //saves the state of the progress bar for the negative Affect question
		int nPeopleAround; //saves the state of the People Around question: -1 = not set, 0 = none, 1 = one, 2 = many
		int nWhere; //saves the state of the Where are you question: -1 = not set, 0 = away, 1 = home
		int nWhat; //saves the state of the What are you doing question: -1 = not set, 0 = Leisure, 1 = Eating, 2 = Work

		MoodDatabase dbMood;
		Android.Database.ICursor cursor;

		Random rnd;

		//use a scrollview: http://stackoverflow.com/questions/4055537/how-do-you-make-a-linearlayout-scrollable

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//CHECK HOW OFTEN WE ALREADY TESTED TODAY
			dbMood = new MoodDatabase(this);
			cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, QuestionFlags FROM MoodData WHERE date = '" + DateTime.Now.ToString("dd.MM.yy") + "'", null); // cursor query
			if (cursor.Count >= 5) { //IF ALREADY 5 OR MORE RETURN TO HOME
				Toast toast = Toast.MakeText (this, GetString (Resource.String.Already5Times), ToastLength.Long);
				toast.SetGravity (GravityFlags.Center, 0, 0);
				toast.Show ();

				//create an intent to go back to the home screen
				Intent intent = new Intent (this, typeof(Home));
				intent.SetFlags (ActivityFlags.ClearTop); //remove the history and go back to home screen
				StartActivity (intent);	
			} else {  //OTHERWISE LOAD THE SCREEN AND START

				// Create your application here
				SetContentView (Resource.Layout.MoodAssessment);

				//select random background picture
				//find view with background
				LinearLayout LayoutMood = FindViewById<LinearLayout>(Resource.Id.LinearLayoutMood);
				rnd = new Random(); //generator is seeded each time it is initialized
				switch (rnd.Next (11)) {
				case 0:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.clouds_low));
					break;
				case 1:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.beach_gauss));
					break;
				case 2:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.dawn_low));
					break;
				case 3:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.flower_pink_low));
					break;
				case 4:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.flower_red_low));
					break;
				case 5:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.forest_low));
					break;
				case 6:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.mountains_low));
					break;
				case 7:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.mountain_green_low));
					break;
				case 8:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.rice_low));
					break;
				case 9:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.rice_sun_low));
					break;
				case 10:
					LayoutMood.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.sea_low));
					break;
				}


				//CHOOSE QUESTIONS TO ASK
				//rnd = new Random(); //generator is seeded each time it is initialized

				//take latest db entry from today and read out which questions have already been asked.
				//if there is no enry yet, set already asked to 0
				int alreadyAsked = 0; //default value: no questions have been asked yet
				if (cursor.Count > 0) { //data was already saved today and questions have been asked, so retrieve which ones have been asked
					cursor.MoveToLast (); //take the last entry of today
					alreadyAsked = cursor.GetInt(cursor.GetColumnIndex("QuestionFlags")); //retrieve value from last entry in db column QuestionFlags
				}
				//This was for testing purposes
				//int alreadyAsked = 1 + 4 + 8 + 32 + 64 + 128 ; //only 1 and 4 are still valid options for pos and 3 & 4 for neg

				int posQuestion = ChoosePositiveQuestion (alreadyAsked);
				int negQuestion = ChooseNegativeQuestion (alreadyAsked);
				//update alreadyAsked with the newly chosen questions, only write to db after the save button has been pressed
				alreadyAsked += (int)Math.Pow(2,posQuestion) + (int)Math.Pow(2,negQuestion+5);
				//Toast.MakeText(this, "New Asked: " + alreadyAsked.ToString(), ToastLength.Short).Show();

				//Provide the chosen questions with seekbars and smileys. Make sure to pick good smileys for the negative affect questions
				//CODE FOR POSITIVE AND NEGATIVE SEEKBAR QUESTIONS
				TextView posAffectText = FindViewById<TextView>(Resource.Id.textViewPosAffect);
				TextView negAffectText = FindViewById<TextView>(Resource.Id.textViewNegAffect);
				switch (posQuestion) {
				case 0:
					posAffectText.Text = GetString(Resource.String.PosAffect1);
					break;
				case 1:
					posAffectText.Text = GetString(Resource.String.PosAffect2);
					break;
				case 2:
					posAffectText.Text = GetString(Resource.String.PosAffect3);
					break;
				case 3:
					posAffectText.Text = GetString(Resource.String.PosAffect4);
					break;
				case 4:
					posAffectText.Text = GetString(Resource.String.PosAffect5);					
					break;
				}
				_seekBarPosAffect = FindViewById<SeekBar>(Resource.Id.seekBarPosAffect);
				_LeftPosAffect = FindViewById<ImageView> (Resource.Id.imageViewLeftPosAffect);
				_RightPosAffect = FindViewById<ImageView> (Resource.Id.imageViewRightPosAffect);
				nProgressPosAffect = _seekBarPosAffect.Progress;
				_seekBarPosAffect.SetOnSeekBarChangeListener(this);

				_seekBarNegAffect = FindViewById<SeekBar>(Resource.Id.seekBarNegAffect);
				_LeftNegAffect = FindViewById<ImageView> (Resource.Id.imageViewLeftNegAffect);
				_RightNegAffect = FindViewById<ImageView> (Resource.Id.imageViewRightNegAffect);
				nProgressNegAffect = _seekBarNegAffect.Progress;
				_seekBarNegAffect.SetOnSeekBarChangeListener(this);

				switch (negQuestion) {
				case 0:
					negAffectText.Text = GetString(Resource.String.NegAffect1);
					break;
				case 1:
					negAffectText.Text = GetString(Resource.String.NegAffect2);
					//change sad smiley to afraid
					_RightNegAffect.SetImageResource(Resource.Drawable.afraid);
					break;
				case 2:
					negAffectText.Text = GetString(Resource.String.NegAffect3);
					break;
				case 3:
					negAffectText.Text = GetString(Resource.String.NegAffect4);
					//change sad smiley to agitated
					_RightNegAffect.SetImageResource(Resource.Drawable.Agitated);
					break;
				case 4:
					negAffectText.Text = GetString(Resource.String.NegAffect5);	
					//change sad smiley to angry
					_RightNegAffect.SetImageResource(Resource.Drawable.angry);
					break;
				}

				//SEEKBAR CODE FOR HOW ARE YOU
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
						insertValues.Put("QuestionFlags", alreadyAsked);
						//score affect questions between 1 and 9 instead of 0 and 8, because in the database question that have not been asked are stored as zeros.
						insertValues.Put("pos" + (posQuestion+1).ToString(), nProgressPosAffect+1); 
						insertValues.Put("neg" + (negQuestion+1).ToString(), nProgressNegAffect+1);


						dbMood.WritableDatabase.Insert ("MoodData", null, insertValues);

						//TEST CODE FOR QUERYING THE DB
						//cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData ORDER BY _id DESC LIMIT 2", null); // cursor query
						//select only these entries where mood = 4; later change to where date = today
						//cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData WHERE mood = 0 ORDER BY _id DESC LIMIT 3", null); // cursor query
						//cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData WHERE date = '24.09.15' ORDER BY _id DESC LIMIT 3", null); // cursor query
						//Select only entries from today
						//cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData WHERE date = '" + DateTime.Now.ToString("dd.MM.yy") + "' ORDER BY _id DESC", null); // cursor query
						//Select only entries for which the question pos2 has been answered
						//					cursor = dbMood.ReadableDatabase.RawQuery("SELECT date, mood FROM MoodData WHERE pos2 IS NOT NULL ORDER BY _id DESC LIMIT 3", null); // cursor query
						//					cursor.MoveToLast();
						//					Toast.MakeText(this, cursor.Count.ToString(), ToastLength.Short).Show();
						//Tutorial on SELECT: http://zetcode.com/db/sqlite/select/

						//create an intent to go to the next screen
						Intent intent = new Intent(this, typeof(Home));
						intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
						StartActivity(intent);
					}					
				};

			}

		}

		private int ChoosePositiveQuestion(int alreadyAskedFlags)
		{
			int questionToAsk = -1; //this will be the return value once a suitable question has been found
			while (questionToAsk == -1) {
				//draw random number between 0 and 4 calculate the power with base 2 in order to get the flag this question would set
				int rndQuest = rnd.Next (5);
				int pow2 = (int)(Math.Pow (2, rndQuest)); 
				//compare this to the already asked questions by doing a bitwise AND
				int bitAnd = pow2 & alreadyAskedFlags;
				//Toast.MakeText(this, "RND: " + rndQuest.ToString() + " BitAND: " + bitAnd.ToString(), ToastLength.Long).Show();
				if (bitAnd == 0)
					questionToAsk = rndQuest;
			}
			//Toast.MakeText(this, "Pos Question: " + questionToAsk.ToString(), ToastLength.Short).Show();
			return questionToAsk;
		}

		private int ChooseNegativeQuestion(int alreadyAskedFlags)
		{
			int questionToAsk = -1; //this will be the return value once a suitable question has been found
			while (questionToAsk == -1) {
				//draw random number between 0 and 4 calculate the power with base 2 in order to get the flag this question would set
				int rndQuest = rnd.Next (5);
				//Here we have to shift the value 5 bits (by adding 5 to the random number) in order to get to such higher numbers that we test bits 6 to 10
				int pow2 = (int)(Math.Pow (2, rndQuest + 5)); 
				//compare this to the already asked questions by doing a bitwise AND
				int bitAnd = pow2 & alreadyAskedFlags;
				//Toast.MakeText(this, "RND: " + rndQuest.ToString() + " BitAND: " + bitAnd.ToString(), ToastLength.Long).Show();
				if (bitAnd == 0)
					questionToAsk = rndQuest;
			}
			//Toast.MakeText(this, "Neg Question: " + questionToAsk.ToString(), ToastLength.Short).Show();
			return questionToAsk;
		}

		public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
		{
			if (fromUser)
			{
				switch (seekBar.Id) {
				case Resource.Id.seekBar1:
					_sadHowAreYou.Alpha = (float)(seekBar.Max - seekBar.Progress) / seekBar.Max;
					_happyHowAreYou.Alpha = (float)seekBar.Progress / seekBar.Max;
					nProgressHowAreYou = seekBar.Progress;
					break;
				case Resource.Id.seekBarPosAffect:
					_LeftPosAffect.Alpha = (float)(seekBar.Max - seekBar.Progress) / seekBar.Max;
					_RightPosAffect.Alpha = (float)seekBar.Progress / seekBar.Max;
					nProgressPosAffect = seekBar.Progress;
					break;
				case Resource.Id.seekBarNegAffect:
					_LeftNegAffect.Alpha = (float)(seekBar.Max - seekBar.Progress) / seekBar.Max;
					_RightNegAffect.Alpha = (float)seekBar.Progress / seekBar.Max;
					nProgressNegAffect = seekBar.Progress;
					break;
				}

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

		protected override void OnDestroy ()
		{
			cursor.Close();
			dbMood.Close ();
			base.OnDestroy();
		}
	}
}

