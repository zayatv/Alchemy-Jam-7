using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.Systems.Loading
{
    public class LoadSceneOperation : ILoadingOperation
    {
        private readonly string _sceneName;
        private readonly LoadSceneMode _loadSceneMode;
        private readonly bool _setAsActive;
        
        public LoadSceneOperation(string sceneName, LoadSceneMode loadSceneMode, bool setAsActive = false)
        {
            _sceneName = sceneName;
            _loadSceneMode = loadSceneMode;
        }
        
        public async UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(_sceneName, _loadSceneMode);

            if (asyncOperation == null)
                return;
            
            asyncOperation.allowSceneActivation = false;

            while (asyncOperation.progress < 0.9f)
            {
                await UniTask.Yield(cancellationToken);
            }
            
            asyncOperation.allowSceneActivation = true;

            while (!asyncOperation.isDone)
            {
                await UniTask.Yield(cancellationToken);
            }

            if (_setAsActive && SceneManager.GetActiveScene().name != _sceneName)
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
            
            await UniTask.Yield(cancellationToken);
        }
    }
}