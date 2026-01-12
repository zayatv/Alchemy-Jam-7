using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public class HideLoadingScreenOperation : ILoadingOperation
    {
        public async UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            await loadingOperationData.LoadingScreen.HideAsync(cancellationToken);
        }
    }
}