using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using TickSyncAPI.HelperClasses;

namespace TickSyncAPI.Middlewares
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HelperClasses.WebSocketManager _manager;

        public WebSocketMiddleware(RequestDelegate next, HelperClasses.WebSocketManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
            {
                var showIdQuery = context.Request.Query["showId"].ToString();
                if (!int.TryParse(showIdQuery, out var showId))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                var socket = await context.WebSockets.AcceptWebSocketAsync();
                await _manager.HandleConnectionAsync(showId, socket);
            }
            else
            {
                await _next(context); // continue to next middleware
            }
        }
    }

}
