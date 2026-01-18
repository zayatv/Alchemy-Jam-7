using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems.Grid;
using Core.Systems.Combat;
using Core.Systems.Logging;
using UnityEngine;

namespace Core.Game.Combat
{
    public class TargetRegistry : ITargetRegistry
    {
        #region Fields
        
        private readonly Dictionary<TileCoordinate, List<ITargetable>> _targetsByTile;
        private readonly List<ITargetable> _allTargets;
        
        #endregion
        
        public TargetRegistry(IGridService gridService)
        {
            _targetsByTile = new Dictionary<TileCoordinate, List<ITargetable>>();
            _allTargets = new List<ITargetable>();
        }

        public void Register(ITargetable target)
        {
            if (target == null)
            {
                GameLogger.Log(LogLevel.Warning, "Attempted to register null target.");
                
                return;
            }

            if (_allTargets.Contains(target))
            {
                GameLogger.Log(LogLevel.Warning, "Target already registered.");
                
                return;
            }

            _allTargets.Add(target);
        }

        public void Unregister(ITargetable target)
        {
            if (target == null)
            {
                GameLogger.Log(LogLevel.Warning, "Attempted to unregister null target.");
                
                return;
            }

            _allTargets.Remove(target);
        }

        public IReadOnlyList<ITargetable> GetTargetsInRadius(Vector3 center, float radius)
        {
            if (radius <= 0f)
                return Array.Empty<ITargetable>();
            
            List<ITargetable> targets = new List<ITargetable>();
            Collider[] colliders = Physics.OverlapSphere(center, radius);

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out ITargetable target))
                {
                    targets.Add(target);
                }
            }
            
            return targets;
        }

        public IReadOnlyList<ITargetable> GetTargetsOfType(TargetType type)
        {
            return _allTargets.Where(t => t != null && t.IsTargetable && t.TargetType == type).ToList();
        }

        public IReadOnlyList<ITargetable> GetTargetsForTeam(TeamType attackerTeam)
        {
            if (attackerTeam == TeamType.Player)
            {
                return _allTargets
                    .Where(t => t != null && t.IsTargetable &&
                        (t.TargetType == TargetType.Enemy || t.TargetType == TargetType.Destructible || t.TargetType == TargetType.Player))
                    .ToList();
            }
            else if (attackerTeam == TeamType.Enemy)
            {
                return _allTargets
                    .Where(t => t != null && t.IsTargetable && t.TargetType == TargetType.Player)
                    .ToList();
            }
            else
            {
                return _allTargets
                    .Where(t => t != null && t.IsTargetable)
                    .ToList();
            }
        }

        /// <summary>
        /// Recalculates the tile positions of all registered targets.
        /// Uses collider bounds to determine all tiles a target occupies,
        /// allowing large or flying enemies to be registered on multiple tiles.
        /// Called before each query to handle dynamic movement.
        /// </summary>
        private void RefreshTileIndices()
        {
            _targetsByTile.Clear();

            foreach (ITargetable target in _allTargets)
            {
                if (target == null)
                    continue;

                TileCoordinate[] occupiedTiles = target.GetOccupiedTiles();

                foreach (TileCoordinate tile in occupiedTiles)
                {
                    if (!_targetsByTile.TryGetValue(tile, out List<ITargetable> targetsOnTile))
                    {
                        targetsOnTile = new List<ITargetable>();
                        
                        _targetsByTile[tile] = targetsOnTile;
                    }

                    targetsOnTile.Add(target);
                }
            }
        }
    }
}
