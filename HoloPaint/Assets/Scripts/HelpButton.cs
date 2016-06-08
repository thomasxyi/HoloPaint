using UnityEngine;

public class HelpButton : MonoBehaviour
{
    public void OnSelect()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
        ModeIndicator.Instance.setActive(10.0f, false);
    }
}
