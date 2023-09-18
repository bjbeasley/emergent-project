using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCulling : MonoBehaviour
{

    public float zoomCutoff = 2;
    
    public string materialPropertyName = "_Cutoff";
    private int materialPropertyID;

    private MeshRenderer meshRenderer;

    float materialValue = 1;

    public float drawSpeed = 1;
    public float eraseSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materialPropertyID = Shader.PropertyToID(materialPropertyName);
    }

    // Update is called once per frame
    void Update()
    {
        if(CameraZoom.Instance.zoom < zoomCutoff)
        {
            materialValue += Time.deltaTime * eraseSpeed;
        }
        else
        {
            materialValue -= Time.deltaTime * drawSpeed;
        }

        materialValue = Mathf.Clamp01(materialValue);

        meshRenderer.enabled = materialValue < 1;

        float value = 1 - materialValue;
        value *= value;
        value = 1 - value;


        meshRenderer.sharedMaterial.SetFloat(materialPropertyID, value);
    }
}
