using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovementNew : MonoBehaviour
{
    public float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerControls controls;
    private LayerMask mask;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
        controls.Player.Newaction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Newaction.canceled += _ => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}