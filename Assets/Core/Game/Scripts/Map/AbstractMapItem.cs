using System.Collections.Generic;
using UnityEngine;

namespace Core.Game.Scripts.Map
{
    public abstract class AbstractMapItem : ScriptableObject
    {
        public int Count;
        public Vector3Int[] Positions;
        public GameObject Item;
        
        public AbstractMapItem(Vector3Int[] positions, GameObject item, int  count)
        {
            this.Positions = positions.Clone() as Vector3Int[];
            this.Item = item;
            this.Count = count;
        }

        public abstract List<GameObject> PlaceItem(Vector3Int basis, int rotation);

        /// <summary>
        /// occupy the map with offset
        /// </summary>
        /// <param name="map"></param>
        /// <param name="basis"></param>
        /// <param name="rotations"></param>
        /// <returns></returns>
        public void Occupy(Dictionary<Vector3Int, CellType> map, Vector3Int basis, int rotations)
        {
            Vector3Int[] poss = Positions.Clone() as Vector3Int[];
            for(int i = 0; i < Positions.Length; i++)
                poss[i] += basis;
                
            for (int r = 0; r < rotations; r++)
                for (int e = 0; e < poss.Length; e++)
                    poss[e] = Rotate90Deg(poss[e]);

            foreach (var pos in poss)
            {
//                map[pos + new Vector3Int(0, 1, 0)] = CellType.Other;
                map[pos] = CellType.OccupiedFloor;
            }
        }
        
        /// <summary>
        /// check if this can occupy map. if yes, second return value is amount of rotations needed which is needed to actually place it
        /// </summary>
        /// <param name="map"></param>
        /// <param name="basis"></param>
        /// <returns></returns>
        public (bool, int) CanPlace(Dictionary<Vector3Int, CellType> map, Vector3Int basis)
        {
            Vector3Int[] poss = Positions.Clone() as Vector3Int[];
            for (int i = 0; i < 4; i++)
            {
                bool canPlace = true;
                foreach (Vector3Int pos in poss)
                {
                    if(!map.ContainsKey(pos + basis) || map[pos + basis] != CellType.Floor)
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    return (true, i);
                }
                
                for (int e = 0; e < poss.Length; e++)
                    poss[e] = Rotate90Deg(poss[e]);
            }
            
            return (false, -1);
        }

        public static Vector3Int Rotate90Deg(Vector3Int position)
        {
            return new Vector3Int(position.z, 0, -position.x);
        }
    }
}