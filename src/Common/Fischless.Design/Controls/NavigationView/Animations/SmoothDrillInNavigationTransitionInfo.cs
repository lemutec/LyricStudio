using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using FluentAvalonia.UI.Media.Animation;

namespace Fischless.Design.Controls;

public class SmoothDrillInNavigationTransitionInfo : DrillInNavigationTransitionInfo
{
    public async override void RunAnimation(Animatable ctrl, CancellationToken cancellationToken)
    {
        var entryAnimation = new Animation
        {
            Easing = new SplineEasing(0.1d, 0.9d, 0.2d, 1d),
            Children =
            {
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, IsReversed ? 1.15d : 0.9d),
                        new Setter(ScaleTransform.ScaleYProperty, IsReversed ? 1.15d : 0.9d),
                        new Setter(Visual.OpacityProperty, 0d)
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 1d),
                        new Setter(ScaleTransform.ScaleYProperty, 1d),
                        new Setter(Visual.OpacityProperty, 1d)
                    },
                    Cue = new Cue(1d)
                }
            },
            Duration = TimeSpan.FromMilliseconds(500d),
            FillMode = FillMode.Forward
        };

#if false
        var exitAnimation = new Animation
        {
            Children =
            {
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0d),
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0d),
                        new Setter(ScaleTransform.ScaleXProperty, IsReversed ? 1.15d : 0.9d),
                        new Setter(ScaleTransform.ScaleYProperty, IsReversed ? 1.15d : 0.9d),
                    },
                    Cue = new Cue(1d)
                }
            },
            Duration = TimeSpan.FromMilliseconds(160d),
            FillMode = FillMode.Forward
        };

        bool isVisible = (ctrl as Visual).IsVisible;

        (ctrl as Visual).IsVisible = false;
        (ctrl as Visual).Opacity = 0d;
        await exitAnimation.RunAsync(ctrl, cancellationToken);
        (ctrl as Visual).IsVisible = isVisible;
#endif
        await entryAnimation.RunAsync(ctrl, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        (ctrl as Visual).Opacity = 1;
    }
}
