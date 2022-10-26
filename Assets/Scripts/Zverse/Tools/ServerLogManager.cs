using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerLogManager : MonoBehaviour
{

    public GameObject networkManager;
    private void Start()
    {
#if UNITY_EDITOR

#else
        //LogManager.Instance.savePath = "Production";
        // LogManager.Register(networkManager, networkManager.name, false, true);
#endif
    }
}
