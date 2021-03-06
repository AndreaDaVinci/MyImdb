﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using Foundation;
using AppKit;
using MyImdbLibrary;

namespace ImdbMacApp
{
	public partial class ViewImdbMoviePage : NSViewController
	{
        public string MovieImdbId { get; set; }
        public ViewImdbMoviePage (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var url = new NSUrl(ConstAndParams.ImdbWebsite);
            if (MovieImdbId.Length > 0) url = new NSUrl(ConstAndParams.ImdbWebsite + "/title/" + MovieImdbId);

            var request = new NSUrlRequest(url);
            ImdbMoviePageView.LoadRequest(request);
        }
	}
}
