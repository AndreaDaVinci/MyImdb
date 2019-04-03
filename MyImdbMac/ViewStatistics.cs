﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using Foundation;
using AppKit;
using System.Collections.Generic;
using MyImdbLibrary;
using System.Linq;

namespace ImdbMacApp
{
    public partial class ViewStatistics : NSViewController
    {
        public DatabaseManager DbManager { get; set; }
        public User MainUser { get; set; }
        private string ImdbIdForOnlineView { get; set; }

        private Dictionary<int, double> UserGenresRating { get; set; }
        private Dictionary<int, double> ImdbGenresRatingForUserMovies { get; set; }

        private Dictionary<int, (double, double)> DictUserImdbGenresRatings { get; set; }

        private Dictionary<int, int> DictMatchLevels { get; set; }

        private List<int> UserFiveMostRatedMoviesByImdbIds { get; set; }
        private List<Movie> UserFiveMostRatedMoviesByImdb { get; set; }

        private List<NSLevelIndicator> RatingBars { get; set; }
        private List<NSLevelIndicator> MatchBars { get; set; }

        private List<NSImageView> PreLoaders { get; set; }
        private List<NSImageView> Posters { get; set; }
        private List<NSTextField> Titles { get; set; }
        private List<NSTextField> Ratings { get; set; }
        private List<NSButton> CkeckOnlineButtons { get; set; }
        private List<NSTextField> Genres { get; set; }
        private List<NSTextField> ImdbVotes { get; set; }
        private List<NSTextField> Plots { get; set; }
        
        public bool ConnectedToInternet { get; set; }

        public ViewStatistics(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            DictUserImdbGenresRatings = new Dictionary<int, (double, double)>();
            DictMatchLevels = new Dictionary<int, int>();
            UserFiveMostRatedMoviesByImdb = new List<Movie>();

            PreLoaders          = new List<NSImageView> { MovieLoading1, MovieLoading2, MovieLoading3, MovieLoading4, MovieLoading5 };
            Posters             = new List<NSImageView> { MoviePoster1, MoviePoster2, MoviePoster3, MoviePoster4, MoviePoster5 };
            Titles              = new List<NSTextField> { MovieTitle1, MovieTitle2, MovieTitle3, MovieTitle4, MovieTitle5 };
            Ratings             = new List<NSTextField> { MovieRating1, MovieRating2, MovieRating3, MovieRating4, MovieRating5 };
            CkeckOnlineButtons  = new List<NSButton> { CheckOnline1, CheckOnline2, CheckOnline3, CheckOnline4, CheckOnline5 };
            Genres              = new List<NSTextField> { MovieGenres1, MovieGenres2, MovieGenres3, MovieGenres4, MovieGenres5 };
            ImdbVotes           = new List<NSTextField> { MovieImdbVotes1, MovieImdbVotes2, MovieImdbVotes3, MovieImdbVotes4, MovieImdbVotes5 };
            Plots               = new List<NSTextField> { MoviePlot1, MoviePlot2, MoviePlot3, MoviePlot4, MoviePlot5 };

            RatingBars = new List<NSLevelIndicator> { RatingBar1, RatingBar2, RatingBar3, RatingBar4, RatingBar5, RatingBar6, RatingBar7, RatingBar8, RatingBar9, RatingBar10, RatingBar11, RatingBar12, RatingBar13 };
            MatchBars = new List<NSLevelIndicator> { MatchBar1, MatchBar2, MatchBar3, MatchBar4, MatchBar5, MatchBar6, MatchBar7, MatchBar8, MatchBar9, MatchBar10, MatchBar11, MatchBar12, MatchBar13 };
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();

            EnablePreLoaderLeft1();
            EnablePreLoaderLeft2();
            EnablePreLoadersRight();

            UserInfo.StringValue = string.Format("User: {0} \nLast seen on: {1:ddd dd-MMM-yyyy}\nat: {1:T}", new object[] { MainUser.FirstName, MainUser.LastSeen.ToLocalTime() });

            GetDataAndLoadUserTopFiveMovies();
            GetDataAndLoadUserTopRatedGenres();
            GetDataAndLoadMatchAnalysis();

            CloseButton.Enabled = true;
        }

        private void GetDataAndLoadUserTopFiveMovies()
        {
            UserFiveMostRatedMoviesByImdbIds = DbManager.GetUserFiveMostRatedMoviesByImdbIds(MainUser.Id);

            FillMovieSection(0);
            FillMovieSection(1);
            FillMovieSection(2);
            FillMovieSection(3);
            FillMovieSection(4);
        }

        private void GetDataAndLoadUserTopRatedGenres()
        {
            UserGenresRating = DbManager.GetUserTopRatedGenres(MainUser.Id);

            UserGenresRating = UserGenresRating.OrderByDescending(x => x.Value).ToDictionary(t => t.Key, t => t.Value);

            FillTopRatedGenres();

            DisablePreLoaderLeft1();
        }

        private void GetDataAndLoadMatchAnalysis()
        {
            ImdbGenresRatingForUserMovies = DbManager.GetImdbTopRatedGenresForUserMovies(MainUser.Id);

            CalculateMatchLevels();

            DictMatchLevels = DictMatchLevels.OrderByDescending(x => x.Value).ToDictionary(t => t.Key, t => t.Value);

            FillMatchLevels();

            DisablePreLoaderLeft2();
        }

        private void CalculateMatchLevels()
        {
            foreach (MovieTypeEnum mt in DbManager.moviesTypes)
            {
                DictMatchLevels.Add(mt.Id, 0);

                (double, double) couple = (0, 0);

                if (UserGenresRating[mt.Id] > 0) couple.Item1 = UserGenresRating[mt.Id];

                if (ImdbGenresRatingForUserMovies[mt.Id] > 0) couple.Item2 = ImdbGenresRatingForUserMovies[mt.Id];

                DictUserImdbGenresRatings.Add(mt.Id, couple);
            }

            foreach (int genre_id in DictUserImdbGenresRatings.Keys)
            {
                int match = 0;

                double r1 = DictUserImdbGenresRatings[genre_id].Item1;
                double r2 = DictUserImdbGenresRatings[genre_id].Item2;

                if ((r1 > 0) && (r2 > 0))
                {
                    if (r2 <= r1) match = (int)(100 * r2 / r1);
                    else match = (int)(100 * r1 / r2);

                    if (match > 0) DictMatchLevels[genre_id] = match;
                }
            }
        }

        private void FillTopRatedGenres()
        {
            string SortedGenresRatings = "";
            string SortedGenresNames = "";

            int i = 0;
            foreach (int genre_id in UserGenresRating.Keys)
            {
                SortedGenresRatings += string.Format("{0:F1}", UserGenresRating[genre_id]) + "\n";
                SortedGenresNames += DbManager.moviesTypes.First(mt => mt.Id == genre_id).Name + "\n";
                RatingBars[i].DoubleValue = UserGenresRating[genre_id];
                i++;
            }

            TopRatedNumbers.StringValue = SortedGenresRatings;
            TopRatedGenres.StringValue = SortedGenresNames;
        }

        private void FillMatchLevels()
        {
            string SortedGenresMatchLevels = "";
            string SortedGenresNames = "";

            int i = 0;
            foreach (int genre_id in DictMatchLevels.Keys)
            {
                SortedGenresMatchLevels += DictMatchLevels[genre_id].ToString() + "%\n";
                SortedGenresNames += DbManager.moviesTypes.First(mt => mt.Id == genre_id).Name + "\n";
                MatchBars[i].IntValue = DictMatchLevels[genre_id];
                i++;
            }

            MatchNumbers.StringValue = SortedGenresMatchLevels;
            MatchGenres.StringValue = SortedGenresNames;
        }

        private void FillMovieSection(int i)
        {
            Movie movie = DbManager.GetMovieById(UserFiveMostRatedMoviesByImdbIds[i]);
            UserFiveMostRatedMoviesByImdb.Add(movie);

            Titles[i].StringValue = movie.Title;
            Ratings[i].StringValue = string.Format("{0:F1}", DbManager.GetMovieImdbRating(movie.Id));
            ImdbVotes[i].StringValue = string.Format("{0:n0} IMDB ratings", DbManager.GetMovieNumberOfImdbRatings(movie.Id));
            Genres[i].StringValue = DbManager.GetMovieTypesNames(movie.Id);
            if (!string.IsNullOrEmpty(movie.Plot)) Plots[i].StringValue = movie.Plot;

            if (ConnectedToInternet)
            {
                CkeckOnlineButtons[i].Enabled = true;
                try
                {
                    if ((movie.Poster.Any()) && (!movie.Poster.Contains("N/A")))
                    {
                        Uri uriSource = new Uri(movie.Poster, UriKind.Absolute);
                        Posters[i].Image = new NSImage(uriSource);
                    }
                    else Posters[i].Image = NSImage.ImageNamed("unavailable");
                }
                catch (Exception)
                {
                    Posters[i].Image = NSImage.ImageNamed("unavailable");
                }
            }
            else
            {
                Posters[i].Image = NSImage.ImageNamed("no_connection");
            }
            DisablePreLoaderRight(PreLoaders[i]);
        }

        partial void CloseButton_Clicked(NSObject sender)
        {
            DismissViewController(this);
        }

        private void EnablePreLoaderLeft1()
        {
            LoadingGifLeft1.Image = NSImage.ImageNamed("loading2");
            LoadingGifLeft1.Enabled = true;
        }

        private void DisablePreLoaderLeft1()
        {
            LoadingGifLeft1.Image = null;
            LoadingGifLeft1.Enabled = false;
        }

        private void EnablePreLoaderLeft2()
        {
            LoadingGifLeft2.Image = NSImage.ImageNamed("loading2");
            LoadingGifLeft2.Enabled = true;
        }

        private void DisablePreLoaderLeft2()
        {
            LoadingGifLeft2.Image = null;
            LoadingGifLeft2.Enabled = false;
        }

        private void EnablePreLoadersRight()
        {
            foreach (NSImageView Gif in PreLoaders)
            {
                Gif.Image = NSImage.ImageNamed("loading2");
                Gif.Enabled = true;
            }
        }

        private void DisablePreLoaderRight(NSImageView Gif)
        {
            Gif.Image = null;
            Gif.Enabled = false;
        }

        partial void CheckOnline1_Clicked(NSObject sender)
        {
            ImdbIdForOnlineView = UserFiveMostRatedMoviesByImdb[0].imdbID;
            if (!string.IsNullOrEmpty(ImdbIdForOnlineView)) PerformSegue("LaunchViewImdbMoviePage", this);
        }

        partial void CheckOnline2_Clicked(NSObject sender)
        {
            ImdbIdForOnlineView = UserFiveMostRatedMoviesByImdb[1].imdbID;
            if (!string.IsNullOrEmpty(ImdbIdForOnlineView)) PerformSegue("LaunchViewImdbMoviePage", this);
        }

        [Export("prepareForSegue:sender:")]
        public override void PrepareForSegue(NSStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            switch (segue.Identifier)
            {
                case "LaunchViewImdbMoviePage":
                    {
                        ViewImdbMoviePage target = segue.DestinationController as ViewImdbMoviePage;
                        target.MovieImdbId = ImdbIdForOnlineView;
                    }
                    break;
            }
        }
    }
}
