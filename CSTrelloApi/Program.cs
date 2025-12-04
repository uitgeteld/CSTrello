using System.Net;
using System.Text.Json;
using CSTrelloApi.Data;
using Task = CSTrelloApi.Data.Task;

namespace CSTrelloApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpListener lister = new HttpListener();
        using (var db = new AppDbContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
        
        lister.Prefixes.Add("http://localhost:8080/");
        lister.Start();
        Console.WriteLine("Listening on http://localhost:8080/");
        while (true)
        {
            HttpListenerContext context = lister.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string responseString = "";

            if (request.Url.AbsolutePath == "/all")
            {
                using (var db = new AppDbContext())
                {
                    var tasks = db.Tasks.ToList();
                    responseString = "<html><body><h1>All Cars</h1><ul>"; // Waarom cars??
                    foreach (var task in tasks)
                    {
                        responseString += $"<li>{task.Id} {task.Title} ({task.AssignedTo}) - {task.Description} - {task.Status}</li>";
                    }
                    responseString += "</ul></body></html>";
                }
            }else if (request.Url.AbsolutePath == "/add" && request.HttpMethod == "POST")
            {
                using (var db = new AppDbContext())
                {
                    var form = request.InputStream;
                    var reader = new System.IO.StreamReader(form);
                    var body = reader.ReadToEnd();
                    var task = JsonSerializer.Deserialize<Task>(body);
                    db.Tasks.Add(task);
                    db.SaveChanges();
                    response.StatusCode = 201;
                }
                

            }else if (request.Url.AbsolutePath == "/remove")
            {
                using (var db = new AppDbContext())
                {
                    var form = request.InputStream;
                    var reader = new System.IO.StreamReader(form);
                    var body = reader.ReadToEnd();
                    var task = JsonSerializer.Deserialize<Task>(body);
                    if (task == null)
                    {
                        response.StatusCode = 404;
                        return;
                    }
                    var taskToRemove = db.Tasks.Find(task.Id);
                    if (taskToRemove != null)
                    {
                        db.Tasks.Remove(taskToRemove);
                        db.SaveChanges();
                        response.StatusCode = 200;  
                    }
                    else
                    {
                        response.StatusCode = 404;
                    }
                }
            }else
            {
                responseString = "<html><body>404 Not Found</body></html>";
                response.StatusCode = 404;
            }
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);   
            output.Close();
        }
        }
        
    }
}
