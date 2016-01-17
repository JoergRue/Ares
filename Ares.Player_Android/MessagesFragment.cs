
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Ares.Player_Android
{
	public class MessagesFragment : Fragment
	{
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Settings.Settings.Instance.Initialize(Application.Context);
		}

		private ListView mListView;
		private MessagesListAdapter mListAdapter;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.MessagesFragment, container, false);

			mListAdapter = new MessagesListAdapter(Activity);

			Button clearButton = view.FindViewById<Button>(Resource.Id.clearButton);
			clearButton.Click += delegate {
				ClearMessages();
			};

			Spinner filterSpinner = view.FindViewById<Spinner>(Resource.Id.filterSpinner);
			var arrayAdapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.filterChoices, Android.Resource.Layout.SimpleSpinnerItem);
			arrayAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			filterSpinner.Adapter = arrayAdapter;
			filterSpinner.SetSelection(Ares.Settings.Settings.Instance.MessageFilterLevel);
			filterSpinner.ItemSelected += delegate(object sender, AdapterView.ItemSelectedEventArgs e) {
				ChangeFilter(e.Position);
			};

			mListView = view.FindViewById<ListView>(Resource.Id.messagesList);
			mListView.Adapter = mListAdapter;

			return view;
		}

		public override void OnStart()
		{
			base.OnStart();
			RefillList();
			Ares.Players.Messages.Instance.MessageReceived += MessageReceived;
		}

		public override void OnStop()
		{
			Ares.Players.Messages.Instance.MessageReceived -= MessageReceived;
			base.OnStop();
		}

		private void MessageReceived (Ares.Players.Message m)
		{
			mListView.Post(() => {
				if ((int)m.Type >= Ares.Settings.Settings.Instance.MessageFilterLevel)
				{
					mListAdapter.Add(m);
					mListView.SetSelection(mListAdapter.Count - 1);
				}
			});
		}

		private class MessagesListAdapter : ArrayAdapter<Ares.Players.Message>
		{
			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				var inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
				var rowView = inflater.Inflate(Resource.Layout.MessagesListEntry, parent, false);
				var textView = rowView.FindViewById<TextView>(Resource.Id.messageLine);
				var imageView = rowView.FindViewById<ImageView>(Resource.Id.messageIcon);

				var message = GetItem(position);
				textView.Text = message.Text;
				switch (message.Type)
				{
				case Ares.Players.MessageType.Debug:
					imageView.SetImageResource(Android.Resource.Drawable.IcMenuManage);
					break;
				case Ares.Players.MessageType.Info:
					imageView.SetImageResource(Android.Resource.Drawable.IcMenuInfoDetails);
					break;
				case Ares.Players.MessageType.Warning:
					imageView.SetImageResource(Android.Resource.Drawable.StatNotifyError);
					break;
				case Ares.Players.MessageType.Error:
				default:
					imageView.SetImageResource(Resource.Drawable.eventlogError);
					break;
				}
				return rowView;
			}

			public MessagesListAdapter(Context context)
				: base(context, Resource.Layout.MessagesListEntry, new List<Ares.Players.Message>())
			{
			}
		}

		private void ChangeFilter(int newFilterPosition)
		{
			Ares.Settings.Settings.Instance.MessageFilterLevel = newFilterPosition;
			Ares.Settings.Settings.Instance.WriteMessageLevel(Application.Context);
			RefillList();
		}

		private void RefillList()
		{
			mListAdapter.Clear();
			List<Ares.Players.Message> newList = new List<Ares.Players.Message>();
			int filterLevel = Ares.Settings.Settings.Instance.MessageFilterLevel;
			foreach (var message in Ares.Players.Messages.Instance.GetAllMessages())
			{
				if ((int)message.Type >= filterLevel)
				{
					newList.Add(message);
				}
			}
			mListAdapter.AddAll(newList);
		}

		private void ClearMessages()
		{
			Ares.Players.Messages.Instance.Clear();
			mListAdapter.Clear();
		}
	}
}

