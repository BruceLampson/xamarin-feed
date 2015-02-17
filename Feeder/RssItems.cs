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
	public class RssItems
	{
		public string Title { get; set; }
		public DateTime PubDate { get; set; }
		public string Creator { get; set; }
		public string Link { get; set; }
	}
}

