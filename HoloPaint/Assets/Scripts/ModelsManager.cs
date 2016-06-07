using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using System;
using HoloToolkit.Sharing;

public class ModelsManager : Singleton<ModelsManager>
{
    GameObject[] PrefabModels;
    Dictionary<string, GameObject> PrefabModelsDictionary;
    public Dictionary<Guid, GameObject> ActiveModelsDictionary;
    public HashSet<Guid> reservedids;

    // Use this for initialization
    void Start()
    {
        PrefabModels = Resources.LoadAll<GameObject>("Models");
        PrefabModelsDictionary = new Dictionary<string, GameObject>();
        ActiveModelsDictionary = new Dictionary<Guid, GameObject>();
        reservedids = new HashSet<Guid>();
        foreach (GameObject prefabHologram in PrefabModels)
        {
            PrefabModelsDictionary.Add(prefabHologram.name, prefabHologram);
        }
        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.InstantiateModel] = this.OnInstantiateHologram;
        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.PaintUV] = this.OnPaintUV;
        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.ClearPaint] = this.OnClearPaint;

        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.Texture2D] = this.OnTexture2DReceived;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTexture2DReceived(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        int w = msg.ReadInt32();
        int h = msg.ReadInt32();

        uint len = (uint)msg.ReadInt32();
        byte[] data = new byte[len];

        msg.ReadArray(data, len);

        Texture2D tex = new Texture2D(w, h);

        tex.LoadImage(data);

        //SpriteLayer.mainTexture = tex;
    }

    void OnClearPaint(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        string instanceUid = msg.ReadString();
        if (!ActiveModelsDictionary.ContainsKey(new Guid(instanceUid)))
            return;
        ClearPaint(new Guid(instanceUid));
    }

    void OnInstantiateHologram(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        string modelName = msg.ReadString();
        string instanceUid = msg.ReadString();

        InstantiateHologram(modelName, new Guid(instanceUid));
    }

    void OnPaintUV(NetworkInMessage msg)
    {
        long userId = msg.ReadInt64();
        P3D_Brush userBrush = BrushManager.Instance.GetGlobalBrush(userId);

        string instanceUid = msg.ReadString();
        if (!ActiveModelsDictionary.ContainsKey(new Guid(instanceUid)))
            return;
        GameObject model = ActiveModelsDictionary[new Guid(instanceUid)];

        Vector3 uv = Messages.Instance.ReadVector3(msg);

        model.GetComponent<TexturePainter>().PaintUVCoordinates(uv, userBrush);
    }

    public void InstantiateHologram(string name)
    {
        Guid uid = new Guid("0");
        while (reservedids.Contains(uid)) {
            uid = Guid.NewGuid();
        }
        InstantiateHologram(name, uid);
        Messages.Instance.SendInstantiateModel(name, uid);
    }

    public void ClearAllPaint()
    {
        foreach (Guid uid in ActiveModelsDictionary.Keys)
        {
            ClearPaint(uid);
            Messages.Instance.SendClearPaint(uid);
        }
    }

    public void ClearPaint(Guid uid)
    {
        GameObject model = ActiveModelsDictionary[uid];
        model.GetComponent<TexturePainter>().ClearPaint();
        // DO NOT SEND CLEAR MESSAGE HERE, WILL CAUSE FEEDBACK LOOP
    }

    void InstantiateHologram(string name, Guid uid)
    {
        // Instantiate object to be in front on the menu
        GameObject menu = GameObject.FindGameObjectWithTag("Menu");
        Vector3 spawnPos = menu.transform.TransformPoint(Vector3.back / 2);
        GameObject hologram = Instantiate(PrefabModelsDictionary[name], spawnPos, Quaternion.identity) as GameObject;

        hologram.transform.SetParent(this.transform, true);

        // Save the uid
        hologram.GetComponent<TexturePainter>().uid = uid;
        ActiveModelsDictionary.Add(uid, hologram);

        hologram.SetActive(true);
    }
}
