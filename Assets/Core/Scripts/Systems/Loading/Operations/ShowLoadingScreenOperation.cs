using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public class ShowLoadingScreenOperation : ILoadingOperation
    {
        private readonly ILoadingScreen _loadingScreen;
        
        public ShowLoadingScreenOperation(ILoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }
        
        public async UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            loadingOperationData.LoadingScreen = _loadingScreen;
            
            await _loadingScreen.ShowAsync(loadingOperationData, cancellationToken);
        }
    }
}