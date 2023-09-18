using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class PortraitDemo : MonoBehaviour
{
    [SerializeField]
    private MeshFilter headMeshFilter;
    [SerializeField]
    private MeshFilter eyesMeshFilter;
    [SerializeField]
    private MeshFilter hairMeshFilter;

    [SerializeField]
    private List<PortraitProfile> profiles;
   


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            int index = Random.Range(0, profiles.Count);

            Portrait portrait = profiles[index].GetPortrait(false);

            headMeshFilter.mesh = portrait.GetHeadMesh();
            eyesMeshFilter.mesh = portrait.GetEyeMesh();
            hairMeshFilter.mesh = portrait.GetHairMesh();
        }
    }
}
