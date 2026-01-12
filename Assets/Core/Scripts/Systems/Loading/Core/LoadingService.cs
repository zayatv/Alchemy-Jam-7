using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public class LoadingService : ILoadingService
    {
        private LoadingOperationData _loadingOperationData = new();
        private Queue<ILoadingOperation> _operations = new();
        
        public LoadingOperationData LoadingOperationData => _loadingOperationData;
        
        public async UniTask Load(Queue<ILoadingOperation> operations)
        {
            foreach (ILoadingOperation operation in operations)
            {
                _operations.Enqueue(operation);
            }
            
            _loadingOperationData.Description = "Loading";

            await ProcessQueue();
        }

        private async UniTask ProcessQueue(CancellationToken cancellationToken = default)
        {
            _loadingOperationData.Phase = LoadingPhase.Loading;

            while (_operations.Count > 0)
            {
                ILoadingOperation currentOperation = _operations.Dequeue();

                await currentOperation.Execute(_loadingOperationData);
            }
            
            _loadingOperationData.Phase = LoadingPhase.Idle;
            _loadingOperationData.Progress = 1;
        }
    }
}