using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.Systems.Loading
{
    public class UnloadSceneOperation : ILoadingOperation
    {
        private readonly string _sceneName;
        private readonly string _newActiveSceneName;

        public UnloadSceneOperation(string sceneName, string newActiveSceneName = null)
        {
            _sceneName = sceneName;
            _newActiveSceneName = newActiveSceneName;
        }
        
        public async UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            var asyncOperation = SceneManager.UnloadSceneAsync(_sceneName);
            
            if (asyncOperation == null)
                return;

            while (asyncOperation.isDone)
            {
                await UniTask.Yield(cancellationToken);
            }
            
            if (_newActiveSceneName != null && SceneManager.GetActiveScene().name != _newActiveSceneName)
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
            
            await UniTask.Yield(cancellationToken);
        }
    }
}