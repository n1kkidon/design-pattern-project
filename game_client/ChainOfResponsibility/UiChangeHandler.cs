using System;
using game_client.Views;

namespace game_client.ChainOfResponsibility;

public class UiChangeHandler: BaseHandler
{
    private readonly MainWindow _mainWindow;
    public UiChangeHandler(MainWindow mainWindow) => _mainWindow = mainWindow;
    public override void Handle(PlayerJoinRequest request)
    {
        Console.WriteLine("UiChangeHandler: removing login ui."); 
                
        _mainWindow.weaponSelectionPanel.IsVisible = false;
        _mainWindow.canvas.Children.Remove(_mainWindow.joinButton);
        _mainWindow.canvas.Children.Remove(_mainWindow.nameField);
        _mainWindow.GameStarted = true;
        NextHandler?.Handle(request);
    }
}