using System;
using Core.Game.Input.Contexts;
using Core.Systems.Logging;
using Core.Systems.Input;
using Core.Systems.ServiceLocator;
using Core.Systems.Update;

namespace Core.Game.Input
{
    [Serializable]
    public class InputServiceConfig : ServiceConfig
    {
        private PlayerInputActions _inputActions;
        private InputService _inputService;
        
        public override void Install(IServiceInstallHelper helper)
        {
            _inputActions = new PlayerInputActions();

            _inputService = new InputService();
            
            _inputService.RegisterContext(new MovementInputContext(_inputActions.Gameplay));
            _inputService.RegisterContext(new InteractionInputContext(_inputActions.Gameplay));
            _inputService.RegisterContext(new BombInputContext(_inputActions.Gameplay));
            _inputService.RegisterContext(new UIInputContext(_inputActions.UI));
            
            _inputActions.Gameplay.Enable();
            _inputActions.UI.Enable();
            
            _inputService.EnableAll();
            
            helper.Register<IInputService>(_inputService);
            
            RegisterUpdatables(helper);
        }

        /// <summary>
        /// Registers all input contexts that implement IUpdatable, IFixedUpdatable, or ILateUpdatable
        /// with the UpdateService if it is available through the provided helper.
        /// </summary>
        /// <param name="helper">
        /// The service installation helper used to retrieve the IUpdateService and register the contexts.
        /// </param>
        private void RegisterUpdatables(IServiceInstallHelper helper)
        {
            if (!helper.TryGet(out IUpdateService updateService))
                return;

            foreach (var context in _inputService.GetAllContexts())
            {
                if (context is IUpdatable || context is IFixedUpdatable || context is ILateUpdatable)
                {
                    updateService.Register(context);
                    
                    GameLogger.Log(LogLevel.Debug, $"Registered {context.GetType().Name} with UpdateService");
                }
            }
        }
    }
}
