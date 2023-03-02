using Etienne;
using Etienne.Animator2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PuzzleBobble
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : Singleton<PlayerController>
    {
        public event System.Action<bool> OnJumpChanged;
        public Collider2D Collider => collider;

        [SerializeField] FireBall fireBallPrefab;
        [SerializeField] float ballSpeed = 10f;
        [SerializeField] float speed = 5f;
        [SerializeField] Vector2 gravity = new Vector2(0f, -9.8f);
        [SerializeField] float jumpSpeed = 12f;
        [SerializeField] float jumpDuration = .4f;
        [Header("Audio")]
        [SerializeField] Cue jumpCue;
        [SerializeField] Cue shootCue;

        Animator2D animator;
        Collider2D collider;
        SpriteRenderer renderer;
        Vector2 inputDirection;
        Vector2 collidedPosition;
        bool isGrounded;

        private void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
            animator = GetComponent<Animator2D>();
            collidedPosition = transform.position;
        }

        void OnMove(InputValue value)
        {
            inputDirection.x = value.Get<Vector2>().x;
            if (inputDirection.x != 0) renderer.flipX = inputDirection.x > 0f;
            if (animator.GetState() == "Shoot") return;
            animator.SetState(inputDirection.x != 0 ? "Walk" : "Idle");
        }

        void OnJump(InputValue value)
        {
            if (inputDirection.y != 0) return;
            if (value.isPressed && isGrounded)
            {
                inputDirection.y = 1;
                StartCoroutine(JumpCoroutine(jumpDuration));
                OnJumpChanged?.Invoke(value.isPressed);
            }
        }

        internal void Bump()
        {
            if (!enabled) return;
            inputDirection.y = 1;
            StartCoroutine(JumpCoroutine(jumpDuration * .4f));
        }

        void OnFire(InputValue value)
        {
            if (!value.isPressed) return;
            FireBall fireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.Euler(0, 0, renderer.flipX ? 0 : 180));
            fireBall.SetSpeed(ballSpeed);
            fireBall.SetOwner(this.gameObject);
            animator.SetState("Shoot", true);
            shootCue.Play();
        }

        private IEnumerator JumpCoroutine(float jumpDuration)
        {
            jumpCue.Play();
            animator.SetState("Jump");
            yield return new WaitForSeconds(jumpDuration);
            OnJumpChanged?.Invoke(false);
            inputDirection.y = 0;
            animator.SetState("Idle");
        }
        private void Update()
        {
            var velocity = inputDirection.normalized;
            velocity.x *= speed;
            velocity.y *= jumpSpeed;
            if (animator.GetState() != "Shoot") collidedPosition += (Time.deltaTime * velocity);
            var futureBounds = collider.bounds;
            futureBounds.center = collidedPosition;
            foreach (var item in Physics2DManager.Instance.Colliders)
            {
                if (!Physics2DManager.EtienneIntersects2D(futureBounds, item.bounds)) continue;
                return;
            }
            transform.position = collidedPosition;
        }

        private void FixedUpdate()
        {
            var velocity = inputDirection.normalized;
            velocity.x *= speed;
            velocity.y *= jumpSpeed;
            if (animator.GetState() != "Shoot") collidedPosition += (Time.fixedDeltaTime * velocity);
            if (inputDirection.y <= 0) collidedPosition += gravity * Time.fixedDeltaTime;
            List<Collider2D> collisions = Physics2DManager.GetCollisions(this.collider.bounds);
            collidedPosition += Physics2DManager.CheckCollision(this.collider, collisions);
            transform.position = collidedPosition;

            Vector2 position = collider.bounds.min;
            position.x = transform.position.x;
            foreach (var item in Physics2DManager.Instance.Colliders)
            {
                if (!item.bounds.Contains(position)) continue;
                isGrounded = true;
                return;
            }
            isGrounded = false;
        }

        private void OnDrawGizmos()
        {
            var collider = GetComponent<Collider2D>();
            Vector2 position = collider.bounds.min;
            position.x = transform.position.x;
            Gizmos.DrawSphere(position, .1f);
        }
    }
}
