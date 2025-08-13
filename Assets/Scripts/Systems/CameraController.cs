using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject targetPlayer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer != null) 
        {
            Vector3 pos = new Vector3(targetPlayer.transform.position.x, -0.2f, -10.0f);
            transform.position = pos;
        }
    }
}
