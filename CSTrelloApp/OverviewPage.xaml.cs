using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSTrelloApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OverviewPage : Page
    {
        private readonly ObservableCollection<Data.Task> _toDo = new();
        private readonly ObservableCollection<Data.Task> _doing = new();
        private readonly ObservableCollection<Data.Task> _inReview = new();
        private readonly ObservableCollection<Data.Task> _done = new();

        public OverviewPage()
        {
            InitializeComponent();

            ToDoList.ItemsSource = _toDo;
            DoingList.ItemsSource = _doing;
            InReviewList.ItemsSource = _inReview;
            DoneList.ItemsSource = _done;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadTasksAsync();
        }

        private async Task LoadTasksAsync()
        {
            ErrorTextBlock.Text = "";
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            string apiUrl = "http://localhost:8080/all";

            try
            {
                var json = await client.GetStringAsync(apiUrl).ConfigureAwait(false);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var tasks = JsonSerializer.Deserialize<System.Collections.Generic.List<Data.Task>>(json, options);
                
                if (tasks == null)
                {
                    // Try single object if array deserialization failed
                    var single = JsonSerializer.Deserialize<Data.Task>(json, options);
                    if (single != null)
                    {
                        tasks = new System.Collections.Generic.List<Data.Task> { single };
                    }
                }

                if (tasks == null)
                {
                    await DispatcherQueue.EnqueueAsync(() => ErrorTextBlock.Text = "Unexpected response format from server.");
                    return;
                }

                // Clear existing and distribute tasks to columns
                await DispatcherQueue.EnqueueAsync(() =>
                {
                    _toDo.Clear();
                    _doing.Clear();
                    _inReview.Clear();
                    _done.Clear();

                    foreach (var t in tasks)
                    {
                        var status = (t.Status ?? string.Empty).Trim().ToLowerInvariant();
                        if (status == "to do" || status == "todo")
                        {
                            _toDo.Add(t);
                        }
                        else if (status == "in progress" || status == "doing")
                        {
                            _doing.Add(t);
                        }
                        else if (status == "in review" || status == "review")
                        {
                            _inReview.Add(t);
                        }
                        else if (status == "completed" || status == "done")
                        {
                            _done.Add(t);
                        }
                        else
                        {
                            // Unknown status -> put in ToDo by default
                            _toDo.Add(t);
                        }
                    }
                });
            }
            catch (HttpRequestException ex)
            {
                await DispatcherQueue.EnqueueAsync(() => ErrorTextBlock.Text = $"Network error: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                await DispatcherQueue.EnqueueAsync(() => ErrorTextBlock.Text = "Request timed out.");
            }
            catch (JsonException ex)
            {
                await DispatcherQueue.EnqueueAsync(() => ErrorTextBlock.Text = $"JSON parse error: {ex.Message}");
            }
            catch (Exception ex)
            {
                await DispatcherQueue.EnqueueAsync(() => ErrorTextBlock.Text = $"Unexpected error: {ex.Message}");
            }
        }
    }

    // Helper extension to marshal to UI thread
    internal static class DispatcherQueueExtensions
    {
        public static System.Threading.Tasks.Task EnqueueAsync(this Microsoft.UI.Dispatching.DispatcherQueue queue, Action action)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<object?>();
            queue.TryEnqueue(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    }
}
