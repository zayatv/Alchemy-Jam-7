using System.Collections.Generic;
using Core.Systems.Loading;
using Core.Systems.ServiceLocator;
using Eflatun.SceneReference;
using Sirenix.OdinInspector;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Game.GameStates.MainMenuState.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Title("Scene References")]
        [SerializeField] private SceneReference gameplayScene;
        
        [Title("Transitions")]
        [SerializeField] private TransitionProfile outroTransition;
        [SerializeField] private TransitionProfile introTransition;
        
        [Title("Loading Screen")]
        [SerializeField] private string loadingScreen;
        
        private ILoadingService _loadingService;

        private void Start()
        {
            _loadingService = ServiceLocator.Get<ILoadingService>();
        }

        public void StartGame()
        {
            Queue<ILoadingOperation> loadingOperations = new();
            
            loadingOperations.Enqueue(new PlayTransitionOperation(outroTransition));
            loadingOperations.Enqueue(new ShowLoadingScreenOperation(loadingScreen));
            loadingOperations.Enqueue(new UnloadSceneOperation(gameObject.scene.name));
            loadingOperations.Enqueue(new LoadSceneOperation(gameplayScene.Name, LoadSceneMode.Additive, true));
            loadingOperations.Enqueue(new HideLoadingScreenOperation());
            loadingOperations.Enqueue(new PlayTransitionOperation(introTransition));
            
            _loadingService.Load(loadingOperations);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}