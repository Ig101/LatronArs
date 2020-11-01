using System;
using System.Threading.Tasks;
using LatronArs.WebClient.Models;
using LatronArs.WebClient.Services.Interfaces;
using Microsoft.JSInterop;

namespace LatronArs.WebClient.Services
{
    public class EventsService
    {
        public static event Func<Task> OnResize;

        public static event Action<KeyboardEvent> OnKeyDown;

        public static event Action<KeyboardEvent> OnKeyUp;

        [JSInvokable]
        public static async Task PushResize()
        {
            if (OnResize != null)
            {
                await OnResize.Invoke();
            }
        }

        [JSInvokable]
        public static void PushKeyDown(KeyboardEvent e)
        {
            OnKeyDown?.Invoke(e);
        }

        [JSInvokable]
        public static void PushKeyUp(KeyboardEvent e)
        {
            OnKeyUp?.Invoke(e);
        }

        private static async Task RegisterEvents(IJSRuntime jsRuntime)
        {
            await jsRuntime.InvokeAsync<object>("eventsExtensions.registerEventsCallback");
        }

        public EventsService(IJSRuntime jsRuntime)
        {
            RegisterEvents(jsRuntime).ConfigureAwait(false);
        }
    }
}