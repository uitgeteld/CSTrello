using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using CSTrelloApi.Data;
using DevOne.Security.Cryptography.BCrypt;
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

                        responseString += JsonSerializer.Serialize(tasks);

                    }
                }else if(request.Url.AbsolutePath == "/update" && request.HttpMethod == "POST"){
                    
                    using (var db = new AppDbContext())
                    {
                        var form = request.InputStream;
                        var reader = new System.IO.StreamReader(form);
                        var body = reader.ReadToEnd();
                        var task = JsonSerializer.Deserialize<Task>(body);
                        if (task != null)
                        {
                            Task taskFromDb = db.Tasks.FirstOrDefault(t => t.Id == task.Id);
                            if (taskFromDb != null)
                            {
                                taskFromDb.Title = task.Title;
                                taskFromDb.Description = task.Description;
                                taskFromDb.Status = task.Status;
                                taskFromDb.AssignedTo = task.AssignedTo;
                                db.Tasks.Update(taskFromDb);
                                db.SaveChanges();
                                response.StatusCode = 201;
                                continue;
                            }else
                            {
                                response.StatusCode = 404;
                                continue;
                            }

                        }
                        else
                        {
                            response.StatusCode = 404;
                            continue;
                        }

                    }
                    
                }else if (request.Url.AbsolutePath == "/create" && request.HttpMethod == "POST")
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
                            continue;
                        }
                        db.Tasks.Add(task);
                        db.SaveChanges();
                        response.StatusCode = 201;
                        //continue;
                    }
                    

                }else if (request.Url.AbsolutePath == "/delete" && request.HttpMethod == "DELETE")
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
                            continue;
                        }
                        var taskToRemove = db.Tasks.Find(task.Id);
                        if (taskToRemove != null)
                        {
                            db.Tasks.Remove(taskToRemove);
                            db.SaveChanges();
                            response.StatusCode = 200;
                            continue;
                        }
                        else
                        {
                            response.StatusCode = 404;
                            continue;
                        }
                    }
                }else if ( request.Url.AbsolutePath == "/users")
                {
                    using(var db = new AppDbContext())
                    {
                        var users = db.Users.ToList();
                        responseString += JsonSerializer.Serialize(users);
                    }
                    
                } else if (request.Url.AbsolutePath == "/register" && request.HttpMethod == "POST")
                {
                    using (var db = new AppDbContext())
                    {
                        var form = request.InputStream;
                        var reader = new System.IO.StreamReader(form);
                        var body = reader.ReadToEnd();
                        var user = JsonSerializer.Deserialize<User>(body);
                        if (user == null)
                        {
                            response.StatusCode = 404;
                            continue;
                        }
                        user.Password = BCryptHelper.HashPassword(user.Password, BCryptHelper.GenerateSalt());
                        db.Users.Add(user);
                        db.SaveChanges();
                        response.StatusCode = 201;
                    }
                }else if (request.Url.AbsolutePath == "/login" && request.HttpMethod == "POST")
                {
                    using (var db = new AppDbContext())
                    {
                        var form = request.InputStream;
                        var reader = new System.IO.StreamReader(form);
                        var body = reader.ReadToEnd();
                        var user = JsonSerializer.Deserialize<User>(body);
                        if (user == null)
                        {
                            response.StatusCode = 404;
                            continue;
                        }
                        bool isPasswordValid = false;
                        var userFromDb = db.Users.FirstOrDefault(u => u.Id == user.Id);
                        if (userFromDb != null)
                        {
                            isPasswordValid = BCryptHelper.CheckPassword(user.Password, userFromDb.Password);
                            response.StatusCode = isPasswordValid ? 201 : 404;
                            continue;
                        }
                    }
                } else
                {
                    responseString = "<html><body>404 Not Found</body></html>";
                    response.StatusCode = 404;
                    continue;
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
