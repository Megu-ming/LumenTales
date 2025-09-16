using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager I {  get; private set; }

    [SerializeField] PlayerInput pi;
    [SerializeField] InputActionAsset inputActionAsset;
    public InputAction action;
    private void Awake()
    {
        pi = GetComponent<PlayerInput>();

        action = inputActionAsset.FindAction("Inventory");
    }
}
