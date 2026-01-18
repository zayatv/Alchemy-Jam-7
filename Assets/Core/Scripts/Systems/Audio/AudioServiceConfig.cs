using System;
using Core.Systems.Logging;
using UnityEngine;
using Core.Systems.ServiceLocator;

namespace Core.Systems.Audio
{
    [Serializable]
    public class AudioServiceConfig : ServiceConfig
    {
        [SerializeField]
        private AudioService audioService;
        
        [SerializeField]
        [Tooltip("Number of AudioSources to pre-create in the pool")]
        private int audioSourcePoolSize = 20;

        public override void Install(IServiceInstallHelper helper)
        {
            if (audioService == null)
            {
                GameLogger.Log(LogLevel.Error, "[AudioServiceConfig] AudioService is null! Cannot install audio service.");
                
                return;
            }
            
            audioService.Initialize(audioSourcePoolSize);

            helper.Register<IAudioService>(audioService);

            GameLogger.Log(LogLevel.Debug, $"AudioService installed with pool size: {audioSourcePoolSize}");
        }
    }
}
