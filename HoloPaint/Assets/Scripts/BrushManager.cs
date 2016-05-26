using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;
using System.Collections.Generic;
using HoloToolkit.Sharing;

public class BrushManager : Singleton<BrushManager>
{
    P3D_Brush LocalBrush;
    float StepSize = 0.001f;

    Dictionary<long, P3D_Brush> UsersBrushDictionary;

    void Start()
    {
        UsersBrushDictionary = new Dictionary<long, P3D_Brush>();
        //LocalBrush.Shape = 
        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.UpdateBrush] = this.OnUpdateBrush;
    }

    public P3D_Brush GetLocalBrush()
    {
        return LocalBrush;
    }

    public P3D_Brush GetGlobalBrush(long userId)
    {
        if (!UsersBrushDictionary.ContainsKey(userId))
        {
            P3D_Brush brush = new P3D_Brush();
            brush.Color = Color.green;
            UsersBrushDictionary.Add(userId, brush);
        }
        return UsersBrushDictionary[userId];
    }

    public float GetStepSize()
    {
        return StepSize;
    }

    public void SetColor(Color c)
    {
        LocalBrush.Color = c;
        UpdateGlobalBrush();
    }

    public void SetSize(Vector2 s)
    {
        LocalBrush.Size = s;
        UpdateGlobalBrush();
    }

    // called by Messages.cs to make sure we have local id
    public void InitializeLocalBrush()
    {
        LocalBrush = new P3D_Brush();
        LocalBrush.Color = Color.red;
        UsersBrushDictionary.Add(Messages.Instance.localUserID, LocalBrush);
        UpdateGlobalBrush();
    }

    public void UpdateGlobalBrush()
    {
        Messages.Instance.SendBrush(LocalBrush);
    }

    void OnUpdateBrush(NetworkInMessage msg)
    {
        long userId = msg.ReadInt64();
        P3D_Brush userBrush = UsersBrushDictionary[userId];

        userBrush.Color = new Color(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        userBrush.Size = new Vector2(msg.ReadFloat(), msg.ReadFloat());

        UsersBrushDictionary.Add(userId, userBrush);
    }
}