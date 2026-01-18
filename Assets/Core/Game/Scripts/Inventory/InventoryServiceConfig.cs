using System;
using Core.Systems.Logging;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Inventory
{
    [Serializable]
    public class InventoryServiceConfig : ServiceConfig
    {
        [Header("Bomb Configuration")]
        [Tooltip("Initial number of bombs the player starts with.")]
        [SerializeField] private int startingBombs = 5;

        [Tooltip("Maximum number of bombs the player can carry.")]
        [SerializeField] private int maxBombs = 99;
        
        IInventoryService _inventoryService;

        public override void Install(IServiceInstallHelper helper)
        {
            if (startingBombs < 0)
            {
                GameLogger.Log(LogLevel.Warning, $"Starting bombs cannot be negative. Using 0 instead of {startingBombs}.");
                
                startingBombs = 0;
            }

            if (maxBombs <= 0)
            {
                GameLogger.Log(LogLevel.Warning, $"Max bombs must be greater than zero. Using 1 instead of {maxBombs}.");
                
                maxBombs = 1;
            }

            if (startingBombs > maxBombs)
            {
                GameLogger.Log(LogLevel.Warning, $"Starting bombs ({startingBombs}) exceeds max bombs ({maxBombs}). Clamping to {maxBombs}.");
                
                startingBombs = maxBombs;
            }

            _inventoryService = new InventoryService(startingBombs, maxBombs);
            
            helper.Register(_inventoryService);

            GameLogger.Log(LogLevel.Debug, $"Inventory Service installed. Starting Bombs: {startingBombs}, Max Bombs: {maxBombs}");
        }
    }
}
