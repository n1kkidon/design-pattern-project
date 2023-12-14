using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using game_client.Views;
using shared;

namespace game_client.Models;

public class GameObject
{
    private Vector2 _location;
    private float? initialWidth;
    private float? initialHeight;
    private readonly Dictionary<Control, Vector2> childDimensions = new();
    public Vector2 Location { 
        get { 
            _location.Y = (float)(Canvas.GetBottom(stackPanel) + Math.Round(GetInternalHeight()/2) - 1);
            _location.X = (float)(Canvas.GetLeft(stackPanel) + Math.Round(GetInternalWidth()/2) - 1);
            return _location;
        }
        protected set {  
            Canvas.SetBottom(stackPanel, value.Y);
            Canvas.SetLeft(stackPanel, value.X);
        }
    }
    private StackPanel stackPanel {get; set;}
    protected GameObject(Vector2 location) //location here is center
    {
        stackPanel = new();
        Location = location;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="control"></param>
    /// <param name="ignoreDimensions">whether this objects dimensions will contribute towards total height/width</param>
    protected void AddToStackPanel(Control control, bool ignoreDimensions = false)
    {
        if(control is TextBlock NameTag)
        {
            FormattedText text = new(
                NameTag.Text!, 
                CultureInfo.CurrentCulture, 
                FlowDirection.LeftToRight, 
                new Typeface(NameTag.FontFamily, NameTag.FontStyle, NameTag.FontWeight, NameTag.FontStretch),
                NameTag.FontSize,
                NameTag.Foreground
            );
            NameTag.Width = text.Width;
            NameTag.Height = text.Height;
        }
        stackPanel.Children.Add(control);
        childDimensions.Add(control, ignoreDimensions ? new(0, 0) : new((float)control.Width, (float)control.Height));
    }

    /// <summary>
    /// sums up all the heights of child objects
    /// </summary>
    /// <returns></returns>
    public float GetHeight()
    {
        double total = 0;
        foreach(var child in childDimensions.Values)
        {
            total += child.Y;
        }
        return (float)total;
    } 
    /// <summary>
    /// finds the child object with the maximum width in the stackPanel
    /// </summary>
    /// <returns></returns>
    public float GetWidth()
    {
        double max = 0;
        foreach(var child in childDimensions.Values)
        {
            if(child.X > max)
                max = child.X;
        }
        return (float)max;
    }

    /// <summary>
    /// sums up all the heights of child objects (that have height)
    /// </summary>
    /// <param name="initial"></param>
    /// <returns></returns>
    private float GetInternalHeight(bool initial = true)
    {
        if(initial && initialHeight != null)
            return (float)initialHeight;

        double total = 0;
        foreach(var child in stackPanel.Children)
        {
            if(double.IsNaN(child.Height))
                continue;
            total += child.Height;
            
        }
        initialHeight ??= (float)total;
        return (float)total;
    } 
    /// <summary>
    /// finds the child object with the maximum width in the stackPanel
    /// </summary>
    /// <param name="initial"></param>
    /// <returns></returns>
    private float GetInternalWidth(bool initial = true)
    {
        if(initial && initialWidth != null)
            return (float)initialWidth;
        double max = 0;
        foreach(var child in stackPanel.Children)
        {
            if(double.IsNaN(child.Width))
                continue;
            if(child.Width > max)
                max = child.Width;
        }
        initialWidth ??= (float)max;
        return (float)max;
    }
    private void AddCenterDebugDot(Vector2 locationOfCenter, Color color)
    {
        var rect = new Rectangle
        {
            Fill = new SolidColorBrush(color),
            Height = 2,
            Width = 2
        };
        Canvas.SetBottom(rect, locationOfCenter.Y);
        Canvas.SetLeft(rect, locationOfCenter.X);
        MainWindow.GetInstance().canvas.Children.Add(rect);
    }
    public virtual void AddObjectToCanvas()
    {
        MainWindow.GetInstance().canvas.Children.Add(stackPanel);
        AddCenterDebugDot(Location, Colors.Blue); //initial pos
    } 
    public void RemoveObjectFromCanvas() => MainWindow.GetInstance().canvas.Children.Remove(stackPanel);

    public void TeleportTo(Vector2 location)
    {
        Location = location;
    }
}
