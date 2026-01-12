using System.Collections.Generic;
using Core.Systems.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace Core.Systems.Loading
{
    public interface ILoadingService : IService
    {
        LoadingOperationData LoadingOperationData { get; }
        UniTask Load(Queue<ILoadingOperation> operations);
    }
}