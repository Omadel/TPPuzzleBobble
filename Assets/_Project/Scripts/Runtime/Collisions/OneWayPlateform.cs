using UnityEngine;

namespace PuzzleBobble
{
    public class OneWayPlateform : MonoBehaviour
    {
        Collider2D collider;
        void Awake()
        {
            collider = GetComponent<Collider2D>();
            Physics2DManager.Instance.AddCollider(collider);
            PlayerController.Instance.OnJumpChanged += JumpChanged;
        }

        private void JumpChanged(bool obj)
        {
            collider.enabled = !obj;
        }
    }
}
