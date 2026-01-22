using Core.Game.Combat;
using Core.Game.Combat.Events;
using Core.Systems.Events;
using Core.Systems.ServiceLocator;
using Core.Systems.UI.HUD;
using Core.Systems.Update;
using TMPro;
using UnityEngine;

namespace Core.Game.GameStates.DungeonState.UI.HUD
{
    public class EnemyHealthHUDItem : HUDItem, ILateUpdatable
    {
        [Header("Enemy Health Settings")]
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Canvas worldSpaceCanvas;
        
        [Header("Behavior")]
        [SerializeField] private bool hideWhenFullHealth = true;
        [SerializeField] private float showDurationOnDamage = 3f;

        private float _showTimer;
        private UnityEngine.Camera _mainCamera;
        private IUpdateService _updateService;

        public int LateUpdatePriority => 0;

        protected override void OnInitialize()
        {
            if (healthComponent == null)
            {
                healthComponent = GetComponentInParent<HealthComponent>();
            }

            if (worldSpaceCanvas == null)
            {
                worldSpaceCanvas = GetComponent<Canvas>();
            }

            if (worldSpaceCanvas != null && worldSpaceCanvas.renderMode != RenderMode.WorldSpace)
            {
                 Debug.LogWarning($"[EnemyHealthHUDItem] Canvas on {name} is not WorldSpace!");
            }

            _mainCamera = UnityEngine.Camera.main;
            
            if (HUDService == null && ServiceLocator.TryGet(out IHUDService hudService))
            {
                 hudService.RegisterItem(this);
            }
            
            if (healthComponent != null)
            {
                UpdateHealthUI(healthComponent.CurrentHealth, healthComponent.MaxHealth);
                
                if (hideWhenFullHealth && healthComponent.CurrentHealth >= healthComponent.MaxHealth)
                {
                    HideImmediate();
                }
            }
            
            _updateService = ServiceLocator.Get<IUpdateService>();
            _updateService.Register(this);
        }

        private void OnDestroy()
        {
            if (_updateService == null)
                ServiceLocator.TryGet(out _updateService);
            
            _updateService?.Unregister(this);
        }

        public void OnLateUpdate(float deltaTime)
        {
            if (worldSpaceCanvas != null && _mainCamera != null)
            {
                worldSpaceCanvas.transform.rotation = Quaternion.LookRotation(worldSpaceCanvas.transform.position - _mainCamera.transform.position);
            }

            if (_showTimer > 0)
            {
                _showTimer -= deltaTime;
                
                if (_showTimer <= 0 && hideWhenFullHealth && healthComponent.CurrentHealth >= healthComponent.MaxHealth)
                {
                    Hide();
                }
            }
        }

        protected override bool ValidateData(IEvent eventData)
        {
            if (healthComponent == null) return false;

            if (eventData is DamageDealtEvent damageEvent)
            {
                return (HealthComponent) damageEvent.Target == healthComponent;
            }
            
            if (eventData is EntityDeathEvent deathEvent)
            {
                return (HealthComponent) deathEvent.Entity == healthComponent;
            }

            return base.ValidateData(eventData);
        }

        protected override void UpdateData(IEvent eventData)
        {
            if (eventData is DamageDealtEvent damageEvent)
            {
                UpdateHealthUI(damageEvent.RemainingHealth, damageEvent.Target.MaxHealth);
                
                _showTimer = showDurationOnDamage;
                
                if (!IsVisible)
                {
                    Show();
                }
            }
            else if (eventData is EntityDeathEvent)
            {
                Hide();
            }
        }

        private void UpdateHealthUI(int current, int max)
        {
            if (healthText != null)
            {
                healthText.text = $"{current}/{max}";
            }
        }
    }
}
