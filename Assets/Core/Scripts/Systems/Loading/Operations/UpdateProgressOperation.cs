using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public class UpdateProgressOperation : ILoadingOperation
    {
        private readonly float _progress;
        
        public UpdateProgressOperation(float progress)
        {
            _progress = progress;
        }
        
        public UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            loadingOperationData.Progress = _progress;
            
            return UniTask.CompletedTask;
        }
    }
}