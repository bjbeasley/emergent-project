using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionTooltip : MonoBehaviour
{
    private RectTransform rectTransform;

    public static ActionTooltip Instance { get; private set; }

    private bool visible = true;

    [SerializeField]
    private TMP_Text text;

    private void Awake ()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show (string text)
    {
        gameObject.SetActive(true);
        visible = true;
        this.text.text = text;

        //Vector2 pos = CombatCamera.Instance.GetMousePosition();

        Vector2 mousePos = Input.mousePosition;
        Vector2 pos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);

        rectTransform.anchorMin = pos;
        rectTransform.anchorMax = pos;
        rectTransform.pivot = new Vector2(pos.x > 0.8f ? 1 : 0, pos.y > 0.8f ? 1 : 0);
        rectTransform.anchoredPosition = Vector3.zero;

        

        //rectTransform.anchoredPosition = Input.mousePosition + Vector3.one * 0.1f;// new Vector3(pos.x + .02f, pos.y + .02f, transform.position.z);
        //Canvas.ForceUpdateCanvases();
        this.text.ForceMeshUpdate();
    }

    private void LateUpdate ()
    {
        if(!visible)
        {
            gameObject.SetActive(false);
            return;
        }
        rectTransform.sizeDelta = new Vector2(this.text.textBounds.size.x + 20, this.text.textBounds.size.y + 20);
        visible = false;
    }
}
