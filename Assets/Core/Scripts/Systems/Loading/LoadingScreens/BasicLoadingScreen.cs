using System.Threading;
using Core.Systems.Loading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Systems.Loading.LoadingScreens
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class BasicLoadingScreen : MonoBehaviour, ILoadingScreen
    {
        #region Fields

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private bool _isActive;
        private LoadingOperationData _progress;

        #endregion
        
        #region Unity Lifecycle

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        #endregion
        
        #region ILoadingScreen Implementation

        public bool IsActive => _isActive;
        public bool CanHide => true;
        
        public async UniTask ShowAsync(LoadingOperationData loadingProgress, CancellationToken cancellationToken = default)
        {
            EnsureCanvasEnabled();
            
            _isActive = true;
            _progress = loadingProgress;
            
            await UniTask.CompletedTask;
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            await UniTask.CompletedTask;

            _isActive = false;
            
            DisableCanvas();
        }

        public void UpdateProgress(LoadingOperationData progress)
        {
        }
        
        #endregion
        
        private void EnsureCanvasEnabled()
        {
            if (_canvas != null)
            {
                _canvas.enabled = true;
            }

            gameObject.SetActive(true);
        }
        
        private void DisableCanvas()
        {
            if (_canvas != null)
            {
                _canvas.enabled = false;
            }
        }
    }
}