using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ShieldIcon : MonoBehaviour
{
    [SerializeField]
    private Material shieldMaterial;
    [SerializeField]
    private Material tempShieldMaterial;

    private MeshRenderer meshRenderer;

    private bool temp = false;

    private void Awake ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetTemp (bool value)
    {
        temp = value;

        meshRenderer.material = temp ? tempShieldMaterial : shieldMaterial;
    }

    public void SetVisisble (bool value)
    {
        meshRenderer.enabled = value;
    }
}
