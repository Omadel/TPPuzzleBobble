using Etienne;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    [DefaultExecutionOrder(-1)]
    public class Physics2DManager : Etienne.Singleton<Physics2DManager>
    {
        public IEnumerable<Collider2D> Colliders => colliders;

        [SerializeField] float delta = .1f;
        [SerializeField, ReadOnly] List<Collider2D> colliders = new List<Collider2D>();

        private void OnEnable() => transform.hideFlags = HideFlags.NotEditable;
        private void OnDisable() => transform.hideFlags = HideFlags.None;

        public void AddCollider(Collider2D collider) => colliders.Add(collider);

        public static List<Collider2D> GetCollisions(Bounds sourceBounds)
        {
            List<Collider2D> collisions = new List<Collider2D>();
            foreach (var collider in Instance.Colliders)
            {
                if (!collider.isActiveAndEnabled) continue;
                if (sourceBounds == collider.bounds) continue;
                if (!EtienneIntersects2D(sourceBounds, collider.bounds)) continue;
                Debug.Log($"Intersects with {collider}", collider);
                collisions.Add(collider);
            }
            return collisions;
        }
        public static bool EtienneIntersects2D(Bounds a, Bounds b) =>
             a.min.x <= b.max.x &&
             a.max.x >= b.min.x &&
             a.min.y <= b.max.y &&
             a.max.y >= b.min.y;

        public static Vector2 CheckCollision(Collider2D collider, List<Collider2D> collisions)
        {
            Vector2 direction = Vector3.zero;
            foreach (var collision in collisions)
            {
                direction += GetCollisionDirection(collider.bounds, collision.bounds);
            }
            return direction;
        }

        public static Vector2 GetCollisionDirection(Bounds a, Bounds b)
        {
            var distance = CalculateBoundsDistance(a, b);
            var absX = Mathf.Abs(distance.x);
            var absY = Mathf.Abs(distance.y);
            if (absX < absY) return new Vector2(distance.x, 0);
            if (absY < absX) return new Vector2(0, distance.y);
            return Vector3.zero;
        }


        public static Vector2 CalculateBoundsDistance(Bounds a, Bounds b)
        {
            Vector2 distance = Vector2.zero;
            Vector2 direction = a.center.Direction(b.center);
            if (direction.x > 0) distance.x = direction.x - (a.extents.x + b.extents.x) - Instance.delta;
            else distance.x = direction.x + (a.extents.x + b.extents.x) + Instance.delta;
            if (direction.y > 0) distance.y = direction.y - (a.extents.y + b.extents.y) - Instance.delta;
            else distance.y = direction.y + (a.extents.y + b.extents.y) + Instance.delta;
            return distance;
        }
    }
}
