using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestButton : MonoBehaviour
{
    [SerializeField]
    private Color dangerousColor;
    [SerializeField]
    private Color safeColor;
    [SerializeField]
    private RawImage ringImage;
    [SerializeField]
    private TextMeshProUGUI tmPro;

    private void OnMouseOver ()
    {
        string text = "Rest here for half a day.\n<i>Characters in your party will recover 1 heart each.</i>";

        if(IsDangerous())
        {
            text += "\nWarning: It is dangerous to rest here, you may be attacked.";
        }
        else
        {
            text += "\nIt is safe to rest here for now.";
        }

        ActionTooltip.Instance.Show(text);
    }

    private void LateUpdate ()
    {
        ringImage.color = IsDangerous() ? dangerousColor : safeColor;
        tmPro.color = ringImage.color;

    }

    public void OnClick ()
    {
        Storyteller.Instance.Rest();
    }

    private bool IsDangerous ()
    {
        if(CaravanController.Instance.Province == null)
        {
            return false;
        }
        return !ResourceManager.Instance.IsRestSafe(CaravanController.Instance.Province, false);
    }
}
