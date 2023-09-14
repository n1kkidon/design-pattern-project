namespace game_server.Sockets;

public static class SocketEndpointsRegister
{
    public static void RegisterSocketEndpoints(this WebApplication app)
    {
        app.UseRouting();
        app.MapHub<ExampleChat>("exampleChat");
    }
}