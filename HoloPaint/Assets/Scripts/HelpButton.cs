using UnityEngine;

public class HelpButton : MonoBehaviour
{
    public void OnSelect()
    {
        ModeIndicator.Instance.setActive(10.0f, false);
    }
}
