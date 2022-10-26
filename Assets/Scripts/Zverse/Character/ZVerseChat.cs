using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


/// <summary>
/// 用户聊天类
/// </summary>
[Serializable]
public class ChannelInfo
{
    public string command; // /w etc.
    public string identifierOut; // for sending
    public string identifierIn; // for receiving
    public GameObject textPrefab;

    public ChannelInfo(string command, string identifierOut, string identifierIn, GameObject textPrefab)
    {
        this.command = command;
        this.identifierOut = identifierOut;
        this.identifierIn = identifierIn;
        this.textPrefab = textPrefab;
    }
}

[Serializable]
public struct ChatMessage
{
    public string sender;
    public string identifier;
    public string message;
    public string replyPrefix; // copied to input when clicking the message
    public GameObject textPrefab;

    public ChatMessage(string sender, string identifier, string message, string replyPrefix, GameObject textPrefab)
    {
        this.sender = sender;
        this.identifier = identifier;
        this.message = message;
        this.replyPrefix = replyPrefix;
        this.textPrefab = textPrefab;
    }

    // construct the message
    public string Construct()
    {
        return "<b>" + sender + identifier + ":</b> " + message;
    }
}
[DisallowMultipleComponent]
public class ZVerseChat : NetworkBehaviour
{

    [Header("Components")] // to be assigned in inspector
   // public PlayerGuild guild;
   // public PlayerParty party;

    [Header("Channels")]
    public ChannelInfo whisperChannel = new ChannelInfo("/w", "(TO)", "(FROM)", null);
    public ChannelInfo localChannel = new ChannelInfo("", "", "", null);
    public ChannelInfo infoChannel = new ChannelInfo("", "(Info)", "(Info)", null);

    [Header("Other")]
    public int maxLength = 70;

    [Header("Events")]
    public UnityEventString onSubmit;

    public override void OnStartLocalPlayer()
    {
        // test messages
        UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, "Use /w NAME to whisper", "", infoChannel.textPrefab));
        UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, "Or click on a message to reply", "", infoChannel.textPrefab));
        UIChat.singleton.AddMessage(new ChatMessage("Someone", whisperChannel.identifierIn, "Are you there?", "/w Someone ", whisperChannel.textPrefab));
        UIChat.singleton.AddMessage(new ChatMessage("Someone", localChannel.identifierIn, "Hello!", "/w Someone ", localChannel.textPrefab));
    }

    /// <summary>
    /// 客户端点击发送消息
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [Client]
    public string OnSubmit(string text)
    {
        // not empty and not only spaces?
        if (!string.IsNullOrWhiteSpace(text))
        {
            // command in the commands list?
            // note: we don't do 'break' so that one message could potentially
            //       be sent to multiple channels (see mmorpg local chat)
            string lastCommand = "";
            if (text.StartsWith(whisperChannel.command))
            {
                // whisper
                (string user, string message) = ParsePM(whisperChannel.command, text);
                if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(message))
                {
                    if (user != name)
                    {
                        lastCommand = whisperChannel.command + " " + user + " ";
                        CmdMsgWhisper(user, message);
                    }
                    else Debug.Log("cant whisper to self");
                }
                else Debug.Log("invalid whisper format: " + user + "/" + message);
            }
            else if (!text.StartsWith("/"))
            {
                // local chat is special: it has no command
                lastCommand = "";
                CmdMsgLocal(text);
            }


            // addon system hooks
            onSubmit.Invoke(text);

            // input text should be set to lastcommand
            return lastCommand;
        }

        // input text should be cleared
        return "";
    }

    // parse a message of form "/command message"
    internal static string ParseGeneral(string command, string msg)
    {
        // return message without command prefix (if any)
        return msg.StartsWith(command + " ") ? msg.Substring(command.Length + 1) : "";
    }

    // parse a private message
    internal static (string user, string message) ParsePM(string command, string pm)
    {
        // parse to /w content
        string content = ParseGeneral(command, pm);

        // now split the content in "user msg"
        if (content != "")
        {
            // find the first space that separates the name and the message
            int i = content.IndexOf(" ");
            if (i >= 0)
            {
                string user = content.Substring(0, i);
                string msg = content.Substring(i + 1);
                return (user, msg);
            }
        }
        return ("", "");
    }

    /// <summary>
    /// 发送本地消息
    /// </summary>
    /// <param name="message"></param>
    [Command]
    void CmdMsgLocal(string message)
    {
        if (message.Length > maxLength) return;

        // it's local chat, so let's send it to all observers via ClientRpc
        RpcMsgLocal(name, message);
    }


    /// <summary>
    /// 给指定人发送消息
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="message"></param>
    [Command]
    void CmdMsgWhisper(string playerName, string message)
    {
         if (message.Length > maxLength) return;

         if (ZVersePlayer.onlinePlayers.TryGetValue(playerName, out ZVersePlayer onlinePlayer))
         {
             // receiver gets a 'from' message, sender gets a 'to' message
             // (call TargetRpc on that GameObject for that connection)
             onlinePlayer.zverseChat.TargetMsgWhisperFrom(name, message);
             TargetMsgWhisperTo(playerName, message);
         }
    }

    // send a global info message to everyone
    [Server]
    public void SendGlobalMessage(string message)
    {
        foreach (ZVersePlayer player in ZVersePlayer.onlinePlayers.Values)
            player.zverseChat.TargetMsgInfo(message);
    }

    // message handlers ////////////////////////////////////////////////////////
    [TargetRpc]
    public void TargetMsgWhisperFrom(string sender, string message)
    {
        // add message with identifierIn
        string identifier = whisperChannel.identifierIn;
        string reply = whisperChannel.command + " " + sender + " "; // whisper
        UIChat.singleton.AddMessage(new ChatMessage(sender, identifier, message, reply, whisperChannel.textPrefab));
    }

    [TargetRpc]
    public void TargetMsgWhisperTo(string receiver, string message)
    {
        // add message with identifierOut
        string identifier = whisperChannel.identifierOut;
        string reply = whisperChannel.command + " " + receiver + " "; // whisper
        UIChat.singleton.AddMessage(new ChatMessage(receiver, identifier, message, reply, whisperChannel.textPrefab));
    }

    [ClientRpc]
    public void RpcMsgLocal(string sender, string message)
    {
        // add message with identifierIn or Out depending on who sent it
        string identifier = sender != name ? localChannel.identifierIn : localChannel.identifierOut;
        string reply = whisperChannel.command + " " + sender + " "; // whisper
        UIChat.singleton.AddMessage(new ChatMessage(sender, identifier, message, reply, localChannel.textPrefab));
    }


    [TargetRpc]
    public void TargetMsgInfo(string message)
    {
        AddMsgInfo(message);
    }

    // info message can be added from client too
    public void AddMsgInfo(string message)
    {
        UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, message, "", infoChannel.textPrefab));
    }
}
