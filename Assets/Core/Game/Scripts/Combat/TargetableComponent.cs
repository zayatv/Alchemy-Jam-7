using UnityEngine;
using Core.Systems.ServiceLocator;
using Core.Systems.Combat;
using Core.Systems.Logging;

namespace Core.Game.Combat
{
    [DisallowMultipleComponent]
    public class TargetableComponent : MonoBehaviour
    {
        [Header("Target Configuration")]
        [Tooltip("The MonoBehaviour component that implements ITargetable. If left empty, will search on this GameObject.")]
        [SerializeField] private MonoBehaviour targetComponent;

        private ITargetable _targetable;
        private ITargetRegistry _targetRegistry;

        private void OnEnable()
        {
            if (targetComponent != null)
            {
                _targetable = targetComponent as ITargetable;
                
                if (_targetable == null)
                {
                    GameLogger.Log(LogLevel.Error, $"Target component on {gameObject.name} does not implement ITargetable!");
                    
                    return;
                }
            }
            else
            {
                _targetable = GetComponent<ITargetable>();
                
                if (_targetable == null)
                {
                    GameLogger.Log(LogLevel.Error, $"No ITargetable component found on {gameObject.name}! Add a HealthComponent or assign targetComponent.");
                    return;
                }
            }

            if (!ServiceLocator.TryGet(out _targetRegistry))
            {
                GameLogger.Log(LogLevel.Error, $"TargetRegistry service not found! Ensure it's registered before scene objects awake.");
                
                return;
            }

            _targetRegistry.Register(_targetable);
        }

        private void OnDisable()
        {
            if (_targetRegistry != null && _targetable != null)
            {
                _targetRegistry.Unregister(_targetable);
            }
        }
    }
}
