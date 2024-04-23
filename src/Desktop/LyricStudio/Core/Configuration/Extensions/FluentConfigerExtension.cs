using System;
using System.Diagnostics;

namespace LyricStudio.Core.Configuration;

public static class FluentConfigerExtension
{
    public static IConfigurationImpl LoadSafe(this IConfigurationImpl self, string fileName)
    {
        try
        {
            self?.Load(fileName);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
        return self;
    }

    public static IConfigurationImpl SaveSafe(this IConfigurationImpl self, string fileName)
    {
        try
        {
            self?.Save(fileName);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
        return self;
    }
}
