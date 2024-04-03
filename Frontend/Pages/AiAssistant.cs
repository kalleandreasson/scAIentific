using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;

namespace Frontend.Pages
{
    public partial class  AiAssistant : ComponentBase
    {
        [Inject]
        public ChatService? ChatService {get; set;}
        [Inject]
        public NavigationManager? NavigationManager {get; set;}

        private string userQuery = "";
        private List<ChatMessage> chatHistory = new List<ChatMessage>();
        private bool isChatHistoryLoaded = false;
        private object chat = new { };

        protected override async Task OnInitializedAsync()
        {
            var chatResponse = await ChatService.GetChatHistoryAsync();
            if (chatResponse?.Messages != null)
            {
                chatHistory = chatResponse.Messages.OrderBy(m => m.CreatedAt).ToList();
            }
            isChatHistoryLoaded = true; // Indicate that the chat history has been loaded
        }

        private async Task HandleSubmitAsync()
        {
            var chatRequest = new ChatRequest { UserMessage = userQuery };
            var chatResponse = await ChatService.PostUserQueryAsync(chatRequest);

            if (chatResponse?.Messages != null)
            {
                chatHistory.AddRange(chatResponse.Messages);
                chatHistory = chatHistory.GroupBy(m => m.Id).Select(g => g.First()).OrderBy(m => m.CreatedAt).ToList();
            }
            userQuery = "";
        }
        private async Task navigateToReplaceFilePage()
        {
            NavigationManager.NavigateTo("/replacefile");
        }
    }
}