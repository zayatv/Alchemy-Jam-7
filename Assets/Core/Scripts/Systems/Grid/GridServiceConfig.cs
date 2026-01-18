using System;
using UnityEngine;
using Core.Systems.Logging;
using Core.Systems.ServiceLocator;

namespace Core.Systems.Grid
{
    [Serializable]
    public class GridServiceConfig : ServiceConfig
    {
        [Header("Configuration")]
        [Tooltip("Grid configuration asset with tile size, bounds, and other grid parameters")]
        [SerializeField] private GridConfig gridConfig;

        private GridService _gridService;
        
        public override void Install(IServiceInstallHelper helper)
        {
            if (gridConfig == null)
            {
                GameLogger.Log(LogLevel.Error, "[GridServiceConfig] GridConfig is null! Cannot install grid service.");
                
                return;
            }

            _gridService = new GridService(gridConfig);

            helper.Register<IGridService>(_gridService);

            GameLogger.Log(LogLevel.Info, "[GridServiceConfig] Grid service installed successfully");
            GameLogger.Log(LogLevel.Debug, $"[GridServiceConfig] Tile Size: {gridConfig.TileSize}");
        }
    }
}
