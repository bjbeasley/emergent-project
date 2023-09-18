using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class DialogButton : MonoBehaviour
{
    private DialogOption option;
    [SerializeField]
    private TMP_Text text;

    private RectTransform rectTransform;

    private DialogBox dialogBox;

    private void Awake ()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Intialize (DialogOption option, DialogBox dialogBox, int index)
    {
        this.option = option;
        text.text = option.Text;

        Vector2 offset = Vector2.up * 0.1f * index;

        this.dialogBox = dialogBox;

        rectTransform.anchorMin += offset;
        rectTransform.anchorMax += offset;
    }

    public void OnPress ()
    {
        option.Execute();
        dialogBox.Close();
    }
}
