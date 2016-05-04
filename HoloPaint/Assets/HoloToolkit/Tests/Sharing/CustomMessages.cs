﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class CustomMessages : Singleton<CustomMessages>
{
    /// <summary>
    /// Message enum containing our information bytes to share.
    /// The first message type has to start with UserMessageIDStart 
    /// so as not to conflict with HoloToolkit internal messages.  
    /// </summary>
    public enum HoloPaintMessageID : byte
    {
        HeadTransform = MessageID.UserMessageIDStart,
        BoardTransform,
        Texture2D,
        Max
    }

    public enum UserMessageChannels
    {
        Anchors = MessageChannel.UserMessageChannelStart,
    }

    /// <summary>
    /// Cache the local user's ID to use when sending messages
    /// </summary>
    public long localUserID
    {
        get; set;
    }

    public delegate void MessageCallback(NetworkInMessage msg);
    private Dictionary<HoloPaintMessageID, MessageCallback> _MessageHandlers = new Dictionary<HoloPaintMessageID, MessageCallback>();
    public Dictionary<HoloPaintMessageID, MessageCallback> MessageHandlers
    {
        get
        {
            return _MessageHandlers;
        }
    }

    /// <summary>
	/// Helper object that we use to route incoming message callbacks to the member
	/// functions of this class
	/// </summary>
	NetworkConnectionAdapter connectionAdapter;

    /// <summary>
    /// Cache the connection object for the sharing service
    /// </summary>
    NetworkConnection serverConnection;

    void Start()
    {
        InitializeMessageHandlers();
    }

    void InitializeMessageHandlers()
    {
        SharingStage sharingStage = SharingStage.Instance;
        if (sharingStage != null)
        {
            serverConnection = sharingStage.Manager.GetServerConnection();
            connectionAdapter = new NetworkConnectionAdapter();
        }

        connectionAdapter.MessageReceivedCallback += OnMessageReceived;

        // Cache the local user ID
        this.localUserID = SharingStage.Instance.Manager.GetLocalUser().GetID();

        for (byte index = (byte)HoloPaintMessageID.HeadTransform; index < (byte)HoloPaintMessageID.Max; index++)
        {
            if (MessageHandlers.ContainsKey((HoloPaintMessageID)index) == false)
            {
                MessageHandlers.Add((HoloPaintMessageID)index, null);
            }

            serverConnection.AddListener(index, connectionAdapter);
        }
    }

    private NetworkOutMessage CreateMessage(byte MessageType)
    {
        NetworkOutMessage msg = serverConnection.CreateMessage(MessageType);
        msg.Write(MessageType);
        // Add the local userID so that the remote clients know whose message they are receiving
        msg.Write(localUserID);
        return msg;
    }

    public void SendHeadTransform(Vector3 position, Quaternion rotation)
    {
        // If we are connected to a session, broadcast our head info
        if (this.serverConnection != null && this.serverConnection.IsConnected())
        {
            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage msg = CreateMessage((byte)HoloPaintMessageID.HeadTransform);

            AppendTransform(msg, position, rotation);

            // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.  
            this.serverConnection.Broadcast(
                msg,
                MessagePriority.Immediate,
                MessageReliability.UnreliableSequenced,
                MessageChannel.Avatar);
        }
    }

    public void SendBoardTransform(Vector3 position, Quaternion rotation)
    {
        // If we are connected to a session, broadcast our head info
        if (this.serverConnection != null && this.serverConnection.IsConnected())
        {
            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage msg = CreateMessage((byte)HoloPaintMessageID.BoardTransform);

            AppendTransform(msg, position, rotation);

            // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.  
            this.serverConnection.Broadcast(
                msg,
                MessagePriority.Immediate,
                MessageReliability.ReliableOrdered,
                MessageChannel.Avatar);
        }
    }

    public void SendTexture2D(Texture2D tex)
    {
        // If we are connected to a session, broadcast our head info
        if (this.serverConnection != null && this.serverConnection.IsConnected())
        {
            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage msg = CreateMessage((byte)HoloPaintMessageID.Texture2D);

            // Store width and height
            msg.Write(tex.width);
            msg.Write(tex.height);

            // Encode to PNG
            byte[] png = tex.EncodeToPNG();
            msg.Write(png.Length);
            msg.WriteArray(png, (uint)png.Length);

            // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.  
            this.serverConnection.Broadcast(
                msg,
                MessagePriority.Immediate,
                MessageReliability.ReliableOrdered,
                MessageChannel.Avatar);
        }
    }

    void OnDestroy()
    {
        if (this.serverConnection != null)
        {
            for (byte index = (byte)HoloPaintMessageID.HeadTransform; index < (byte)HoloPaintMessageID.Max; index++)
            {
                this.serverConnection.RemoveListener(index, this.connectionAdapter);
            }
            this.connectionAdapter.MessageReceivedCallback -= OnMessageReceived;
        }
    }

    void OnMessageReceived(NetworkConnection connection, NetworkInMessage msg)
    {
        byte messageType = msg.ReadByte();
        MessageCallback messageHandler = MessageHandlers[(HoloPaintMessageID)messageType];
        if (messageHandler != null)
        {
            messageHandler(msg);
        }
    }

    #region HelperFunctionsForWriting
    void AppendTransform(NetworkOutMessage msg, Vector3 position, Quaternion rotation)
    {
        AppendVector3(msg, position);
        AppendQuaternion(msg, rotation);
    }

    void AppendVector3(NetworkOutMessage msg, Vector3 vector)
    {
        msg.Write(vector.x);
        msg.Write(vector.y);
        msg.Write(vector.z);
    }

    void AppendQuaternion(NetworkOutMessage msg, Quaternion rotation)
    {
        msg.Write(rotation.x);
        msg.Write(rotation.y);
        msg.Write(rotation.z);
        msg.Write(rotation.w);
    }
    #endregion

    #region HelperFunctionsForReading 
    public Vector3 ReadVector3(NetworkInMessage msg)
    {
        return new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
    }

    public Quaternion ReadQuaternion(NetworkInMessage msg)
    {
        return new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
    }
    #endregion
}