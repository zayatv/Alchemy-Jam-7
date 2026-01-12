using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.Systems.Loading
{
    public class LoadSceneOperation : ILoadingOperation
    {
        private readonly string _sceneName;
        private readonly LoadSceneMode _loadSceneMode;
        
        public LoadSceneOperation(string sceneName, LoadSceneMode loadSceneMode)
        {
            _sceneName = sceneName;
            _loadSceneMode = loadSceneMode;
        }
        
        public async UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(_sceneName, _loadSceneMode);

            if (asyncOperation == null)
                return;

            while (asyncOperation.progress < 0.9f)
            {
                await UniTask.Yield(cancellationToken);
            }
            
            asyncOperation.allowSceneActivation = true;
            
            await UniTask.Yield(cancellationToken);
        }
    }
}