using System;
using UnityEngine;
using Core.Systems.ServiceLocator;
using Core.Systems.Logging;
using Object = UnityEngine.Object;

namespace Core.Systems.Pooling
{
    [Serializable]
    public class ObjectPoolServiceConfig : ServiceConfig
    {
        [Header("Pool Configuration")]
        [Tooltip("Default pool size for prewarming (0 = no prewarming by default)")]
        [SerializeField] private int defaultPoolSize = 10;

        [Header("References")]
        [SerializeField] private ObjectPoolService service;

        public override void Install(IServiceInstallHelper helper)
        {
            helper.Register<IObjectPoolService>(service);

            GameLogger.Log(LogLevel.Info, $"[ObjectPoolServiceConfig] ObjectPoolService installed (DefaultPoolSize: {defaultPoolSize})");
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            if (service != null)
            {
                Object.Destroy(service.gameObject);
                service = null;
            }

            GameLogger.Log(LogLevel.Debug, "[ObjectPoolServiceConfig] ObjectPoolService uninstalled");
        }
    }
}
