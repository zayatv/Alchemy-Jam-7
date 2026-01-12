using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public interface ILoadingScreen
    {
        UniTask ShowAsync(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default);
        UniTask HideAsync(CancellationToken cancellationToken = default);
        bool IsActive { get; }
        bool CanHide { get; }
    }
}