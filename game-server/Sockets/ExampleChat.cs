using Microsoft.AspNetCore.SignalR;

public class ExampleChat : Hub
{
    public Task ServerMessageReceiver(string message)
    {
        var ctx = Context.GetHttpContext();
        Console.WriteLine($"Message received from {ctx?.Connection.RemoteIpAddress}: {message}");
        Clients.Others.SendAsync("SendMessageToClient", $"{ctx?.Connection.RemoteIpAddress}: {message}");
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client from {Context.GetHttpContext()?.Connection.RemoteIpAddress} disconnected from gamer server chat socket.");
        return base.OnDisconnectedAsync(exception);
    }
    public override Task OnConnectedAsync()
    {
        try
        {
            Console.WriteLine($"Client from {Context.GetHttpContext()?.Connection.RemoteIpAddress} connected to game server chat socket.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on connecting client: {ex.Message}");
        }
        return base.OnConnectedAsync();
    }
}