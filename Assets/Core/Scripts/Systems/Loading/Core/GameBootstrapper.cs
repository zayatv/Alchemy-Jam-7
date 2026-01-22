using System.Collections.Generic;
using Core.Systems.Logging;
using Eflatun.SceneReference;
using Sirenix.OdinInspector;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.SceneManagement;
using LogLevel = Core.Systems.Logging.LogLevel;

namespace Core.Systems.Loading
{
    public class GameBootstrapper : MonoBehaviour
    {
        #region Fields
        
        [Title("Scenes")]
        [SerializeField] private SceneReference coreScene;
        [SerializeField] private SceneReference gameScene;
        [SerializeField] private SceneReference mainMenuScene;
        
        [Title("Loading Screen")]
        [SerializeReference] private MonoBehaviour loadingScreen;
        
        [Title("Transition")]
        [SerializeField] private bool useIntroTransition = true;
        [SerializeField] private TransitionProfile transitionProfile;
        
        #endregion

        public void LoadGame()
        {
            ILoadingService loadingService = ServiceLocator.ServiceLocator.Get<ILoadingService>();

            if (loadingService == null)
            {
                GameLogger.Log(LogLevel.Error, "Cannot load game. Loading service is null.");

                return;
            }
            
            Queue<ILoadingOperation> operations = new();
            
            operations.Enqueue(new ShowLoadingScreenOperation(loadingScreen.GetComponent<ILoadingScreen>()));
            
            bool isCoreSceneLoaded = SceneManager.GetSceneByName(coreScene.Name).IsValid();
            
            if (!isCoreSceneLoaded)
                operations.Enqueue(new LoadSceneOperation(coreScene.Name, LoadSceneMode.Additive));

            operations.Enqueue(new LoadSceneOperation(gameScene.Name, LoadSceneMode.Additive));
            operations.Enqueue(new LoadSceneOperation(mainMenuScene.Name, LoadSceneMode.Additive, true));
            operations.Enqueue(new HideLoadingScreenOperation());
            
            if (useIntroTransition)
                operations.Enqueue(new PlayTransitionOperation(transitionProfile));

            loadingService.Load(operations);
        }
    }
}