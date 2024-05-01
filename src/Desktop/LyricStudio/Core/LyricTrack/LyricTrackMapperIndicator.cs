using AutoMapper;
using Fischless.Mapper;
using LyricStudio.ViewModels;

namespace LyricStudio.Core.LyricTrack;

internal class LyricTrackMapperIndicator : MapperIndicator
{
    public override void CreateMap(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<LrcLine, ObservableLrcLine>();
    }
}
