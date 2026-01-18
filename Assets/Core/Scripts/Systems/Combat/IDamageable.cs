using System;

namespace Core.Systems.Combat
{
    public interface IDamageable
    {
        /// <summary>
        /// The current health of this entity.
        /// Will be between 0 and MaxHealth (inclusive).
        /// </summary>
        int CurrentHealth { get; }

        /// <summary>
        /// The maximum health this entity can have.
        /// </summary>
        int MaxHealth { get; }

        /// <summary>
        /// Whether this entity is still alive.
        /// Typically false when CurrentHealth reaches 0.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// The team this entity belongs to.
        /// Used to determine friendly fire and damage rules.
        /// </summary>
        TeamType Team { get; }

        /// <summary>
        /// Applies damage to this entity.
        /// This is the main entry point for all damage application.
        /// Implementations should validate the damage, apply it to health,
        /// and trigger appropriate events and effects.
        /// </summary>
        /// <param name="damage">Complete information about the damage event.</param>
        void TakeDamage(DamageInfo damage);

        /// <summary>
        /// Event raised when this entity receives damage.
        /// Fired after damage is applied and health is updated.
        /// Allows other systems (VFX, SFX, UI, etc.) to react to damage.
        /// </summary>
        event Action<DamageInfo> OnDamageReceived;
    }
}
