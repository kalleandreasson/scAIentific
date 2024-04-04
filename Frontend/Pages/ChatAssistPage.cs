using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Frontend.Pages
{
    public partial class ChatAssistPage : ComponentBase
    {
        [Inject]
        public ChatService? ChatService { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        private ElementReference chatHistoryElement;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (chatHistory.Any())
            {
                await Task.Delay(500);
                Console.WriteLine($"Is chatHistoryElement set? {chatHistoryElement.Id}");
                await JsRuntime.InvokeVoidAsync("scrollToBottom", chatHistoryElement);
            }
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

            await InvokeAsync(StateHasChanged); // Request the component to re-render
            await Task.Delay(100); // Allow time for the UI to update
            await JsRuntime.InvokeVoidAsync("scrollToBottom", chatHistoryElement);

        }
        private async Task HandleDeletionSuccess(bool success)
        {
            if (success)
            {
                NavigationManager.NavigateTo("/");
                Console.WriteLine($"success: {success}");
            }
        }
    }
}