using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 2f;
    [SerializeField] float idleThreshold = 0.1f;
    Vector2 motionVector = Vector2.zero;
    Animator animator;
    bool isMoving = false;
    public PlayerInputActions playerInput;
    private InputAction move;
    private InputAction interact;

    private InputAction pauseAction;
    public static bool isPaused = false;
    [SerializeField] GameObject pauseMenuObject;

    private void Awake()
    {
        playerInput = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        move = playerInput.Player.Move;
        move.Enable();

        interact = playerInput.Player.Interact;
        interact.Enable();

        pauseAction = playerInput.Player.Pause;
        pauseAction.performed += OnPaused;
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        interact.Disable();
        pauseAction.Disable();
    }

    private void Update()
    {
        motionVector = move.ReadValue<Vector2>();

        animator.SetFloat("horizontal", motionVector.x);
        animator.SetFloat("vertical", motionVector.y);

        isMoving = (motionVector.magnitude > idleThreshold);
        animator.SetBool("IsMoving", isMoving);
    }

    private void FixedUpdate()
    {
        //move
        rb.velocity = motionVector.normalized * speed;
    }

    public void OnPaused(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        pauseMenuObject.SetActive(isPaused);
        
        Time.timeScale = isPaused ? 0 : 1;
    }
}