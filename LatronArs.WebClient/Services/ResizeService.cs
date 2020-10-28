using System;
using System.Threading.Tasks;
using LatronArs.WebClient.Services.Interfaces;
using Microsoft.JSInterop;

namespace LatronArs.WebClient.Services
{
    public class ResizeService
    {
        public static event Func<Task> OnResize;

        [JSInvokable]
        public static async Task PushResize()
        {
            await OnResize?.Invoke();
        }

        public ResizeService(IJSRuntime jsRuntime)
        {
            jsRuntime.InvokeAsync<object>("browserResize.registerResizeCallback").ConfigureAwait(false);
        }
    }
}