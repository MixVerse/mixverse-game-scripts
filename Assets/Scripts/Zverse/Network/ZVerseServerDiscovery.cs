using System;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using Mirror.Discovery;
using System.Net;

[Serializable]
public class ZVerseServerFoundEvent : UnityEvent<ZVerseFindServerRes> { };

[DisallowMultipleComponent]
[AddComponentMenu("Network/NetworkDiscovery")]
public class ZVerseServerDiscovery : NetworkDiscoveryBase<ZVerseFindServerReq, ZVerseFindServerRes>
{

    #region Server

    public long ServerId { get; private set; }


    [Tooltip("服务器搜索调用协议")]
    public Transport transport;

    [Tooltip("搜索到可用服务器回调")]
    public ZVerseServerFoundEvent OnServerFound;

    public override void Start()
    {
        ServerId = RandomLong();
        if (transport == null)
            transport = Transport.activeTransport;

        base.Start();
    }

    /// <summary>
    /// 客户端请求可用的服务器，并获取服务器信息
    /// </summary>
    /// <param name="request"></param>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    protected override ZVerseFindServerRes ProcessRequest(ZVerseFindServerReq request, IPEndPoint endpoint)
    {

        try
        {

            return new ZVerseFindServerRes
            {
                serverId = ServerId,
                uri = transport.ServerUri()
            };
        }
        catch (NotImplementedException)
        {
            Debug.LogError($"Transport {transport} does not support network discovery");
            throw;
        }
    }



    #endregion

    #region Client

    protected override ZVerseFindServerReq GetRequest() => new ZVerseFindServerReq();



    /// <summary>
    /// 客户端处理服务器返回服务器信息
    /// </summary>
    /// <param name="response"></param>
    /// <param name="endpoint"></param>
    protected override void ProcessResponse(ZVerseFindServerRes response, IPEndPoint endpoint)
    {

        response.EndPoint = endpoint;

        // although we got a supposedly valid url, we may not be able to resolve
        // the provided host
        // However we know the real ip address of the server because we just
        // received a packet from it,  so use that as host.
        UriBuilder realUri = new UriBuilder(response.uri)
        {
            Host = response.EndPoint.Address.ToString()
        };
        response.uri = realUri.Uri;

        OnServerFound.Invoke(response);
    }

    #endregion

}
