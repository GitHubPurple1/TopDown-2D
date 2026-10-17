using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputReader))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    private Rigidbody2D rb;
    private Camera cam;
    private InputReader reader;
    private const float MinAimDistanceSqr = 0.1f;

    #region Unity 
  
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        reader = GetComponent<InputReader>();

        cam = Camera.main;

        // Fail Safe For Camera Reference
        if (cam == null)
        {
            Debug.LogError("Main Camera Not Found");
            this.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        UpdatePlayerMovement();
        UpdatePlayerLook();
    }

    #endregion

    private void UpdatePlayerMovement()
    {
        Vector2 moveInput = reader.GetMoveInput();
        Vector2 targetVelocity = Vector2.ClampMagnitude(moveInput, 1f) * moveSpeed; // Clamped so diagonals aren’t faster
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    private void UpdatePlayerLook()
    {
        Vector2 lookInput = reader.GetLookInput();
        Vector3 screenPosition = new Vector3(lookInput.x,lookInput.y,0);
        Vector3 targetPosition = cam.ScreenToWorldPoint(screenPosition);
        Vector2 direction = new Vector2(targetPosition.x,targetPosition.y) -  rb.position ;

        if (direction.sqrMagnitude < MinAimDistanceSqr) return; // Zero distance edge case
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // +X is forward

        rb.MoveRotation(angle);
    }
}