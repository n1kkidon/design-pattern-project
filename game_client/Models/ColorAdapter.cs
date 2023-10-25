using System;
using AvaloniaColor = Avalonia.Media.Color;
using DrawingColor = System.Drawing.Color;

namespace game_client.Adapters
{
    public static class ColorAdapter
    {
        public static AvaloniaColor ToAvaloniaColor(DrawingColor color)
        {
            return AvaloniaColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static DrawingColor ToDrawingColor(AvaloniaColor color)
        {
            return DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        // Additional utility methods
        public static AvaloniaColor LightenColor(AvaloniaColor color, double factor)
        {
            return AvaloniaColor.FromArgb(
                color.A,
                (byte)Math.Min(255, color.R + 255 * factor),
                (byte)Math.Min(255, color.G + 255 * factor),
                (byte)Math.Min(255, color.B + 255 * factor));
        }

        public static AvaloniaColor DarkenColor(AvaloniaColor color, double factor)
        {
            return AvaloniaColor.FromArgb(
                color.A,
                (byte)Math.Max(0, color.R - 255 * factor),
                (byte)Math.Max(0, color.G - 255 * factor),
                (byte)Math.Max(0, color.B - 255 * factor));
        }

        public static string ToHexString(AvaloniaColor color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
