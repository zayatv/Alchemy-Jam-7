using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public interface ILoadingOperation
    {
        UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default);
    }
}