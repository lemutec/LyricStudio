using System.Threading.Tasks;
using System.Threading;

namespace LyricStudio.Core.Translators;

public interface ITranslator
{
    public Task<string?> TranslateAsync(CancellationToken cancellationToken);
}
