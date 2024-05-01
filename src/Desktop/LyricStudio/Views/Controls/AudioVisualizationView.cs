using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using LyricStudio.Models.Audios;
using System;
using System.Collections.Generic;

namespace LyricStudio.Views.Controls;

public sealed class AudioVisualizationView : Control
{
    public List<AudioVolume> Volumes
    {
        get => (List<AudioVolume>)GetValue(VolumesProperty);
        set => SetValue(VolumesProperty, value);
    }

    public static readonly AvaloniaProperty<List<AudioVolume>> VolumesProperty =
        AvaloniaProperty.Register<AudioVisualizationView, List<AudioVolume>>(nameof(Volumes),
            defaultBindingMode: BindingMode.TwoWay,
            coerce: OnVolumesPropertyChanged);

    private static List<AudioVolume> OnVolumesPropertyChanged(AvaloniaObject sender, List<AudioVolume> value)
    {
        if (sender is AudioVisualizationView self)
        {
            self.InvalidateVisual();
        }
        return value;
    }

    public AudioVisualizationView()
    {
    }

    public sealed override void Render(DrawingContext context)
    {
        base.Render(context);
        context.RenderAsSpectrum(Bounds, Volumes);
    }
}

file static class AudioVisualizationRender
{
    public const string ThemeColor = "#AA009C00";

    public static void RenderAsSpectrum(this DrawingContext context, Rect rect, List<AudioVolume> volumes)
    {
        if (volumes == null || volumes.Count == 0)
            return;

        double width = rect.Width;
        double height = rect.Height;

        double xScale = width / volumes.Count;
        double yScale = height / 100d;

        Pen pen = new(new SolidColorBrush(Color.Parse(ThemeColor)), 1);

        for (int i = 0; i < volumes.Count; i++)
        {
            double x = i * xScale;
            double y = volumes[i].Volume * yScale;

            context.DrawLine(pen, new Point(x, height / 2f - y), new Point(x, height / 2f + y));
        }
    }

    [Obsolete(nameof(RenderAsSpectrum))]
    public static void RenderAsLineChart(this DrawingContext context, Rect rect, List<AudioVolume> volumes)
    {
        if (volumes == null || volumes.Count == 0)
            return;

        double width = rect.Width;
        double height = rect.Height;

        double xScale = width / volumes[^1].Time;
        double yScale = height / 100d;

        Pen pen = new(new SolidColorBrush(Color.Parse(ThemeColor)), 1);

        Point previousPoint = new(0, height);
        foreach (var volume in volumes)
        {
            double x = volume.Time * xScale;
            double y = height - volume.Volume * yScale;

            context.DrawLine(pen, previousPoint, new Point(x, y));

            previousPoint = new Point(x, y);
        }
    }

    [Obsolete(nameof(RenderAsSpectrum))]
    public static void RenderAsSolidLineChart(this DrawingContext context, Rect rect, List<AudioVolume> volumes)
    {
        if (volumes == null || volumes.Count == 0)
            return;

        double width = rect.Width;
        double height = rect.Height;

        double xScale = width / volumes.Count;
        double yScale = height / 100d;

        Pen pen = new(new SolidColorBrush(Color.Parse(ThemeColor)), 1);

        for (int i = 0; i < volumes.Count; i++)
        {
            double x = i * xScale;
            double y = height - volumes[i].Volume * yScale;

            context.DrawLine(pen, new Point(x, height), new Point(x, y));
        }
    }
}
