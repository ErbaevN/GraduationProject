using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReStart2
{
    public class Chat
    {
        public Chat(IHubContext<ChatHub> hubContext)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    hubContext.Clients.All.InvokeAsync("chatik",DateTime.Now.Ticks);
                    Thread.Sleep(500);
                }
            });
        }
    }
}
