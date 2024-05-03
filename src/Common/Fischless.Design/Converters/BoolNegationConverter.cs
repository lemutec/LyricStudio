﻿namespace Fischless.Design.Converters;

public class BoolNegationConverter : BoolToValueConverter<bool>
{
    public BoolNegationConverter()
    {
        this.TrueValue = true;
        this.FalseValue = false;
        this.IsInverted = true;
    }
}
