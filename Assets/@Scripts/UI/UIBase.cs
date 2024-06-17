using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public bool IsEnabled { get; private set; } = true;

    public virtual void OpenUI()
    {
        IsEnabled = true;
        gameObject.SetActive(true);
    }

    public virtual void CloseUI()
    {
        gameObject.SetActive(false);
        IsEnabled = false;
    }
}
