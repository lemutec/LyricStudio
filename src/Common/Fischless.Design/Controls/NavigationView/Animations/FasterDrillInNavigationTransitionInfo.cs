using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using FluentAvalonia.UI.Media.Animation;

namespace Fischless.Design.Controls;

public class FasterDrillInNavigationTransitionInfo : DrillInNavigationTransitionInfo
{
    public async override void RunAnimation(Animatable ctrl, CancellationToken cancellationToken)
    {
        var scaleAnimation = new Animation
        {
            Easing = new SplineEasing(0.1, 0.9, 0.2, 1.0),
            Children =
            {
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, IsReversed ? 1.05d :0.95d),
                        new Setter(ScaleTransform.ScaleYProperty, IsReversed ? 1.05d :0.95d)
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 1d),
                        new Setter(ScaleTransform.ScaleYProperty, 1d)
                    },
                    Cue = new Cue(1d)
                }
            },
            Duration = TimeSpan.FromSeconds(0.67d),
            FillMode = FillMode.Forward
        };

#if false
        var opacityAnimation = new Animation
        {
            Easing = new LinearEasing(),
            Children =
            {
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0d)
                    },
                    Cue = new Cue(0.0)
                },
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d)
                    },
                    Cue = new Cue(1d)
                }
            },
            Duration = TimeSpan.FromSeconds(0.4d),
            FillMode = FillMode.Forward
        };

        await Task.WhenAll(
            scaleAnimation.RunAsync(ctrl, cancellationToken),
            opacityAnimation.RunAsync(ctrl, cancellationToken)
        );
        (ctrl as Visual).Opacity = 1;
#else
        (ctrl as Visual).Opacity = 1;
        await scaleAnimation.RunAsync(ctrl, cancellationToken);
#endif
    }
}
