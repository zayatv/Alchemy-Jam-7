using UnityEngine;

namespace Core.Game.Combat
{
    [CreateAssetMenu(fileName = "HealthConfig", menuName = "Core/Game/Combat/Health Config")]
    public class HealthConfig : ScriptableObject
    {
        [Header("Health Settings")]
        [Tooltip("Maximum health value for this entity")]
        [SerializeField] private int maxHealth = 3;
        
        public int MaxHealth => maxHealth;
    }
}
