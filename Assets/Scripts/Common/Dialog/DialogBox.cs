using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class DialogBox : MonoBehaviour
{
    [SerializeField]
    private Transform dialogButtonPrefab;

    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text bodyText;
    [SerializeField]
    private Transform panelTransform;

    [SerializeField]
    private Transform fadeScreenPrefab;

    public void Initialize (string title, string text, params DialogOption[] options)
    {
        titleText.text = title;
        bodyText.text = text;

        for(int i = 0; i < options.Length; i++)
        {
            CreateButton(i, options[options.Length - 1 - i]);
        }
    }

    private void CreateButton (int index, DialogOption option)
    {
        DialogButton button = Instantiate(dialogButtonPrefab, panelTransform).GetComponent<DialogButton>();
        button.Intialize(option, this, index);
    }

    public void Close ()
    {
        Destroy(gameObject);
        Instantiate(fadeScreenPrefab);
    }
}
