using System.Threading;
using Cysharp.Threading.Tasks;
using TransitionsPlus;

namespace Core.Systems.Loading
{
    public class PlayTransitionOperation : ILoadingOperation
    {
        private readonly TransitionType _transitionType;
        
        public PlayTransitionOperation(TransitionType transitionType)
        {
            _transitionType = transitionType;
        }
        
        public async UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            var animator = TransitionAnimator.Start(_transitionType);

            while (animator.isPlaying)
            {
                await UniTask.Yield(cancellationToken);
            }
        }
    }
}