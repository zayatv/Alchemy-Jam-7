using UnityEngine;

namespace Core.Game.Combat
{
    [CreateAssetMenu(fileName = "HealthConfig", menuName = "Core/Game/Combat/Health Config")]
    public class HealthConfig : ScriptableObject
    {
        [Header("Health Settings")]
        [Tooltip("Maximum health value for this entity")]
        [SerializeField] private int maxHealth = 3;

        [Header("Immunity Settings")]
        [Tooltip("Whether this entity gains temporary immunity after taking damage")]
        [SerializeField] private bool enableDamageImmunity;

        [Tooltip("Duration of immunity in seconds after taking damage")]
        [SerializeField] private float immunityDuration = 1f;

        public int MaxHealth => maxHealth;
        public bool EnableDamageImmunity => enableDamageImmunity;
        public float ImmunityDuration => immunityDuration;
    }
}
