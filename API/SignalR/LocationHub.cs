using System;
using System.Threading.Tasks;
using Application.Location.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class LocationHub : Hub
    {
        public async Task SendLocation(LocationDto locationDto)
        {
            Console.WriteLine("SendLocation");
            await Clients.Caller.SendAsync("ReceiveLocation", "TestString");
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("OnConnected");
            return Task.CompletedTask;
        }
    }
}