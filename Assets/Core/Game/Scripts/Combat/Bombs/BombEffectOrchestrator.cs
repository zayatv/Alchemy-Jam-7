using Core.Systems.Audio;
using Core.Systems.Grid;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Combat.Bombs
{
    public class BombEffectOrchestrator
    {
        #region Fields

        private readonly IVFXService _vfxService;
        private readonly IAudioService _audioService;
        private readonly IGridService _gridService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BombEffectOrchestrator"/> class.
        /// </summary>
        /// <param name="vfxService">The VFX service for spawning visual effects.</param>
        /// <param name="audioService">The audio service for playing sounds.</param>
        /// <param name="gridService">The grid service for tile-to-world conversions.</param>
        public BombEffectOrchestrator(IVFXService vfxService, IAudioService audioService, IGridService gridService)
        {
            _vfxService = vfxService;
            _audioService = audioService;
            _gridService = gridService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Plays the bomb placement sound at the specified position.
        /// </summary>
        /// <param name="definition">The bomb definition containing the placement sound.</param>
        /// <param name="position">The world position to play the sound at.</param>
        public void PlayPlacementSound(BombDefinition definition, Vector3 position)
        {
            if (definition.PlacementSound != null)
                _audioService.PlayAtPosition(definition.PlacementSound, position);
        }

        /// <summary>
        /// Spawns the fuse visual effect attached to the specified parent transform.
        /// </summary>
        /// <param name="definition">The bomb definition containing the fuse VFX.</param>
        /// <param name="parent">The transform to attach the VFX to.</param>
        /// <returns>A handle to the spawned VFX, or an invalid handle if no VFX is defined.</returns>
        public VFXHandle SpawnFuseVFX(BombDefinition definition, Transform parent)
        {
            if (definition.FuseVFX != null)
                return _vfxService.SpawnAttached(definition.FuseVFX, parent);

            return VFXHandle.Invalid;
        }

        /// <summary>
        /// Plays the fuse sound attached to the specified parent transform.
        /// </summary>
        /// <param name="definition">The bomb definition containing the fuse sound.</param>
        /// <param name="parent">The transform to attach the sound to.</param>
        /// <returns>A handle to the playing audio, or an invalid handle if no sound is defined.</returns>
        public AudioHandle PlayFuseSound(BombDefinition definition, Transform parent)
        {
            if (definition.FuseSound != null)
                return _audioService.PlayAttached(definition.FuseSound, parent);

            return AudioHandle.Invalid;
        }

        /// <summary>
        /// Spawns a tile indicator VFX at the specified tile coordinate.
        /// </summary>
        /// <param name="definition">The bomb definition containing the indicator VFX.</param>
        /// <param name="tile">The tile coordinate to spawn the indicator at.</param>
        /// <param name="radius">The radius of the explosion area.</param>
        /// <returns>A handle to the spawned VFX, or an invalid handle if no VFX is defined.</returns>
        public VFXHandle SpawnIndicatorVFX(BombDefinition definition, TileCoordinate tile, float radius)
        {
            if (definition.AreaIndicatorVFX == null)
                return VFXHandle.Invalid;

            Vector3 position = _gridService.TileToCenteredWorld(tile);
            var handle = _vfxService.Spawn(definition.AreaIndicatorVFX, position);
            
            _vfxService.SetScale(handle, Vector3.one * radius);

            return handle;
        }

        /// <summary>
        /// Spawns explosion visual effects at the specified tile coordinate.
        /// </summary>
        /// <param name="definition">The bomb definition containing the explosion VFX.</param>
        /// <param name="centerTile">The center tile of the explosion.</param>
        /// <param name="radius">The radius of the explosion area.</param>
        public void SpawnExplosionVFX(BombDefinition definition, TileCoordinate centerTile, float radius)
        {
            Vector3 position = _gridService.TileToCenteredWorld(centerTile);

            if (definition.AreaExplosionVFX != null)
            {
                var handle = _vfxService.Spawn(definition.AreaExplosionVFX, position);
                
                _vfxService.SetScale(handle, Vector3.one * radius);
            }
        }

        /// <summary>
        /// Plays the explosion sound at the specified position.
        /// </summary>
        /// <param name="definition">The bomb definition containing the explosion sound.</param>
        /// <param name="position">The world position to play the sound at.</param>
        public void PlayExplosionSound(BombDefinition definition, Vector3 position)
        {
            if (definition.ExplosionSound != null)
                _audioService.PlayAtPosition(definition.ExplosionSound, position);
        }

        /// <summary>
        /// Stops the fuse visual and audio effects.
        /// </summary>
        /// <param name="vfxHandle">The handle to the fuse VFX to stop.</param>
        /// <param name="audioHandle">The handle to the fuse audio to stop.</param>
        public void StopFuseEffects(VFXHandle vfxHandle, AudioHandle audioHandle)
        {
            if (vfxHandle.IsValid)
                _vfxService.Stop(vfxHandle);

            if (audioHandle.IsValid)
                _audioService.Stop(audioHandle);
        }

        /// <summary>
        /// Stops a visual effect by its handle.
        /// </summary>
        /// <param name="handle">The handle to the VFX to stop.</param>
        public void StopVFX(VFXHandle handle)
        {
            if (handle.IsValid)
                _vfxService.Stop(handle);
        }

        #endregion
    }
}
