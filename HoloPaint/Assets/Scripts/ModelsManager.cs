using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class ModelsManager : Singleton<ModelsManager>
{
    GameObject[] PrefabModels;
    Dictionary<string, GameObject> PrefabModelsDictionary;

    // Use this for initialization
    void Start()
    {
        PrefabModels = Resources.LoadAll<GameObject>("Models");
        PrefabModelsDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject prefabHologram in PrefabModels)
        {
            PrefabModelsDictionary.Add(prefabHologram.name, prefabHologram);
        }
        instantiateHologram("Whale");
    }

    public void instantiateHologram(string name)
    {
        GameObject hologram = Instantiate(PrefabModelsDictionary[name]) as GameObject;
        hologram.transform.SetParent(this.transform);
        hologram.transform.localPosition = Vector3.zero; // Should do it in the prefab as well
        hologram.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
