using Etienne;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PuzzleBobble
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : Singleton<PlayerController>
    {
        public event System.Action<bool> OnJumpChanged;

        [SerializeField] float speed = 5f;
        [SerializeField] Vector2 gravity = new Vector2(0f, -9.8f);
        [SerializeField] Collider2D targetCollider;

        Collider2D collider;
        SpriteRenderer renderer;
        Vector2 inputDirection;
        Vector2 velocity;

        private void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
        }

        void OnMove(InputValue value)
        {
            inputDirection.x = value.Get<Vector2>().x;
            renderer.flipX = inputDirection.x > 0f;
        }

        void OnJump(InputValue value)
        {
            inputDirection.y = value.isPressed ? 1 : 0;
            OnJumpChanged?.Invoke(value.isPressed);
        }

        private void FixedUpdate()
        {
            transform.position += (speed * Time.fixedDeltaTime * (Vector3)inputDirection.normalized);
            if (inputDirection.y <= 0) transform.position += (Vector3)gravity * Time.fixedDeltaTime;
            List<Collider2D> collisions = Physics2DManager.GetCollisions(this.collider.bounds);
            transform.position += (Vector3)Physics2DManager.CheckCollision(this.collider, collisions);
        }
    }
}
