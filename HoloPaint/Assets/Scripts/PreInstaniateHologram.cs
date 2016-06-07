using System;
using UnityEngine;

public class PreInstaniateHologram : MonoBehaviour {

    public string ReservedUID;

	// Use this for initialization
	void Start () {
        if (ReservedUID != null)
        {
            Guid uid = new Guid(ReservedUID);
            ModelsManager.Instance.ReservedIDs.Add(uid);
            ModelsManager.Instance.ActiveModelsDictionary.Add(uid, this.gameObject);
        }
        else
        {
            // If there's no reserve guid specified just make it a non associated model
            ModelsManager.Instance.ActiveModelsDictionary.Add(Guid.NewGuid(), this.gameObject);
        }
    }
}
