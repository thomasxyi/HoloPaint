using UnityEngine;

public class PreInstaniateHologram : MonoBehaviour {

    public string reserveduid;

	// Use this for initialization
	void Start () {
        if (reserveduid != null)
        {
            ModelsManager.Instance.ActiveModelsDictionary.Add(new System.Guid(reserveduid), this.gameObject);
        }
    }
}
