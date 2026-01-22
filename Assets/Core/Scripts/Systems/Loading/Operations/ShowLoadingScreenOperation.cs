using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public class ShowLoadingScreenOperation : ILoadingOperation
    {
        private readonly ILoadingScreenService _loadingScreenService;
        private readonly ILoadingScreen _loadingScreen;
        
        public ShowLoadingScreenOperation(ILoadingScreen loadingScreen)
        {
            _loadingScreenService = ServiceLocator.ServiceLocator.Get<ILoadingScreenService>();
            _loadingScreen = _loadingScreenService.LoadingScreens.FirstOrDefault(screen => screen == loadingScreen);
        }
        
        public ShowLoadingScreenOperation(string loadingScreenName)
        { 
            _loadingScreenService = ServiceLocator.ServiceLocator.Get<ILoadingScreenService>();
            _loadingScreen = _loadingScreenService.GetLoadingScreen(loadingScreenName);
        }
        
        public async UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            loadingOperationData.LoadingScreen = _loadingScreen;
            
            await _loadingScreen.ShowAsync(loadingOperationData, cancellationToken);
        }
    }
}