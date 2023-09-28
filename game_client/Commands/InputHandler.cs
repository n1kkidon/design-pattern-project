using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;

namespace game_client;

public class InputHandler
{
    private readonly Dictionary<Key, ICommand> keyMapping = new();


    public void SetCommand(Key key, ICommand command)
    {
        keyMapping[key] = command;
    }

    public async Task HandleInput(Key key)
    {
        if (keyMapping.TryGetValue(key, out ICommand? value))
        {
            await value.Execute();
        }
    }

    public bool RemoveCommand(Key key) => keyMapping.Remove(key);
}
