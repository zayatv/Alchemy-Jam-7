using Core.Systems.Combat;
using Core.Systems.Events;
using Core.Systems.Grid;

namespace Core.Game.Combat.Bombs
{
    public struct BombDetonationEvent : IEvent
    {
        #region Fields
        
        public BombHandle Handle;
        public TileCoordinate CenterTile;
        public float ExplosionRadius;
        public int Damage;
        public TeamType SourceTeam;

        #endregion
    }
}
