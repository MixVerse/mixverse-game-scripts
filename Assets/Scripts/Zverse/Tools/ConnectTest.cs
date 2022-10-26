using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ConnectTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Connect", 1f, 3f);
    }


    private void Connect()
    {
        if(!ZVerseGameManager.FirstOpenApp && NetworkClient.connection==null && !NetworkServer.active )
        {
            ZVerseNetworkManager.Instance.StartClient();
        }
    }
}
