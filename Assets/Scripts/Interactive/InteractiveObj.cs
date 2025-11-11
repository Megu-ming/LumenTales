using UnityEngine;

public class InteractiveObj : MonoBehaviour
{
    public virtual void OnInteraction()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}