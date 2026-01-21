using Sirenix.OdinInspector;
using UnityEditor;

namespace Core.Game.Scripts.Map
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = System.Random;
    using System.Linq;

    public class MapGenerator : MonoBehaviour
    {
        private static readonly Vector3Int UpDir = new Vector3Int(0, 0, 1);
        private static readonly Vector3Int DownDir = new Vector3Int(0, 0, -1);
        private static readonly Vector3Int RightDir = new Vector3Int(1, 0, 0);
        private static readonly Vector3Int LeftDir = new Vector3Int(-1, 0, 0);

        private readonly HashSet<GameObject> PlacedTiles = new();
        private readonly HashSet<GameObject> PlacedItems = new();

        //Variables
        private Dictionary<Vector3Int, CellType> _tiles = new ();

        public List<AbstractMapItem> defaultItems = new ();

        public GameObject floor;
        public GameObject wall;
        public int seed = 123456789;
        public int scale = 1;

        public SeedGenerator seedGenerator;
        public int defaultIterations = 10;
        public int defaultSteps = 100;


        private Random _rnd;

        void Start()
        {
            
        }

        public void RegenerateSeed()
        {
            seed = seedGenerator switch
            {
                SeedGenerator.Identity => seed,
                SeedGenerator.Random => UnityEngine.Random.Range(int.MinValue, int.MaxValue),
                _ => throw new ArgumentOutOfRangeException()
            };            
            _rnd = new Random(seed);
        }

        public (Vector3Int[][], int[][]) GenerateMap()
        {
            return GenerateMap(defaultIterations, defaultSteps, defaultItems);
        }
        
        public (Vector3Int[][], int[][]) GenerateMap(int its, int ss, List<AbstractMapItem> items)
        {
            if(_rnd == null)
                RegenerateSeed();
            
            ClearTiles();
            ClearItems();
            
            _tiles.Clear();
            _tiles[new Vector3Int(0, 0, 0)] = CellType.Floor;
            double distance = Math.Log(ss);
            for (int i = 0; i < its; i++)
            {
                double iter = Math.Floor(i / 12f);
                
                Vector3Int start = new Vector3Int(0, 0, (int) (iter * distance));
                
                for (int r = 0; r < (i % 4); r++)
                {
                    start = AbstractMapItem.Rotate90Deg(start);                    
                }
                WalkFloor(ss, start);
            }

            Vector3Int[][] basiss = new Vector3Int[items.Count][];
            int[][] rotations = new int[items.Count][];
            for (int a = 0; a < basiss.Length; a++)
            {
                AbstractMapItem item = items[a];
                List<int> rots = new List<int>();
                List<Vector3Int> bass = new List<Vector3Int>();
                List<Vector3Int> prevs = new  List<Vector3Int>();
                List<Vector3Int> shuffled = _tiles.Keys.OrderBy(x => _rnd.Next()).ToList();
                for (int i = 0; i < item.Count; i++)
                {
                    (Vector3Int, int) asd = TryPlaceItem(item, prevs, ss, ss, shuffled);
                    if (asd.Item2 < 0)
                        break;
                    rots.Add(asd.Item2);
                    bass.Add(asd.Item1);
                    prevs.Add(asd.Item1);
                }
                basiss[a] = bass.ToArray();
                rotations[a] = rots.ToArray();
            }
            
            CreateWalls();
            PlaceTiles();
            
            PlaceItems(items, basiss, rotations);
            
            return (basiss, rotations);
        }

        private void PlaceItems(List<AbstractMapItem> items, Vector3Int[][] basisss, int[][] rotations)
        {
            for (int i = 0; i < items.Count; i++)
            {
                AbstractMapItem item = items[i];
                Vector3Int[] basiss = basisss[i];
                int[] rots = rotations[i];

                for (int k = 0; k < basiss.Length; k++)
                {
                    if (rots[k] < 0)
                        break;
                    foreach (GameObject o in item.PlaceItem(basiss[k], rots[k]))
                        PlacedItems.Add(o);
                }
            }
        }

        private void PlaceTiles()
        {
            foreach (var entry in _tiles)
            {
                if (entry.Value == CellType.Wall)
                    PlacedTiles.Add(Instantiate(wall, new Vector3(entry.Key.x * scale, entry.Key.y + 1, entry.Key.z * scale), Quaternion.identity));
                else if (entry.Value == CellType.Floor || entry.Value == CellType.OccupiedFloor)
                    PlacedTiles.Add(Instantiate(floor, new Vector3(entry.Key.x * scale, entry.Key.y, entry.Key.z * scale), Quaternion.identity));
            }
        }
        
        private (Vector3Int, int) TryPlaceItem(AbstractMapItem item, List<Vector3Int> prevs, double distanceSqr, int steps, List<Vector3Int> shuffled)
        {
            double decrF = 1f - 1f / (steps);
            decrF *= decrF;
//            List<Vector3Int> shuffled = new List<Vector3Int>(_tiles.Keys);
//            shuffled = shuffled.OrderBy(x => _rnd.Next()).ToList();
            int i = 0;
            foreach (Vector3Int basis in shuffled)
            {
                i++;
                double closest = Double.MaxValue;
                foreach (Vector3Int prev in prevs)
                {
                    closest = Math.Min(closest, (basis - prev).sqrMagnitude);
                    if ((basis - prev).sqrMagnitude < distanceSqr)
                    {
                        distanceSqr = distanceSqr * decrF;
                        break;
                    }    
                }

                if (closest < distanceSqr)
                    continue;
                
                (bool, int) res = item.CanPlace(_tiles, basis);
                if (res.Item1)
                {
                    item.Occupy(_tiles, basis, res.Item2);
                    Debug.Log("iterations needed: " + i + " shortest dist sqr " + closest + " req " + distanceSqr);
                    return (basis, res.Item2);
                }
            }
            return (new Vector3Int(0,0), -1);
        }

        private void ClearTiles()
        {
            foreach (GameObject go in PlacedTiles)
                Destroy(go);
            PlacedTiles.Clear();
        }
        
        private void ClearItems()
        {
            foreach (GameObject go in PlacedItems)
                Destroy(go);
            PlacedItems.Clear();
        }

        private Vector3Int GetDirection(Vector3Int curPos)
        {

            //x horizontal, z vertical
            double hypo = curPos.x * curPos.x + curPos.z * curPos.z;
            double cos = curPos.x == 0 ? 0 : curPos.x / hypo;
            double sin = curPos.z == 0 ? 0 : curPos.z / hypo;

            cos = Math.Sign(cos) * Math.Pow(Math.Abs(cos), 1/4d);
            sin = Math.Sign(sin) * Math.Pow(Math.Abs(sin), 1/4d);
            //prioritize moving away from center for less clustering
            double upChance = 1 + sin;
            double downChance = 1 - sin;
            double rightChance = 1 + cos;
            double leftChance = 1 - cos;

            double r = _rnd.NextDouble() * (upChance + downChance + rightChance + leftChance);
            if (r <= upChance)
                return UpDir;
            r -= upChance;
            if (r <= downChance)
                return DownDir;
            r -= downChance;
            if (r <= rightChance)
                return RightDir;
            return LeftDir;
        }

        private void WalkFloor(int ss, Vector3Int curPos)
        {
            for (int i = 0; i < ss; i++)
            {
                Vector3Int direction = GetDirection(curPos);
                curPos = curPos + direction;
                _tiles[curPos] = CellType.Floor;
            }
        }

        private void CreateWalls()
        {
            Dictionary<Vector3Int, CellType> walls = new();
            foreach (Vector3Int key in _tiles.Keys)
            {
                foreach (Vector3Int neighbor in new[] { key + UpDir, key + DownDir, key + LeftDir, key + RightDir })
                {
                    if (!_tiles.ContainsKey(neighbor))
                        walls[neighbor] = CellType.Wall;
                }
            }

            foreach (var w in walls)
            {
                if (!_tiles.ContainsKey(w.Key))
                    _tiles.Add(w.Key, w.Value);
            }
        }
    }

    public enum SeedGenerator
    {
        Identity,
        Random
    }

    public enum CellType
    {
        Wall,
        Floor,
        OccupiedFloor
    }
}