using AutoGen.Core;
using AutoGen.Gemini;

namespace HavalNeGovno;

public class GeminiApi
{
    private static string apiKey;
    private MiddlewareStreamingAgent<GeminiChatAgent> agent;
    
    GeminiApi()
    {
        apiKey = Environment.GetEnvironmentVariable("GOOGLE_GEMINI_API_KEY");

        agent = new GeminiChatAgent(
                name: "gemini",
                model: "gemini-1.5-flash-001",
                apiKey: apiKey,
                systemMessage: "You are a helpful C# engineer, put your code between ```csharp and ```, don't explain the code")
            .RegisterMessageConnector()
            .RegisterPrintMessage();

    }
}