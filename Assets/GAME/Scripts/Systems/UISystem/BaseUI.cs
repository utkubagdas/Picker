using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    [SerializeField] protected RectTransform parentPanel;
    
    public void SetShow()
    {
        parentPanel.gameObject.SetActive(true);
    }

    public void SetHidden()
    {
        parentPanel.gameObject.SetActive(false);
    }
}