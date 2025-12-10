using UnityEngine;
using UnityEngine.InputSystem;
using VoxelWorld.Core.InputSystem;
using VoxelWorld.Core.Events;

public class InputController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private bool isGamePaused = false;

    private void Awake() => _playerInput = GetComponent<PlayerInput>();

    // Called by Unity Input System
    public void OnMove(InputValue value)
    {
        if (!isGamePaused)
            InputService.Instance.Move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if (!isGamePaused)
            InputService.Instance.Look = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!isGamePaused)
            InputService.Instance.Jump = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        if (!isGamePaused)
            InputService.Instance.Sprint = value.isPressed;
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            isGamePaused = !isGamePaused;
            EventService.Instance.OnGamePause.InvokeEvent(isGamePaused);
        }
    }
}