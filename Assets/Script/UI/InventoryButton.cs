using UnityEngine;

public class ButtonActivateObject : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    public void Activate()
    {
        if (targetObject != null)
            targetObject.SetActive(true);
    }

    public void Deactivate()
    {
        if (targetObject != null)
            targetObject.SetActive(false);
    }

    public void Toggle()
    {
        if (targetObject != null)
            targetObject.SetActive(!targetObject.activeSelf);
    }
}