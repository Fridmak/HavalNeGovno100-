using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Server;
class SimpleServer
{
    private readonly HttpListener _listener;
    private readonly string _url;

    public SimpleServer(string url)
    {
        _url = url;
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine($"Сервер запущен и слушает {_url}");

        Thread listenerThread = new Thread(Listen);
        listenerThread.Start();
    }

    public void Stop()
    {
        _listener.Stop();
        Console.WriteLine("Сервер остановлен");
    }

    private void Listen()
    {
        while (_listener.IsListening)
        {
            try
            {
                HttpListenerContext context = _listener.GetContext();
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    try
                    {
                        ProcessRequest(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке запроса: {ex.Message}");
                    }
                });
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode == 995) 
                    Console.WriteLine("Сервер завершает работу...");
                else
                    Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        try
        {
            switch (request.HttpMethod.ToUpper())
            {
                case "GET":
                case "POST":
                    SendResponse(response, "Task received", HttpStatusCode.OK);
                    break;

                default:
                    SendResponse(response, "Only GET and POST are supported", HttpStatusCode.MethodNotAllowed);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing request: {ex.Message}");
            SendResponse(response, "Internal Server Error", HttpStatusCode.InternalServerError);
        }
        finally
        {
            response.OutputStream.Close();
        }
    }

    private void SendResponse(HttpListenerResponse response, string message, HttpStatusCode statusCode)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);

        response.StatusCode = (int)statusCode;
        response.ContentType = "text/plain";
        response.ContentLength64 = buffer.Length;

        response.OutputStream.Write(buffer, 0, buffer.Length);
    }
}

class Program
{
    static void Main(string[] args)
    {
        string url = "http://localhost:8080/"; 

        var server = new SimpleServer(url);
        server.Start();

        Console.WriteLine("Нажмите любую клавишу для остановки сервера...");
        Console.ReadKey();

        server.Stop();
    }
}