using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropPreviewer : MonoBehaviour
{
    public PropUVPacker propUVPacker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos ()
    {
        if(propUVPacker == null)
        {
            return;
        }
        for(int i = 0; i < propUVPacker.maps.Count; i++)
        {
            PropUVMap propUV = propUVPacker.maps[i];

            for(int j = 0; j < propUV.uvs.Count; j++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(
                    propUV.uvs[j],
                    propUV.uvs[(j + 1) % propUV.uvs.Count]);
                Gizmos.DrawSphere(propUV.uvs[j], 0.01f);
            }
        }
    }
}
