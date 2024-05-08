using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using FluentAvalonia.UI.Media.Animation;

namespace Fischless.Design.Controls;

public class FasterEntranceNavigationTransitionInfo : EntranceNavigationTransitionInfo
{
    public override async void RunAnimation(Animatable ctrl, CancellationToken cancellationToken)
    {
        await new Animation
        {
            Easing = new SplineEasing(0.1d, 0.9d, 0.2d),
            Children =
            {
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0d),
                        new Setter(TranslateTransform.XProperty, FromHorizontalOffset),
                        new Setter(TranslateTransform.YProperty, FromVerticalOffset)
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d),
                        new Setter(TranslateTransform.XProperty, 0d),
                        new Setter(TranslateTransform.YProperty, 0d)
                    },
                    Cue = new Cue(1d)
                }
            },
            Duration = TimeSpan.FromSeconds(0.25d),
            FillMode = FillMode.Forward
        }.RunAsync(ctrl, cancellationToken);
        (ctrl as Visual).Opacity = 1.0;
    }
}
