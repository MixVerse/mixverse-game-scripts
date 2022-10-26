using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{

    public Slider slider;


    public void  Open()
    {
        gameObject.SetActive(true);
        slider.value = 0;
    }


    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetProgress(float _value)
    {
        slider.value = _value;
    }
}
