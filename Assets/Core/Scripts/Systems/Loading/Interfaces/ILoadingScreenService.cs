using System.Collections.Generic;
using Core.Systems.ServiceLocator;

namespace Core.Systems.Loading
{
    public interface ILoadingScreenService : IService
    {
        IReadOnlyList<ILoadingScreen> LoadingScreens { get; }
        void RegisterLoadingScreen(ILoadingScreen loadingScreen);
        void UnregisterLoadingScreen(ILoadingScreen loadingScreen);
        ILoadingScreen GetLoadingScreen(string name);
    }
}