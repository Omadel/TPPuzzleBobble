using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    public class Wall : MonoBehaviour
    {
        private void Awake()
        {
            Collider2D collider = GetComponent<Collider2D>();
            Physics2DManager.Instance.AddCollider(collider);
        }
    }
}
