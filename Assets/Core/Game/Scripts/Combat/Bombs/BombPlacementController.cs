using Core.Game.Input.Contexts;
using Core.Systems.Grid;
using Core.Systems.Update;
using UnityEngine;

namespace Core.Game.Combat.Bombs
{
    public class BombPlacementController : IUpdatable
    {
        #region Fields

        private readonly IBombService _bombService;
        private readonly IGridService _gridService;
        private readonly BombInputContext _bombInput;
        private readonly Transform _playerTransform;

        #endregion

        #region Properties

        public int UpdatePriority => 60;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BombPlacementController"/> class.
        /// </summary>
        /// <param name="bombService">The bomb service for placing and detonating bombs.</param>
        /// <param name="gridService">The grid service for world-to-tile conversion.</param>
        /// <param name="bombInput">The input context for bomb actions.</param>
        /// <param name="playerTransform">The player's transform for determining bomb placement location.</param>
        public BombPlacementController(
            IBombService bombService,
            IGridService gridService,
            BombInputContext bombInput,
            Transform playerTransform)
        {
            _bombService = bombService;
            _gridService = gridService;
            _bombInput = bombInput;
            _playerTransform = playerTransform;
        }

        #endregion

        #region IUpdatable Implementation

        public void OnUpdate(float deltaTime)
        {
            if (!_bombInput.BombPressed)
                return;

            if (_bombService.HasActiveBomb && _bombService.CurrentStats.RequiresManualDetonation)
            {
                _bombService.DetonateActiveBomb();
                return;
            }

            if (!_bombService.CanPlaceBomb)
            {
                Debug.Log("Can't place bomb");
                return;
            }

            TileCoordinate tile = _gridService.WorldToTile(_playerTransform.position);
            _bombService.PlaceBomb(tile);
        }

        #endregion
    }
}
