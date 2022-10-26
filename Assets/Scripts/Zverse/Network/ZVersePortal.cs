using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text.RegularExpressions;
using Mirror.Examples.AdditiveLevels;


/// <summary>
/// 传送门
/// </summary>
public class ZVersePortal : NetworkBehaviour
{
    [Scene, Tooltip("传送到哪个场景")]
    public string destinationScene;

    [Tooltip("Where to spawn player in Destination Scene")]
    public Vector3 startPosition;

    [Tooltip("Reference to child TMP label")]
    public TMPro.TextMeshPro label;

    [SyncVar(hook = nameof(OnLabelTextChanged))]
    public string labelText;


    // This is approximately the fade time
    WaitForSeconds waitForFade = new WaitForSeconds(2f);

    #region Server相关-------------------------------------------------------------------------------------------------------
    public override void OnStartServer()
    {
        labelText = Path.GetFileNameWithoutExtension(destinationScene);

        // Simple Regex to insert spaces before capitals, numbers
        labelText = Regex.Replace(labelText, @"\B[A-Z0-9]+", " $0");
    }
    #endregion


    #region Client相关-------------------------------------------------------------------------------------------------------
    public void OnLabelTextChanged(string _, string newValue)
    {
        label.text = labelText;
    }
    #endregion

    /// <summary>
    /// 这个客户端和服务器都会触发
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        // tag check in case you didn't set up the layers and matrix as noted above
        if (!other.CompareTag("Player")) return;

        //Debug.Log($"{System.DateTime.Now:HH:mm:ss:fff} Portal::OnTriggerEnter {gameObject.name} in {gameObject.scene.name}");

        // applies to host client on server and remote clients
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
            playerController.enabled = false;

        if (isServer)
            StartCoroutine(SendPlayerToNewScene(other.gameObject));
    }

    [ServerCallback]
    IEnumerator SendPlayerToNewScene(GameObject player)
    {
        if (player.TryGetComponent<NetworkIdentity>(out NetworkIdentity identity))
        {
            NetworkConnectionToClient conn = identity.connectionToClient;
            if (conn == null) yield break;

            // Tell client to unload previous subscene. No custom handling for this.
            conn.Send(new SceneMessage { sceneName = gameObject.scene.path, sceneOperation = SceneOperation.UnloadAdditive, customHandling = true });

            yield return waitForFade;

            //Debug.Log($"SendPlayerToNewScene RemovePlayerForConnection {conn} netId:{conn.identity.netId}");
            NetworkServer.RemovePlayerForConnection(conn, false);

            // reposition player on server and client
            player.transform.position = startPosition;
            //player.transform.LookAt(Vector3.up);

            // Move player to new subscene.
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByPath(destinationScene));

            // Tell client to load the new subscene with custom handling (see NetworkManager::OnClientChangeScene).
            conn.Send(new SceneMessage { sceneName = destinationScene, sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

            //Debug.Log($"SendPlayerToNewScene AddPlayerForConnection {conn} netId:{conn.identity.netId}");
            NetworkServer.AddPlayerForConnection(conn, player);

            // host client would have been disabled by OnTriggerEnter above
            if (NetworkClient.localPlayer != null && NetworkClient.localPlayer.TryGetComponent<PlayerController>(out PlayerController playerController))
                playerController.enabled = true;
        }
    }

}
