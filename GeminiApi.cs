using System;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace GeminiServer;

class GeminiApi
{
    private readonly string _geminiApiKey;

    public GeminiApi(string geminiApiKey)
    {
        _geminiApiKey = geminiApiKey;
    }
    
    public (HttpListenerResponse, string, HttpStatusCode) ProcessGeminiRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (!request.HasEntityBody)
        {
            return (response, "No body provided", HttpStatusCode.BadRequest);
        }

        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string requestBody = reader.ReadToEnd();
            var requestData = JsonConvert.DeserializeObject<GeminiRequest>(requestBody);

            if (string.IsNullOrWhiteSpace(requestData?.Prompt))
            {
                return (response, "Promt should be", HttpStatusCode.BadRequest);
            }
            
            string geminiResponse = SimulateGeminiApiCall(requestData.Prompt); //ToDO: pizda

            var responseData = new
            {
                generated_text = geminiResponse,
                timestamp = DateTime.UtcNow
            };

            return (response, JsonConvert.SerializeObject(responseData), HttpStatusCode.OK);
        }
    }

    private string SimulateGeminiApiCall(string prompt)
    {
        Console.WriteLine($"Имитация запроса к Gemini с промтом: {prompt}");
        Thread.Sleep(500); // Имитация задержки сети

        return $"Это имитация ответа Gemini на ваш запрос: '{prompt}'. " +
               $"В реальной реализации здесь будет ответ от API Gemini.";
    }
}

class GeminiRequest
{
    public string Prompt { get; set; }
}