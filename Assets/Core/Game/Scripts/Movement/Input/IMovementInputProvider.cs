using UnityEngine;

namespace Core.Game.Movement.Input
{
    public interface IMovementInputProvider
    {
        /// <summary>
        /// Retrieves the movement input as a 2D vector representing the player's desired movement direction.
        /// </summary>
        /// <returns>
        /// A Vector2 containing the horizontal (x) and vertical (y) input values.
        /// The magnitude and direction of the vector correspond to the input provided by the player.
        /// Returns a zero vector if input is disabled or there is no movement input.
        /// </returns>
        Vector2 GetMovementInput();

        /// <summary>
        /// Determines whether the player has requested to activate sprinting functionality.
        /// </summary>
        /// <returns>
        /// A boolean value indicating the sprint request state:
        /// True if sprint input is detected and input is enabled;
        /// False otherwise.
        /// </returns>
        bool IsSprintRequested();

        /// <summary>
        /// Indicates whether input functionality is currently enabled for the player.
        /// </summary>
        /// <returns>
        /// A boolean value representing the input state:
        /// True if input is enabled and responsive;
        /// False if input is disabled or unavailable.
        /// </returns>
        bool IsInputEnabled();
    }
}