using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private Player player;

    private PlayerController controls;
    public Vector2 moveInput {get; private set;}
    private Vector2 aminInput;
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement Info")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    public Vector3 movement;
    public float speed;
    private float verticalGravity = 0;

    private bool isRunning;

    private void AssignInputEvents() {
        controls = player.controls;
        
        controls.Character.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Character.Movement.canceled += ctx => moveInput = Vector2.zero;

        controls.Character.Run.performed += ctx => {
            speed = runSpeed;
            isRunning = true;
        };
        controls.Character.Run.canceled += ctx => {
            speed = walkSpeed;
            isRunning = false;
        };
    }

    private void Start() {

        player = GetComponent<Player>();

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update() {
        
        // this.transform.position
        ApplyMovement();
        ApplyRotation();
        AnimatorController();
    }

    private void AnimatorController() {
        float xVelocity = Vector3.Dot(movement.normalized, transform.right);
        float zVelocity = Vector3.Dot(movement.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        bool playRunAnimation = isRunning && movement.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
    }

    private void ApplyRotation() {

        Vector3 lookingDirection = player.aim. GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0;
        lookingDirection.Normalize();

        // transform.forward = lookingDirection;

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement() {
        movement = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (movement.magnitude > 0) {
            characterController.Move(movement * Time.deltaTime * speed);
        }
    }

    private void ApplyGravity() {
        if (!characterController.isGrounded) {
            verticalGravity -= 9.81f * Time.deltaTime; // Apply gravity over time            
            movement.y = verticalGravity;
        } else {
            verticalGravity = -0.5f; // Keeps player grounded
        }
    }

}
