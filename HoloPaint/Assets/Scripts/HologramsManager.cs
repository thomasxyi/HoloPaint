using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class HologramsManager : Singleton<HologramsManager> {
    public GameObject[] PrefabHolograms;
    public Dictionary<string, GameObject> PrefabHologramsDictionary;
    public GameObject ActiveHologramCollection;

    // Use this for initialization
    void Start ()
    {
	    if (PrefabHolograms == null)
        {
            PrefabHolograms = GameObject.FindGameObjectsWithTag("PrefabHolograms");
        }
        PrefabHologramsDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject prefabHologram in PrefabHolograms)
        {
            PrefabHologramsDictionary.Add(prefabHologram.name, prefabHologram);
        }
        ActiveHologramCollection = GameObject.FindGameObjectWithTag("ActiveHologramCollection");
	}

    public void instantiateHologram(string name)
    {
        GameObject hologram = Instantiate(PrefabHologramsDictionary[name]);
        hologram.transform.SetParent(ActiveHologramCollection.transform);
        hologram.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
