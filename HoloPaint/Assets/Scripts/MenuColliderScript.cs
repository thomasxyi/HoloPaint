using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class MenuColliderScript : MonoBehaviour
{

    Vector3 pos = Vector3.zero;

    public void OnOpenCommand()
    {
        pos = Camera.main.transform.position + Camera.main.transform.forward * 1.75f;
    }

    void Update()
    {
        if (pos != Vector3.zero)
        {
            if (Vector3.Distance(this.gameObject.transform.position, pos) > 0.001f)
            {
                gameObject.transform.LookAt(gameObject.transform.position * 2 - Camera.main.transform.position);
            }

            this.gameObject.transform.position = Vector3.Lerp(transform.position, pos, 0.2f);
        }

        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement)
        {
            this.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
