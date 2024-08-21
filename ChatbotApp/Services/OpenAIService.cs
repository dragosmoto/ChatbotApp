using OpenAI_API;
using OpenAI_API.Chat;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotApp.Hubs;

namespace ChatbotApp.Services
{
    public class OpenAIService
    {
        private readonly OpenAIAPI _openAIAPI;
        private readonly IHubContext<ChatHub> _hubContext;
        private List<ChatMessage> _conversationHistory = new List<ChatMessage>();

        public OpenAIService(OpenAIAPI openAIAPI, IHubContext<ChatHub> hubContext)
        {
            _openAIAPI = openAIAPI;
            _hubContext = hubContext;
        }

        public async Task GetChatResponseAsync(string userInput, string connectionId)
        {
            // Add the user's input to the conversation history
            _conversationHistory.Add(new ChatMessage { Role = ChatMessageRole.User, TextContent = userInput });

            // Calculate the token count of the conversation history
            int tokenCount = CalculateTokenCount(_conversationHistory);

            // If the total token count exceeds 700, truncate the oldest messages
            while (tokenCount > 700)
            {
                _conversationHistory.RemoveAt(0);
                tokenCount = CalculateTokenCount(_conversationHistory);
            }

            // Create the chat request with the truncated conversation history
            var chatRequest = new ChatRequest
            {
                Model = "gpt-3.5-turbo-0125",
                Messages = _conversationHistory.ToArray()
            };

            var chatResponse = await _openAIAPI.Chat.CreateChatCompletionAsync(chatRequest);

            // Add the assistant's response to the conversation history
            _conversationHistory.Add(chatResponse.Choices[0].Message);

            foreach (var word in chatResponse.Choices[0].Message.TextContent.Split(' '))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "assistant", word + " ");
                await Task.Delay(50); // Simulate typing delay
            }
        }

        // Helper method to estimate the token count
        private int CalculateTokenCount(List<ChatMessage> messages)
        {
            int tokenCount = 0;

            foreach (var message in messages)
            {
                tokenCount += Tokenizer.EstimateTokenCount(message.TextContent);
            }

            return tokenCount;
        }
    }
}

// Tokenizer class to estimate token counts (very basic approximation)
public static class Tokenizer
{
    public static int EstimateTokenCount(string text)
    {
        // A simple estimate: 1 token per 4 characters (this is an approximation)
        return text.Length / 4;
    }
}
