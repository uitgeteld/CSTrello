using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CSTrelloApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatePage : Page
    {
        public CreatePage()
        {
            InitializeComponent();

            HttpClient client = new HttpClient();
            string apiUrl = "http://localhost:8080/users/";
            var response = client.GetAsync(apiUrl).Result;

            if(response.StatusCode == HttpStatusCode.OK)
            {
                List<Data.User> users = JsonSerializer.Deserialize<List<Data.User>>(response.Content.ReadAsStringAsync().Result);

                foreach (var user in users)
                {
                    UserSelection.Items.Add(user.Name);
                }
            }

            
        }

        [RequiresUnreferencedCode("Calls System.ComponentModel.DataAnnotations.ValidationContext.ValidationContext(Object)")]
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var task  = new Data.Task
            {
                Title = TaskTitle.Text,
                Description = TaskDescription.Text,
                AssignedTo = UserSelection.SelectedItem?.ToString() ?? "",
                Status = "To Do"
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
                errorsTextBlock.Text = "";
            }
        }

    }
}
