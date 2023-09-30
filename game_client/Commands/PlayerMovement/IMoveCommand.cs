using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using game_client.Models;

namespace game_client.Commands.PlayerMovement;
public interface IMoveCommand
{
    public Game game {get; set;}
}