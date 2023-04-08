using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace exam_6_aruuke_maratova
{
    internal class Server
    {
        private Thread _serverThread;
        private string _siteDirectory;
        private HttpListener _listener;
        private int _port;
        public Server(string path, int port)
        {
            Initialize(path, port);
        }

        private void Initialize(string path, int port)
        {
            _siteDirectory = path;
            _port = port;
            _serverThread = new Thread(Listen);
            _serverThread.Start();
            Console.WriteLine("Сервер запущен на порту " + _port);
            Console.WriteLine("файлы лежат в папке " + _siteDirectory);
        }

        private void Listen(object? obj)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{_port}/");
            _listener.Start();
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            string fileName = context.Request.Url.AbsolutePath;
            //var query = context.Request.QueryString;
            //query.Get("q"); 
            //var method = context.Request.HttpMethod;
            //if(method == "POST")
            //{

            //}
            Console.WriteLine(fileName);
            string content = "";
            if (fileName.Contains(".html"))
                content = BuildHtml(fileName, context);
            else
            {
                content = File.ReadAllText(_siteDirectory + fileName);
            }
            fileName = _siteDirectory + fileName;
            if (File.Exists(fileName))
            {
                try
                {
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    context.Response.ContentType = GetContentType(fileName);
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    Stream fileStream = new MemoryStream(buffer);
                    int dataLength;
                    do
                    {
                        dataLength = fileStream.Read(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Write(buffer, 0, dataLength);
                    }
                    while (dataLength > 0);
                    fileStream.Close();
                    context.Response.OutputStream.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
        }

        private string GetContentType(string fileName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                {".css", "text/css"},
                {".html", "text/html"},
                {".ico", "image/x-icon" },
                {".js", "application/x-javascript" },
                {".json", "application/json" },
                {".png", "image/png" }
            };
            string contentType = "";
            string fileExt = Path.GetExtension(fileName);
            dictionary.TryGetValue(fileExt, out contentType);
            return contentType;
        }

        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }
        private string BuildHtml(string filename, HttpListenerContext context)
        {
            string html = "";
            string layoutPath = _siteDirectory + "/layout.html";
            string filePath = _siteDirectory + filename;
            var razorService = Engine.Razor;
            if (!razorService.IsTemplateCached("layout", null))
                razorService.AddTemplate("layout", File.ReadAllText(layoutPath));
            if (!razorService.IsTemplateCached(filename, null))
            {
                razorService.AddTemplate(filename, File.ReadAllText(filePath));
                razorService.Compile(filename);
            }
            string json = File.ReadAllText("../../../tasks.json");
            Console.WriteLine(DateTime.Now.ToString());

            List<Task> tasks = JsonSerializer.Deserialize<List<Task>>(json);
            html = razorService.Run(filename, null, new
            {
                Tasks = tasks
            });
            return html;
        }
    }
}
