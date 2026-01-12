using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public class UpdateDescriptionOperation : ILoadingOperation
    {
        private readonly string _description;
        
        public UpdateDescriptionOperation(string description)
        {
            _description = description;
        }
        
        public UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            loadingOperationData.Description = _description;
            
            return UniTask.CompletedTask;
        }
    }
}