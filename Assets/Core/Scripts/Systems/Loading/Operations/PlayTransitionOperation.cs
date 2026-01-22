using System.Threading;
using Cysharp.Threading.Tasks;
using TransitionsPlus;
using UnityEngine;

namespace Core.Systems.Loading
{
    public class PlayTransitionOperation : ILoadingOperation
    {
        private readonly TransitionProfile _transitionProfile;
        private TransitionAnimator _transitionAnimator;
        
        public PlayTransitionOperation(TransitionProfile transitionProfile)
        {
            _transitionProfile = transitionProfile;
            
            _transitionAnimator = Object.FindFirstObjectByType<TransitionAnimator>();
        }
        
        public async UniTask Execute(LoadingOperationData loadingOperationData, CancellationToken cancellationToken = default)
        {
            if (_transitionAnimator == null)
            {
                _transitionAnimator = Object.FindFirstObjectByType<TransitionAnimator>();
            }
            
            _transitionAnimator.SetProgress(0f);
            _transitionAnimator.SetProfile( _transitionProfile);
            _transitionAnimator.autoPlay = false;
            _transitionAnimator.useUnscaledTime = true;
            _transitionAnimator.Play();

            while (_transitionAnimator.isPlaying)
            {
                await UniTask.Yield(cancellationToken);
            }
        }
    }
}