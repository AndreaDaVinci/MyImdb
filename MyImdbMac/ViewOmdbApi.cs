﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using Foundation;
using AppKit;
using System.Collections.Generic;
using System.Linq;
using MyImdbLibrary;
using System.Threading.Tasks;

namespace ImdbMacApp
{
	public partial class ViewOmdbApi : NSViewController
	{
        public DatabaseManager DbManager { get; set; }
        public User MainUser { get; set; }
        public List<ApiModelOmdb> OmdbMovies { get; set; }
        public ApiModelOmdb CurrentOmdbMovie { get; set; }
        public string PresumedTitle;
        public string PresumedYear;
        private double imdbRating = 0;
        private int imdbVotes = 0;
        public ViewController MainView { get; set; }
        private int NbOfResults { get; set; }

        public ViewOmdbApi (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ClearForm();
        }

        public override void ViewDidAppear()
        {
            OmdbMovies = new List<ApiModelOmdb> { };
            SetButtonsStatus(ConstAndParams.ButtonsAndComboBoxState_Disabled);
            LoadMovie(reload: false);
        }

        private void ClearForm()
        {
            CkeckOnlineMoviePage.Enabled = false;

            YesButton.Hidden = true;
            NoButton.Hidden = true;
            CloseButton.Hidden = true;
            YesNoMessage.StringValue = "";

            MovieTitle.StringValue = "";
            MovieYear.StringValue = "";

            MoviePoster.Enabled = true;
            MoviePoster.Image = NSImage.ImageNamed("loading");
        }

        private void LoadMovie(bool reload)
        {
            ClearForm();

            if (reload == false)
            {
                // Extra results from the page 1 API
                OmdbMovies = ApiProcessorOmdb.LoadOmdbList(PresumedTitle, PresumedYear, MainUser.OmdbApiKey);
                // Main result from the regular API
                ApiModelOmdb MainMovie = ApiProcessorOmdb.LoadOmdbObject(PresumedTitle, PresumedYear, MainUser.OmdbApiKey);
                if (MainMovie != null)
                {
                    if (OmdbMovies != null)
                    {
                        int index = OmdbMovies.FindIndex(f => f.imdbID == MainMovie.imdbID);
                        if (index < 0) OmdbMovies.Insert(0, MainMovie);
                        else OmdbMovies[index] = MainMovie;
                    }
                    else OmdbMovies = new List<ApiModelOmdb> { MainMovie };
                }
                else if (OmdbMovies == null)
                {
                    MoviePoster.Image = NSImage.ImageNamed("heyYou");
                    MessageToUser.StringValue = "No results!";
                    MessageToUser2.StringValue = "Please check if your API key is still valid!";
                    FailureLoadingMovie();
                    return;
                }
                NbOfResults = OmdbMovies.Count;
            }
            if ((OmdbMovies != null) && (OmdbMovies.Count > 0))
            {
                try
                {
                    try
                    {
                        MessageToUser.StringValue = string.Format("Total results {0}", NbOfResults);
                        MessageToUser2.StringValue = string.Format("{0} results left.", OmdbMovies.Count - 1);
                        CkeckOnlineMoviePage.Enabled = true;

                        CurrentOmdbMovie = OmdbMovies.First();
                    }
                    catch (Exception) { }
                    try
                    {
                        if (!string.IsNullOrEmpty(CurrentOmdbMovie.Poster) && (!CurrentOmdbMovie.Poster.Contains("N/A")))
                        {
                            MoviePoster.Enabled = true;
                            Uri uriSource = new Uri(CurrentOmdbMovie.Poster, UriKind.Absolute);
                            MoviePoster.Image = new NSImage(uriSource);
                        }
                        else MoviePoster.Image = NSImage.ImageNamed("unavailable");
                    }
                    catch (Exception)
                    {
                        MoviePoster.Image = NSImage.ImageNamed("unavailable");
                    }
                    try
                    {
                        if (!string.IsNullOrEmpty(CurrentOmdbMovie.Title))
                        {
                            MovieTitle.Enabled = true;
                            MovieTitle.StringValue = CurrentOmdbMovie.Title;
                        }
                    }
                    catch (Exception) { }
                    try
                    {
                        if (!string.IsNullOrEmpty(CurrentOmdbMovie.Year))
                        {
                            MovieYear.Enabled = true;
                            MovieYear.StringValue = CurrentOmdbMovie.Year;
                        }
                    }
                    catch (Exception) { }

                    SuccessLoadingMovie();
                }
                catch
                {
                    FailureLoadingMovie();
                }
            }
            else
            {
                MoviePoster.Image = NSImage.ImageNamed("no_results");
                MessageToUser.StringValue = "Movie Not Found!";
                MessageToUser2.StringValue = "";

                FailureLoadingMovie();
            }
        }

        private void SuccessLoadingMovie()
        {
            YesNoMessage.StringValue = "Is this the correct movie ?";
            SetButtonsStatus(ConstAndParams.ButtonsAndComboBoxState_Enabled);
            YesButton.Hidden = false;
            NoButton.Hidden = false;
        }

        private void SetButtonsStatus(bool status)
        {
            YesButton.Enabled = status;
            NoButton.Enabled = status;
        }

        private void FailureLoadingMovie()
        {
            YesButton.Hidden = true;
            NoButton.Hidden = true;
            CloseButton.Hidden = false;
            CkeckOnlineMoviePage.Enabled = false;
            YesNoMessage.Hidden = true;
        }

        private async Task CompleteMovieInfo(string imdb_id)
        {
            if (!string.IsNullOrEmpty(imdb_id))
            {
                ApiModelOmdb movieFromId = await ApiProcessorOmdb.LoadOmdbFromId(imdb_id, MainUser.OmdbApiKey);

                if (!string.IsNullOrEmpty(movieFromId.imdbRating)) double.TryParse(movieFromId.imdbRating, out imdbRating);
                if (!string.IsNullOrEmpty(movieFromId.imdbVotes)) int.TryParse(movieFromId.imdbVotes.Replace(",",""), out imdbVotes);
                if (!string.IsNullOrEmpty(movieFromId.Country)) CurrentOmdbMovie.Country = movieFromId.Country;
                if (!string.IsNullOrEmpty(movieFromId.Genre)) CurrentOmdbMovie.Genre = movieFromId.Genre;
            }
        }

        [Action("YesButton_Clicked:")]
        private async void YesButton_Clicked(NSObject sender)
        {
            SetButtonsStatus(ConstAndParams.ButtonsAndComboBoxState_Disabled);

            if (!string.IsNullOrEmpty(CurrentOmdbMovie.imdbID))
            {
                if (string.IsNullOrEmpty(CurrentOmdbMovie.imdbVotes) || string.IsNullOrEmpty(CurrentOmdbMovie.Country) || string.IsNullOrEmpty(CurrentOmdbMovie.Genre))
                {
                    await CompleteMovieInfo(CurrentOmdbMovie.imdbID);
                }
                else
                {
                    double.TryParse(CurrentOmdbMovie.imdbRating, out imdbRating);
                    int.TryParse(CurrentOmdbMovie.imdbVotes.Replace(",", ""), out imdbVotes);
                }

                int.TryParse(CurrentOmdbMovie.Year, out int year);

                Movie NewMovie = new Movie(CurrentOmdbMovie.Title, year, CurrentOmdbMovie.Poster, CurrentOmdbMovie.imdbID, CurrentOmdbMovie.Plot);

                List<int> Countries_Ids = new List<int>();
                if (!string.IsNullOrEmpty(CurrentOmdbMovie.Country))
                {
                    Countries_Ids = JsonTextProcessor.ExtractCountriesIds(CurrentOmdbMovie.Country, DbManager.countriesNamesAndCodes);
                }

                List<int> Genres = new List<int> { };
                if (!string.IsNullOrEmpty(CurrentOmdbMovie.Genre))
                {
                    foreach (var t in DbManager.moviesTypes)
                    {
                        if (CurrentOmdbMovie.Genre.Contains(t.Name)) Genres.Add(t.Id);
                    }
                }

                if (DbManager.IsThereAnExistingMovie(NewMovie.imdbID))
                {
                    if (DbManager.IsThereAnExistingMovieForUser(NewMovie.imdbID, MainUser.Id))
                    {
                        DismissViewController(this);
                        MainView.AlertMessage("This movie already exists in your list of movies.", "Already exists!");
                        NewMovie = DbManager.GetMovieByImdbId(NewMovie.imdbID);
                        MainView.ReloadDisplay(NewMovie);
                    }
                    else
                    {
                        DismissViewController(this);
                        MainView.AddMovieToDbAndReload(false, NewMovie, Countries_Ids, Genres, ConstAndParams.ImdbOnlineUserId, imdbRating, imdbVotes);
                    }
                }
                else
                {
                    DismissViewController(this);
                    MainView.AddMovieToDbAndReload(true, NewMovie, Countries_Ids, Genres, ConstAndParams.ImdbOnlineUserId, imdbRating, imdbVotes);
                }
            }
            else
            {
                // Impossible to add a movie without imdbID
                DismissViewController(this);
                MainView.AlertMessage("The attempt to link this movie to the online database was not successful!", "Could not add this movie!");
            }
        }

        partial void NoButton_Clicked(NSObject sender)
        {
            if ((OmdbMovies != null) && (OmdbMovies.Count > 1))
            {
                OmdbMovies.Remove(CurrentOmdbMovie);
                LoadMovie(reload: true);
            }
            else DismissViewController(this);
        }

        partial void CloseButton_Clicked(NSObject sender)
        {
            DismissViewController(this);
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
                        target.MovieImdbId = CurrentOmdbMovie.imdbID;
                    }
                    break;
            }
        }
    }
}
