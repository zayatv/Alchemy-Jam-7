using System.Collections.Generic;
using Core.Game.Combat.Events;
using Core.Game.Pickups.Events;
using Core.Systems.Combat;
using Core.Systems.Events;
using Core.Systems.UI.HUD;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.UI.HUD
{
    public class PlayerHealthHUDItem : HUDItem
    {
        [Header("Heart Configuration")]
        [Tooltip("Prefab containing an Image component for the heart icon")]
        [SerializeField] private GameObject heartPrefab;
        
        [Tooltip("Container Transform where heart icons will be spawned")]
        [SerializeField] private Transform heartContainer;
        
        [Tooltip("Sprite for a full heart")]
        [SerializeField] private Sprite fullHeartSprite;
        
        [Tooltip("Sprite for an empty heart")]
        [SerializeField] private Sprite emptyHeartSprite;

        [Tooltip("How much health one heart represents. usually 1.")]
        [SerializeField] private int healthPerHeart = 1;

        private List<Image> _heartImages = new List<Image>();

        protected override bool ValidateData(IEvent eventData)
        {
            if (eventData is DamageDealtEvent damageEvent)
            {
                if (damageEvent.Target is ITargetable targetable)
                    return targetable.TargetType == TargetType.Player;
                
                return false;
            }
            
            if (eventData is HealthPickupCollectedEvent)
            {
                return true;
            }
            
            if (eventData is EntityDeathEvent deathEvent)
            {
                return deathEvent.TargetType == TargetType.Player;
            }

            return base.ValidateData(eventData);
        }

        protected override void UpdateData(IEvent eventData)
        {
            if (eventData is DamageDealtEvent damageEvent)
            {
                UpdateHearts(damageEvent.RemainingHealth, damageEvent.Target.MaxHealth);
            }
            else if (eventData is HealthPickupCollectedEvent pickupEvent)
            {
                UpdateHearts(pickupEvent.NewHealth, pickupEvent.MaxHealth);
            }
            else if (eventData is EntityDeathEvent)
            {
                 UpdateHearts(0, _heartImages.Count * healthPerHeart);
            }
        }
        
        private void UpdateHearts(int currentHealth, int maxHealth)
        {
            if (heartPrefab == null || heartContainer == null) return;

            int requiredHearts = Mathf.CeilToInt((float)maxHealth / healthPerHeart);
            
            if (_heartImages.Count != requiredHearts)
            {
                RebuildHearts(requiredHearts);
            }

            for (int i = 0; i < _heartImages.Count; i++)
            {
                int heartValue = (i + 1) * healthPerHeart;
                
                if (currentHealth >= heartValue)
                    _heartImages[i].sprite = fullHeartSprite;
                else
                    _heartImages[i].sprite = emptyHeartSprite;
            }
        }

        private void RebuildHearts(int count)
        {
            foreach (var img in _heartImages)
            {
                if (img != null) Destroy(img.gameObject);
            }
            
            _heartImages.Clear();
            
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(heartPrefab, heartContainer);
                var img = go.GetComponent<Image>();
                
                if (img != null)
                    _heartImages.Add(img);
            }
        }
    }
}
