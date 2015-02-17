using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Object;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Feeder
{
	[Activity (Label = "Feeder", MainLauncher = true, Icon = "@drawable/icon")]
	public class FeedListActivity : Activity
	{
	
		private List<RssFeed> _Feeds;
		private ListView _FieldListView;
		private Button _AddFeedButton;
		private const string FEED_FILE_NAME = "FeedData.bin";
		private string _FilePath;

		public FeedListActivity()
		{
			_Feeds = new List<RssFeed> ();
			var path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			_FilePath = System.IO.Path.Combine (path, FEED_FILE_NAME);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.FeedList);
			_FieldListView = FindViewById<ListView> (Resource.Id.FeedList);
			_AddFeedButton = FindViewById<Button> (Resource.Id.AddFeedButton);

			_FieldListView.ItemClick +=  FeedListViewOnItemclick;
			_AddFeedButton.Click += AddFeedButtonOnClick;

			if( File.Exists(_FilePath) ){
				using (var fs = new FileStream (_FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {

					var Formatter = new BinaryFormatter();

					try{
						_Feeds = (List<RssFeed>)Formatter.Deserialize (fs);
					} catch(Exception e){
						Log.Error ("Feeder", "Encounterd an error attempting to deserialized feed data: {0}", e.Message);
					}

					if (_Feeds.Count () > 0) {
						UpdateList ();
					}

				}
			}

		}

		private void UpdateList()
		{
			_FieldListView.Adapter = new FieldListAdapter (_Feeds.ToArray(), this);
		}


		private void AddFeedButtonOnClick (object sender, EventArgs e)
		{
			var intent = new Intent(this, typeof(AddFeedActivity) );
			StartActivityForResult (intent, 0);
		}

		protected override void OnActivityResult( int requestCode, Result resultCode, Intent data) {
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == Result.Ok) {
				var url = data.GetStringExtra ("url");
				AddFeedUrl (url);
			}
		}

		private async void AddFeedUrl(string url){
			var newFeed = new RssFeed {
				DateAdded = DateTime.Now,
				Url = url
			};

			using (var client = new HttpClient ()) {
				var xmlFeed = await client.GetStringAsync ( url );
				var doc = new XDocument ( xmlFeed );

				var channel = doc.Descendants ("channel").FirstOrDefault().Element("title").Value;
				newFeed.Name = channel;

				XNamespace dc = "http://purl.org/dc/elements/1.1/";

				newFeed.Items = ( from item in doc.Descendants("item") select new RssItems{
					Title = item.Element("title").Value,
					PubDate = item.Element("pubDate").Value,
					Creator = item.Element("creator").Value,
					Link = item.Element("link").Value,
				}).ToList();

				_Feeds.Add (newFeed);
				UpdateList()

			}
		}

		private void FeedListViewOnItemclick (object sender, AdapterView.ItemClickEventArgs e)
		{
			var intent = new Intent ( this, typeof( FeedItemListActivity ) );
			var selectedFeed = _Feeds [e.Position];
			var feed = JsonConvert.SerializeObject (selectedFeed);
			intent.PutExtra ("feed", feed);

			StartActivity ( intent );

		}
	}
}


