namespace LyricStudio.Core.Configuration;

public sealed class ConfigurationImpl : PersistentSettings, IConfigurationImpl
{
    public override int Version => 1;

    public ConfigurationImpl()
        : base(JsonSettingsSerializer.Default)
    {
    }

    protected override void OnUpgrade(int oldVersion)
    {
    }
}
