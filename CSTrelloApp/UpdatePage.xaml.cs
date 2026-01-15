using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CSTrelloApp.Data;

namespace CSTrelloApp
{
    public sealed partial class UpdatePage : Page
    {
        private int id = 0;

        public UpdatePage()
        {
            InitializeComponent();
            HttpClient client = new HttpClient();
            string apiUrl = "http://localhost:8080/users";
            var response = client.GetAsync(apiUrl).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Data.User> users = JsonSerializer.Deserialize<List<Data.User>>(response.Content.ReadAsStringAsync().Result);
                foreach (var user in users)
                {
                    UserSelection.Items.Add(user.Name);
                }
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Validate ID
            if (e.Parameter is not int passedId || passedId == 0)
            {
                errorsTextBlock.Text = "No valid ID provided for update.";
                return;
            }

            this.id = passedId;

            // Load task directly in OnNavigatedTo
            try
            {
                using var client = new HttpClient();
                string apiUrl = "http://localhost:8080/all";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var tasks = JsonSerializer.Deserialize<List<Data.Task>>(json);

                    var task = tasks?.FirstOrDefault(t => t.Id == id);

                    if (task != null)
                    {
                        TaskTitle.Text = task.Title ?? "";
                        TaskDescription.Text = task.Description ?? "";
                        UserSelection.SelectedItem = task.AssignedTo;
                    }
                    else
                    {
                        errorsTextBlock.Text = "Task not found.";
                    }
                }
                else
                {
                    errorsTextBlock.Text = "Failed to load task data.";
                }
            }
            catch (Exception ex)
            {
                errorsTextBlock.Text = $"Error loading task: {ex.Message}";
            }
        }

        [RequiresUnreferencedCode("Calls System.ComponentModel.DataAnnotations.ValidationContext.ValidationContext(Object)")]
        private async void UpdateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskTitle.Text))
            {
                errorsTextBlock.Text = "Title is required for a task";
                return;
            }

            if (StatusSelection.SelectedItem == null)
            {
                errorsTextBlock.Text = "Status is required";
                return;
            }

            var selectedStatus = ((ComboBoxItem)StatusSelection.SelectedItem).Tag?.ToString() ?? "todo";

            var task = new Data.Task
            {
                Id = this.id,
                Title = TaskTitle.Text,
                Description = TaskDescription.Text,
                AssignedTo = UserSelection.SelectedItem?.ToString() ?? "",
                Status = selectedStatus
            };

            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(task);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!Validator.TryValidateObject(task, validationContext, validationResults, true))
            {
                var errors = new List<string>();
                foreach (var validationResult in validationResults)
                {
                    errors.Add(validationResult.ErrorMessage);
                }
                errorsTextBlock.Text = string.Join(Environment.NewLine, errors);
            }
            else
            {
                try
                {
                    using var client = new HttpClient();
                    string apiUrl = "http://localhost:8080/update";
                    var jsonContent = new StringContent(JsonSerializer.Serialize(task), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        MainWindow.ContentFrame.Navigate(typeof(OverviewPage));
                    }
                    else
                    {
                        errorsTextBlock.Text = "Failed to update task.";
                    }
                }
                catch (HttpRequestException ex)
                {
                    errorsTextBlock.Text = $"Network error: {ex.Message}";
                }
                catch (TaskCanceledException)
                {
                    errorsTextBlock.Text = "Request timed out.";
                }
                catch (Exception ex)
                {
                    errorsTextBlock.Text = $"Unexpected error: {ex.Message}";
                }
            }
        }

        private async void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.id == 0)
            {
                errorsTextBlock.Text = "No task to delete.";
                return;
            }

            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Delete Task",
                Content = "Are you sure you want to delete this task? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    using var client = new HttpClient();
                    string apiUrl = "http://localhost:8080/delete";
                    
                    var task = new Data.Task { Id = this.id };
                    var jsonContent = new StringContent(JsonSerializer.Serialize(task), System.Text.Encoding.UTF8, "application/json");
                    
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(apiUrl),
                        Content = jsonContent
                    };
                    
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        MainWindow.ContentFrame.Navigate(typeof(OverviewPage));
                    }
                    else
                    {
                        errorsTextBlock.Text = "Failed to delete task.";
                    }
                }
                catch (HttpRequestException ex)
                {
                    errorsTextBlock.Text = $"Network error: {ex.Message}";
                }
                catch (TaskCanceledException)
                {
                    errorsTextBlock.Text = "Request timed out.";
                }
                catch (Exception ex)
                {
                    errorsTextBlock.Text = $"Unexpected error: {ex.Message}";
                }
            }
        }
    }
}