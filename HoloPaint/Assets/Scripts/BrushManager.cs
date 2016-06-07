using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;
using HoloToolkit.Sharing;

public class BrushManager : Singleton<BrushManager>
{
    public P3D_Brush DefaultBrush;
    public P3D_Brush LocalBrush;
    public float StepSize = 0.02f;

    Dictionary<long, P3D_Brush> UsersBrushDictionary = new Dictionary<long, P3D_Brush>();

    void Start()
    {
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
            brush.Shape = DefaultBrush.Shape;
            brush.Size = DefaultBrush.Size;
            UsersBrushDictionary.Add(userId, brush);
        }
        return UsersBrushDictionary[userId];
    }

    public float GetStepSize()
    {
        return System.Math.Min(StepSize * LocalBrush.Size.x, 1.0f);
    }

    public void SetColor(Color c)
    {
        LocalBrush.Color = c;
        UpdateGlobalBrush();
    }

    public void SetSize(Vector2 s)
    {
        //TODO scale cursor by 0.1 every 20 steps
        LocalBrush.Size = s;
        UpdateGlobalBrush();
    }

    // called by Messages.cs to make sure we have local id
    public void InitializeLocalBrush()
    {
        LocalBrush = new P3D_Brush();
        LocalBrush.Color = DefaultBrush.Color;
        LocalBrush.Shape = DefaultBrush.Shape;
        LocalBrush.Size = DefaultBrush.Size;
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
        if (!UsersBrushDictionary.ContainsKey(userId))
        {
            P3D_Brush brush = new P3D_Brush();
            brush.Color = Color.green;
            brush.Shape = DefaultBrush.Shape;
            brush.Size = DefaultBrush.Size;
            UsersBrushDictionary.Add(userId, brush);
        }
        P3D_Brush userBrush = UsersBrushDictionary[userId];

        userBrush.Shape = DefaultBrush.Shape;
        userBrush.Color = new Color(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        userBrush.Size = new Vector2(msg.ReadFloat(), msg.ReadFloat());

        UsersBrushDictionary[userId] = userBrush;
    }
}