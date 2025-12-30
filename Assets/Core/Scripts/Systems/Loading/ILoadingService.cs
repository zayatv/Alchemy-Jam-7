using Core.Systems.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace Core.Scripts.Systems.Loading
{
    public interface ILoadingService : IService
    {
        UniTask LoadScene(string sceneName);
    }
}