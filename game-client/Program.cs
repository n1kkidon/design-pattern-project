using Microsoft.AspNetCore.SignalR.Client;

public class Program
{
    public static async Task Main()
    {
        var socket = new HubConnectionBuilder()
        .WithUrl("http://localhost:5025/exampleChat")
        .Build();

        socket.On("SendMessageToClient", (string message) => OnMessageReceived(message));
        await socket.StartAsync();
        System.Console.WriteLine("connected to server.");
        while(true)
        {
            var msg = Console.ReadLine();
            await socket.InvokeAsync("ServerMessageReceiver", msg);
        }
    }

    public static Task OnMessageReceived(string message)
    {
        System.Console.WriteLine(message);
        return Task.CompletedTask;
    }
}