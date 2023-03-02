using DG.Tweening;
using UnityEngine;

namespace PuzzleBobble
{
    public class FireBall : MonoBehaviour
    {
        [SerializeField] float bubbleSpeed = 3f;
        [SerializeField] float lifeSpan = 5f;
        [SerializeField] float size = .5f;
        [SerializeField] float friction = .02f;

        float speed = 5f;
        GameObject owner;
        public void SetSpeed(float speed) => this.speed = speed;

        private void Start()
        {
            Invoke("Kill", lifeSpan);
        }

        void Update()
        {
            if (speed <= bubbleSpeed) return;
            transform.position += speed * Time.deltaTime * transform.right;
            speed -= friction * Time.deltaTime;
            if (speed <= bubbleSpeed) StartFollowPath();
        }

        private void FixedUpdate()
        {
            foreach (var item in Physics2DManager.Instance.Colliders)
            {
                if (!item.enabled) continue;
                Vector3 closestPoint = item.bounds.ClosestPoint(transform.position);
                if (!(Vector3.Distance(closestPoint, transform.position) <= size)) continue;
                Collide(item);
                return;
            }
            if (speed <= bubbleSpeed)
            {
                var item = PlayerController.Instance.Collider;
                Vector3 closestPoint = item.bounds.ClosestPoint(transform.position);
                if (!(Vector3.Distance(closestPoint, transform.position) <= size)) return;
                Collide(item);
            }
        }

        private void Collide(Collider2D item)
        {
            if (item.TryGetComponent(out Enemy enemy))
            {
                enemy.Bubble();
                Kill();
            }
            if (item.TryGetComponent(out PlayerController player))
            {
                player.Bump();
                Kill();
            }
            if (speed > bubbleSpeed)
                StartFollowPath();
        }

        private void StartFollowPath()
        {
            speed = bubbleSpeed;
            transform.DOPath(PathSingleton.Instance.GetClosestPath(transform.position), bubbleSpeed).SetSpeedBased(true).SetEase(Ease.Linear);
        }

        void Kill()
        {
            if (this != null && gameObject != null)
            {
                Destroy(this.gameObject);
            }
        }

        internal void SetOwner(GameObject owner)
        {
            // this.owner = owner;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, size);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
