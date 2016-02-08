using System;
using Android.App;
using Android.Widget;
using Android.Text;

namespace Ares.Player_Android
{
	public class UsageDialogFragment : DialogFragment
	{
		public UsageDialogFragment()
		{
		}

		public class MyTagHandler : Java.Lang.Object, Android.Text.Html.ITagHandler {
			bool first= true;
			int index=1;

			public void HandleTag(bool opening, string tag, IEditable output, Org.Xml.Sax.IXMLReader xmlReader)
			{
				if (tag == "li") 
				{
					if (first)
					{
						output.Append("\n\t" + index + ". ");
						first = false;
						index++;
					}
					else
					{
						first = true;
					}
				}
			}
		}

		public override Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
			var inflater = Activity.LayoutInflater;
			var view = inflater.Inflate(Resource.Layout.Usage, null);
			/*
			var musicUsage = view.FindViewById<TextView>(Resource.Id.musicUsage);
			musicUsage.Text = String.Format(Resources.GetString(Resource.String.usage3), Ares.Settings.Settings.Instance.MusicFolder.DisplayName);
			var soundUsage = view.FindViewById<TextView>(Resource.Id.soundUsage);
			soundUsage.Text = String.Format(Resources.GetString(Resource.String.usage4), Ares.Settings.Settings.Instance.SoundFolder.DisplayName);
			var projectUsage = view.FindViewById<TextView>(Resource.Id.projectUsage);
			projectUsage.Text = String.Format(Resources.GetString(Resource.String.usage5), Ares.Settings.Settings.Instance.ProjectFolder.DisplayName);
			*/
			var usageText = view.FindViewById<TextView>(Resource.Id.usageText);
			usageText.SetText(Android.Text.Html.FromHtml(String.Format(Resources.GetString(Resource.String.usageHtml),
				Ares.Settings.Settings.Instance.MusicFolder.DisplayName,
				Ares.Settings.Settings.Instance.SoundFolder.DisplayName,
				Ares.Settings.Settings.Instance.ProjectFolder.DisplayName
			), null, new MyTagHandler()), TextView.BufferType.Spannable);
			builder.SetView(view);
			builder.SetPositiveButton(Android.Resource.String.Ok, (source, args) => {
			});
			return builder.Create();
		}
	}
}

