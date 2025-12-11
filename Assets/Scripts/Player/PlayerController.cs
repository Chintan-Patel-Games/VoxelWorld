using UnityEngine;
using VoxelWorld.Core.InputSystem;

namespace VoxelWorld.Player
{
    public class PlayerController
    {
        private PlayerModel model;
        private CharacterController controller;
        private Transform cameraTarget;

        // Footsteps
        private bool wasMoving = false;
        private bool wasSprinting = false;

        private float verticalVelocity;
        private float terminalVelocity = 53f;
        private float speed;
        private float rotationVelocity;
        private float cinemachinePitch;

        private bool Grounded;
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;

        private float smoothing = 12f;
        private const float threshold = 0.01f;
        private Vector2 smoothLook;
        private Vector2 currentLook;

        public PlayerController(PlayerModel model, CharacterController controller, Transform cameraTarget)
        {
            this.model = model;
            this.controller = controller;
            this.cameraTarget = cameraTarget;

            jumpTimeoutDelta = model.JumpTimeout;
            fallTimeoutDelta = model.FallTimeout;
        }

        public void TickUpdate(Transform playerTransform)
        {
            GroundedCheck(playerTransform);
            JumpAndGravity();
            Move(playerTransform);
        }

        public void TickLateUpdate(Transform playerTransform) => CameraRotation(playerTransform);

        private void GroundedCheck(Transform playerTransform)
        {
            Vector3 spherePos = new Vector3(playerTransform.position.x,
                                            playerTransform.position.y - model.GroundedOffset,
                                            playerTransform.position.z);

            Grounded = Physics.CheckSphere(spherePos, model.GroundedRadius, model.GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void Move(Transform playerTransform)
        {
            var input = InputService.Instance;

            float targetSpeed = input.Sprint ? model.SprintSpeed : model.MoveSpeed;
            if (input.Move == Vector2.zero) targetSpeed = 0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = input.Move.magnitude;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * model.SpeedChangeRate);
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else speed = targetSpeed;

            Vector3 inputDir = (playerTransform.right * input.Move.x + playerTransform.forward * input.Move.y).normalized;

            controller.Move(inputDir * (speed * Time.deltaTime) + new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);

            // Determine movement state
            float horizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
            bool isMoving = horizontalSpeed > 0.1f;
            bool isSprinting = InputService.Instance.Sprint;

            // Apply footstep logic
            HandleFootsteps(isMoving, isSprinting);
        }

        private void HandleFootsteps(bool isMoving, bool isSprinting)
        {
            // Must be grounded to play footsteps OR If player is not moving -> stop footsteps
            if (!isMoving || !Grounded)
            {
                GlobalSoundService.Instance.SoundService.StopFootsteps();
                wasMoving = false;
                return;
            }

            // If player JUST started moving -> start footsteps
            if (isMoving && !wasMoving)
            {
                GlobalSoundService.Instance.SoundService.StartFootsteps(isSprinting);
            }

            // If player changed from walk -> sprint or sprint -> walk
            if (wasSprinting != isSprinting && wasMoving)
            {
                GlobalSoundService.Instance.SoundService.UpdateFootstepsMode(isSprinting);
            }

            wasMoving = isMoving;
            wasSprinting = isSprinting;
        }

        private void JumpAndGravity()
        {
            var input = InputService.Instance;

            if (Grounded)
            {
                fallTimeoutDelta = model.FallTimeout;

                if (verticalVelocity < 0f)
                    verticalVelocity = -2f;

                if (input.Jump && jumpTimeoutDelta <= 0f)
                {
                    verticalVelocity = Mathf.Sqrt(model.JumpHeight * -2f * model.Gravity);

                    // STOP FOOTSTEPS IMMEDIATELY WHEN JUMPING
                    GlobalSoundService.Instance.SoundService.StopFootsteps();
                    wasMoving = false;
                }

                if (jumpTimeoutDelta >= 0f)
                    jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                jumpTimeoutDelta = model.JumpTimeout;

                if (fallTimeoutDelta >= 0f)
                    fallTimeoutDelta -= Time.deltaTime;

                // STOP FOOTSTEPS WHILE FALLING
                GlobalSoundService.Instance.SoundService.StopFootsteps();
                wasMoving = false;

                input.Jump = false;
            }

            if (verticalVelocity < terminalVelocity)
                verticalVelocity += model.Gravity * Time.deltaTime;
        }

        private void CameraRotation(Transform playerTransform)
        {
            if (InputService.Instance == null) return;

            var input = InputService.Instance;

            Vector2 look = input.Look;

            // Clamp look input if frame stutter occurs
            if (Time.deltaTime > 0.04f) // ~25+ FPS threshold
                look *= (0.04f / Time.deltaTime);

            // Apply smoothing
            look = Vector2.Lerp(currentLook, look, smoothing * Time.deltaTime);
            currentLook = look;

            if (look.sqrMagnitude >= threshold)
            {
                float delta = Time.deltaTime;

                // vertical rotation (camera pitch)
                cinemachinePitch += look.y * model.RotationSpeed * delta;
                cinemachinePitch = Mathf.Clamp(cinemachinePitch, model.BottomClamp, model.TopClamp);

                cameraTarget.localRotation = Quaternion.Euler(cinemachinePitch, 0f, 0f);

                // horizontal rotation (player yaw)
                playerTransform.Rotate(Vector3.up * look.x * model.RotationSpeed * delta);
            }
        }
    }
}