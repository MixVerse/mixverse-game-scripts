using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    public static UIPopup Instance;
    public GameObject panel;
    public Text messageText;
    public Text titleText;

    void Awake()
    {
        Instance = this;
    }

    public void Show(string message,string title="")
    {

        if (!string.IsNullOrEmpty(title))
            titleText.text = title;
        messageText.text = message;
       // if (panel.activeSelf) messageText.text += ";\n" + message;
       // else messageText.text = message;
        panel.SetActive(true);
    }

}
