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
    Dictionary<Guid, GameObject> ActiveModelsDictionary;

    // Use this for initialization
    void Start()
    {
        PrefabModels = Resources.LoadAll<GameObject>("Models");
        PrefabModelsDictionary = new Dictionary<string, GameObject>();
        ActiveModelsDictionary = new Dictionary<Guid, GameObject>();
        foreach (GameObject prefabHologram in PrefabModels)
        {
            PrefabModelsDictionary.Add(prefabHologram.name, prefabHologram);
        }
        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.InstantiateModel] = this.OnInstantiateHologram;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnInstantiateHologram(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        string modelName = msg.ReadString();
        string instanceUid = msg.ReadString();

        InstantiateHologram(modelName, new Guid(instanceUid));
    }

    public void InstantiateHologram(string name)
    {
        Guid uid = Guid.NewGuid();
        InstantiateHologram(name, uid);
        Messages.Instance.SendInstantiateModel(name, uid);
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
