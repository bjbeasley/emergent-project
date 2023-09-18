using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private bool combat = false;
    [SerializeField]
    RawImage background;
    [SerializeField]
    TextMeshProUGUI text;

    bool hideText = false;

    float opacity = 1;

    void Start()
    {
        if(!combat && WorldBuilder.Instance != null && WorldBuilder.Instance.Generated)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        background.color = new Color(background.color.r, background.color.g, background.color.b, opacity);
        text.color = new Color(text.color.r, text.color.g, text.color.b, hideText ? 0 : opacity);

        if(WorldBuilder.Instance != null && WorldBuilder.Instance.Generated)
        {
            opacity -= Time.unscaledDeltaTime;            

            if(opacity < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
