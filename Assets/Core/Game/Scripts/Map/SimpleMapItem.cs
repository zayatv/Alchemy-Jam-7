using System.Collections.Generic;
using UnityEngine;

namespace Core.Game.Scripts.Map
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SimpleMapItem")]
    public class SimpleMapItem : AbstractMapItem
    {
        SimpleMapItem(Vector3Int[] poss, GameObject item, int count) : base(poss, item, count)
        {
            
        }

        public override List<GameObject> PlaceItem(Vector3Int basis, int rotation)
        {
            basis = basis + new Vector3Int(0, 1, 0);
            Vector3Int[] poss = base.Positions.Clone() as Vector3Int[];
            for (int i = 0; i < poss.Length; i++)
                for(int j = 0; j < rotation; j++)
                    poss[i] = Rotate90Deg(poss[i]);
            List<GameObject> items = new();
            foreach (Vector3Int pos in poss)
                items.Add(Instantiate(base.Item, pos + basis, Quaternion.identity));
            
            return items;
        }
    }
}