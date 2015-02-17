using System;
using System.Collections;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Feeder
{
	public class RssFeed
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public DateTime DateAdded { get; set; }
		public List<RssItems> Items { get; set; }


		public RssFeed()
		{
			Items = new List<RssItems> ();
		}
	}
}

