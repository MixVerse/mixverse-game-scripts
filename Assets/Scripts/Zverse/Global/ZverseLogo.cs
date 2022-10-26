using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Examples.AdditiveLevels;
using UnityEngine.SceneManagement;

public class ZverseLogo : MonoBehaviour
{

    public List<GameObject> logos;
    public int switchDelta = 2;
    private FadeInOut fade;
    public bool autoPlay = false;
    public string nextScene = "";


    void Awake()
    {
        foreach (var item in logos)
            item.SetActive(false);
    }
    void Start()
    {
        fade = GetComponentInParent<FadeInOut>();
        if (autoPlay) StartCoroutine(ShowLogo());
    }

   
    public  IEnumerator ShowLogo()
    {
        yield return new WaitForSeconds(2f);

        foreach(var item in logos)
        {

            item.SetActive(true);

            yield return fade.FadeOut();

            yield return new WaitForSeconds(switchDelta);

            yield return fade.FadeIn();

            item.SetActive(false);
        }


        LoadScene();

        

    }

    public  void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
}
