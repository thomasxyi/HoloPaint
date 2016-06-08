using System;
using UnityEngine;

public class PreInstaniateHologram : MonoBehaviour {

    public string ReservedUID;

    Vector3 StaringPosition;
    Quaternion StartingRotation;
    Vector3 StaringScale;

	// Use this for initialization
	void Start () {
        StaringPosition = this.transform.localPosition;
        StartingRotation = this.transform.localRotation;
        StaringScale = this.transform.localScale;
        Guid uid = Guid.NewGuid();
        if (ReservedUID != null)
        {
            uid = new Guid(ReservedUID.PadLeft(32, '0'));
            ModelsManager.Instance.ReservedIDs.Add(uid);
            ModelsManager.Instance.ActiveModelsDictionary.Add(uid, this.gameObject);
        }
        else
        {
            // If there's no reserve guid specified just make it a non associated model
            ModelsManager.Instance.ActiveModelsDictionary.Add(uid, this.gameObject);
        }
        GetComponent<TexturePainter>().uid = uid;
    }

    public void ResetToStartingTransform()
    {
        this.transform.localPosition = StaringPosition;
        this.transform.localRotation = StartingRotation;
        this.transform.localScale = StaringScale;
    }
}
