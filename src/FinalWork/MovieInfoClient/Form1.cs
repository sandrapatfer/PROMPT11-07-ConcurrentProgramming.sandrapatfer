using System;
using System.Drawing;
using System.Globalization;
using System.Json;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using Utils;

namespace MoviesInfo
{
	public partial class Form1 : Form
	{
		// Change URL service according to your configuration
        private const string MashupServiceEndpoint = "http://localhost:49822/MoviesAsync";
        private CancellationTokenSource _cancelToken;
        private bool _canceled;
	
		private static string PrepareServiceUrl(string movie, int year, string lang)
		{
			var sb = new StringBuilder(MashupServiceEndpoint);
			sb.Append("?t=");
			sb.Append(movie);
			if (year != 0)
			{
				sb.Append("&y=");
				sb.Append(year);
			}
			if (!String.IsNullOrEmpty(lang))
			{
				sb.Append("&l=");
				sb.Append(lang);
			}
			return sb.ToString();
		}

		public Form1()
		{
			InitializeComponent();
			posterBox.SizeMode = PictureBoxSizeMode.StretchImage;
		}

		private void ClearInput()
		{
			plotBox.Text = "";
			titleBox.Text = "";
			yearBox.Text = "";
			directorBox.Text = "";
			 
			if (posterBox.Image != null)
			{
				posterBox.Image.Dispose();
				posterBox.Image = null;

			}
			foreach (PictureBox b in photosPanel.Controls)
			{
				b.Dispose();
			}
			photosPanel.Controls.Clear();
			reviewsList.Items.Clear();
		}

		//Start a new search
		private void searchButton_Click(object sender, EventArgs e)
		{
			int year=0;

			if (String.IsNullOrEmpty(movieIn.Text))
			{
				MessageBox.Show("Must enter movie!");
				return;
			}
			if (!String.IsNullOrEmpty(yearIn.Text) &&
				!int.TryParse(yearIn.Text, NumberStyles.Integer, null,out year))
			{
				MessageBox.Show("Invalid year!");
				return;
			}
			ClearInput();
			searchButton.Enabled = false;
            _canceled = false;
			GetMovieInfo(movieIn.Text, year, langIn.Text);
		}

        Task<Image> TaskLoadingImageFromUrl(string location)
        {
            var httpClient = new HttpClient();
            var streamTask = httpClient.GetStreamAsync(location);
            var cts = new TaskCompletionSource<Image>();
            streamTask.ContinueWith(t =>
            {
                cts.SetResult(Image.FromStream(streamTask.Result));
            });
            return cts.Task;
        }

		// Change json names according to your result
		private void GetMovieInfo(string title, int year, string lang)
		{
            cancelButton.Enabled = true;

			SynchronizationContext guiCtx = SynchronizationContext.Current;

            var url = PrepareServiceUrl(title, year, lang);
            var httpClient = new HttpClient();
            _cancelToken = new CancellationTokenSource();
            var contentTask = httpClient.GetAsync(url, _cancelToken.Token);

            contentTask.ContinueWith(t =>
            {
                if (t.Status != TaskStatus.RanToCompletion)
                {
                    ActivateSearch(guiCtx);
                    return;
                }
                if (!t.Result.IsSuccessStatusCode)
                {
                    ActivateSearch(guiCtx);
                    MessageBox.Show("Error in request");
                    return;
                }

                var content = t.Result.Content;
                var readTask = content.ReadAsAsync<JsonValue>();

                readTask.ContinueWith(t2 =>
                {
                    if (_canceled)
                    {
                        ActivateSearch(guiCtx);
                        return;
                    }

                    dynamic json = t2.Result.AsDynamic();
                    guiCtx.Post(_ =>
                    {
                        // Show generic info
                        plotBox.Text = json.Plot;
                        titleBox.Text = json.Title;
                        directorBox.Text = json.Director;
                        yearBox.Text = json.Year;

                        // Show Reviews 
                        foreach (dynamic review in json.NYTimesReviews)
                        {
                            var item = reviewsList.Items.Add((string)review.Reviewer);
                            item.SubItems.Add((string)review.Summary);
                            item.SubItems.Add((string)review.Url);
                        }
                    }, null);

                    if (_canceled)
                    {
                        ActivateSearch(guiCtx);
                        return;
                    }

                    List<Task> pendingTasks = new List<Task>();

                    // Get Poster
                    var taskPosterImage = TaskLoadingImageFromUrl((string)json.CoverUrl);
                    pendingTasks.Add(taskPosterImage);
                    taskPosterImage.ContinueWith(t3 =>
                    {
                        if (_canceled)
                        {
                            ActivateSearch(guiCtx);
                            return;
                        }

                        guiCtx.Post(_ =>
                        {
                            posterBox.Image = t3.Result;
                        }, null);
                    });

                    // Show Related Photos
                    if (json.FlickrPhotos != null)
                    {
                        JsonArray photos = json.FlickrPhotos;
                        foreach (dynamic photo in photos)
                        {
                            if (_canceled)
                            {
                                ActivateSearch(guiCtx);
                                return;
                            }
                            var taskPhoto = TaskLoadingImageFromUrl((string)photo);
                            pendingTasks.Add(taskPhoto);
                            taskPhoto.ContinueWith(t4 =>
                            {
                                if (_canceled)
                                {
                                    ActivateSearch(guiCtx);
                                    return;
                                }
                                guiCtx.Post(_ =>
                                {
                                    // Create picture box
                                    var pbx = new PictureBox();
                                    photosPanel.Controls.Add(pbx);
                                    pbx.BorderStyle = BorderStyle.Fixed3D;
                                    pbx.Width = 200;
                                    pbx.Dock = DockStyle.Left;
                                    pbx.Image = t4.Result;
                                }, null);
                            });
                        }
                    }

                    TaskUtils.WhenAll(pendingTasks.ToArray()).ContinueWith(t5 =>
                    {
                        guiCtx.Post(_ =>
                        {
                            // Enable new search
                            searchButton.Enabled = true;
                            cancelButton.Enabled = false;
                        }, null);
                    });
                });
            });
		}

        private void ActivateSearch(SynchronizationContext guiCtx)
        {
            guiCtx.Post(_ =>
                {
                    searchButton.Enabled = true;
                    cancelButton.Enabled = false;
                }, null);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (_cancelToken != null)
            {
                if (!_cancelToken.IsCancellationRequested)
                {
                    _cancelToken.Cancel();
                }
            }
            _canceled = true;
        }
	}
}
