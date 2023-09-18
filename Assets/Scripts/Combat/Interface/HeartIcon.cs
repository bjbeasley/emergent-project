using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartIcon : MonoBehaviour
{
    [SerializeField]
    private Material fullHeart;
    [SerializeField]
    private Material emptyHeart;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private RawImage image;

    private bool filled = true;

    private void Awake ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetFilled (bool value)
    {
        if(filled == value)
        {
            return;
        }
        filled = value;

        Material mat = filled ? fullHeart : emptyHeart;

        if(meshRenderer != null)
        {
            meshRenderer.material = mat;
        }
        if(image != null)
        {
            image.material = mat;
        }
    }

    public void SetVisisble (bool value)
    {
        meshRenderer.enabled = value;
    }
}
