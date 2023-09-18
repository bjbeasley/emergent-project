using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "Data", menuName = "PropUVPacker", order = 2)]
public class PropUVPacker : ScriptableObject
{
    [SerializeField]
    public List<PropUVMap> maps = new List<PropUVMap>();

    private List<PropUVMap> sortedMaps = null;

    public PropUVMap GetPropUVMap (float maxSize, int seed)
    {
        float maxSizeSqr = maxSize * maxSize;

        if(sortedMaps == null || sortedMaps.Count != maps.Count)
        {
            sortedMaps = maps.OrderBy(m => m.GetSqrLengthOfFirstEdge()).ToList();
        }

        int maxIndex = sortedMaps.Count;

        while(maxIndex >= 0)
        {
            System.Random random = new System.Random(seed);
            int index = random.Next(maxIndex);

            PropUVMap map = sortedMaps[index];

            if(map.GetSqrLengthOfFirstEdge() < maxSizeSqr)
            {
                return map;
            }
            else
            {
                maxIndex = index - 1;
            }
        }
        return null;
    }


}

[Serializable]
public class PropUVMap : IComparable<PropUVMap>
{
    [SerializeField]
    public List<Vector2> uvs = new List<Vector2>();

    public int CompareTo (PropUVMap other)
    {
        return GetSqrLengthOfFirstEdge().CompareTo(other.GetSqrLengthOfFirstEdge());
    }

    public float GetSqrLengthOfFirstEdge ()
    {
        if(uvs == null || uvs.Count < 2)
        {
            return 0;
        }
        return VectorMathsHelper.DistanceSquared(uvs[0], uvs[1]);
    }

    
}
