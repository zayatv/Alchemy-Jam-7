using System.Collections.Generic;

namespace Core.Systems.Loading
{
    public class LoadingScreenService : ILoadingScreenService
    {
        private readonly List<ILoadingScreen> _loadingScreens = new();
        
        public IReadOnlyList<ILoadingScreen> LoadingScreens => _loadingScreens;
        
        public void RegisterLoadingScreen(ILoadingScreen loadingScreen)
        {
            _loadingScreens.Add(loadingScreen);
        }

        public void UnregisterLoadingScreen(ILoadingScreen loadingScreen)
        {
            _loadingScreens.Remove(loadingScreen);
        }

        public ILoadingScreen GetLoadingScreen(string name)
        {
            foreach (var screen in _loadingScreens)
            {
                if (screen.Name == name) 
                    return screen;
            }

            return null;
        }
    }
}