using System;
using UnityEngine;
using Core.Game.Camera.Data;
using Core.Systems.Logging;
using Core.Systems.ServiceLocator;
using Core.Systems.Update;

namespace Core.Game.Camera
{
    [Serializable]
    public class CameraServiceConfig : ServiceConfig
    {
        [Header("Configuration")]
        [Tooltip("Camera configuration asset with all camera parameters")]
        [SerializeField] private CameraConfig cameraConfig;

        [Header("Scene References")]
        [Tooltip("Reference to the CameraController component on the Main Camera GameObject")]
        [SerializeField] private CameraController cameraController;

        private CameraService _cameraService;

        public override void Install(IServiceInstallHelper helper)
        {
            // Validate configuration
            if (cameraConfig == null)
            {
                GameLogger.Log(LogLevel.Error, "[CameraServiceConfig] CameraConfig is null! Cannot install camera service.");
                return;
            }

            if (cameraController == null)
            {
                GameLogger.Log(LogLevel.Error, "[CameraServiceConfig] CameraController is null! Cannot install camera service.");
                return;
            }

            // Create camera service
            _cameraService = new CameraService(cameraConfig, cameraController);

            // Register with ServiceLocator
            helper.Register<ICameraService>(_cameraService);

            // Register with UpdateService
            RegisterWithUpdateService(helper);

            GameLogger.Log(LogLevel.Info, "[CameraServiceConfig] Camera service installed successfully");
        }

        public override void OnInstalled(IServiceInstallHelper helper)
        {
            // Find player and set as camera target
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _cameraService.SetTarget(player.transform);
                GameLogger.Log(LogLevel.Debug, "[CameraServiceConfig] Camera target set to Player");
            }
            else
            {
                GameLogger.Log(LogLevel.Warning, "[CameraServiceConfig] Player GameObject not found in scene. Camera will not have a target.");
            }
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            // Unregister from UpdateService
            if (helper.TryGet(out IUpdateService updateService))
            {
                updateService.Unregister(_cameraService);
                GameLogger.Log(LogLevel.Debug, "[CameraServiceConfig] Unregistered from UpdateService");
            }
        }

        private void RegisterWithUpdateService(IServiceInstallHelper helper)
        {
            if (!helper.TryGet(out IUpdateService updateService))
            {
                GameLogger.Log(LogLevel.Warning, "[CameraServiceConfig] UpdateService not found. Camera will not receive updates.");
                return;
            }

            updateService.Register(_cameraService);
            GameLogger.Log(LogLevel.Debug, "[CameraServiceConfig] Registered with UpdateService (LateUpdate priority: 100)");
        }
    }
}
