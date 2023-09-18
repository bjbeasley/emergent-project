using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField]
    private Transform dialogBoxPrefab;

    private void Awake ()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void CreateDialog (string title, string text, params DialogOption[] options)
    {
        DialogBox box = Instantiate(dialogBoxPrefab, transform).GetComponent<DialogBox>();

        box.Initialize(title, text, options);
    }
}
