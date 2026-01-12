using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public interface ITransitionEffect
    {
        UniTask PlayIntroAsync(CancellationToken cancellationToken = default);
        UniTask PlayOutroAsync(CancellationToken cancellationToken = default);
        float IntroDuration { get; }
        float OutroDuration { get; }
    }
}