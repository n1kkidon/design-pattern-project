using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace shared;

public class PlayerInfo
{
    public required double Y {get; set;}
    public required double X {get; set;}
    public required string Name {get; set;}
    public required string Uuid {get; set;}
    public required RGB Color {get; set;}
}

public record RGB(byte R, byte G, byte B);