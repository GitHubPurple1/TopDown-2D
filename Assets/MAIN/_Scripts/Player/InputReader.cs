using UnityEngine;
/// <summary>
/// Container class for Player Input reference
/// </summary>
public class InputReader : MonoBehaviour
{
    private IA_Player playerInput;
    #region Unity
    private void Awake()
    {
      playerInput = new IA_Player();
    }
    private void OnEnable()
    {
      playerInput.Player.Enable();
    }
    private void OnDisable()
    {
      playerInput.Player.Disable();
    }
    private void OnDestroy()
    {
      playerInput.Dispose();
    }
    #endregion
    public Vector2 GetMoveInput()
    {
        return playerInput.Player.Movement.ReadValue<Vector2>();
    }
    public Vector2 GetLookInput()
    {
        return playerInput.Player.Look.ReadValue<Vector2>();
    }

    public bool GetShootingState()
    {
        return playerInput.Player.Shoot.IsPressed();
    }

}