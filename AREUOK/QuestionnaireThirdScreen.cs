
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
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon")]			
	public class QuestionnaireThirdScreen : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.QuestionnaireThirdScreen);
			int Ergebnis = Intent.GetIntExtra ("ergebnis", 0);

			TextView ErgebnisText = FindViewById<TextView> (Resource.Id.textView3);
			ErgebnisText.Text = string.Format ("{0} {1}", Resources.GetText (Resource.String.Score), Ergebnis);

			Button ganzeZeit = FindViewById<Button> (Resource.Id.button1);
			ganzeZeit.Click += delegate {
				//create an intent to go to the next screen
				Intent intent = new Intent(this, typeof(QuestionnaireFourthScreen));
				//attach data to this extent: the current score + 5. If we come back to this activity the score will again be 0
				intent.PutExtra("ergebnis",Ergebnis+5);
				StartActivity(intent);
			};

			Button meistens = FindViewById<Button> (Resource.Id.button2);
			meistens.Click += delegate {
				Intent intent = new Intent(this, typeof(QuestionnaireFourthScreen));
				intent.PutExtra("ergebnis",Ergebnis+4);
				StartActivity(intent);
			};

			Button ueberHaelfte = FindViewById<Button> (Resource.Id.button3);
			ueberHaelfte.Click += delegate {
				Intent intent = new Intent(this, typeof(QuestionnaireFourthScreen));
				intent.PutExtra("ergebnis",Ergebnis+3);
				StartActivity(intent);
			};

			Button unterHaelfte = FindViewById<Button> (Resource.Id.button4);
			unterHaelfte.Click += delegate {
				Intent intent = new Intent(this, typeof(QuestionnaireFourthScreen));
				intent.PutExtra("ergebnis",Ergebnis+2);
				StartActivity(intent);
			};

			Button abundzu = FindViewById<Button> (Resource.Id.button5);
			abundzu.Click += delegate {
				Intent intent = new Intent(this, typeof(QuestionnaireFourthScreen));
				intent.PutExtra("ergebnis",Ergebnis+1);
				StartActivity(intent);
			};

			Button nie = FindViewById<Button> (Resource.Id.button6);
			nie.Click += delegate {
				Intent intent = new Intent(this, typeof(QuestionnaireFourthScreen));
				intent.PutExtra("ergebnis",Ergebnis+0);
				StartActivity(intent);
			};
		}
	}
}

