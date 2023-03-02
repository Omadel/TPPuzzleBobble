using DG.Tweening;
using Etienne;
using Etienne.Animator2D;
using UnityEngine;

namespace PuzzleBobble
{
    public class Enemy : MonoBehaviour
    {
        public event System.Action OnDie, OnLoose;
        [SerializeField] Cue pickupCue;
        Collider2D collider;
        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            Physics2DManager.Instance.AddCollider(collider);
        }

        internal void Bubble()
        {
            GetComponent<Animator2D>().SetState("Bubble");
            collider.enabled = false;
            transform.DOPath(PathSingleton.Instance.GetClosestPath(transform.position), 3f).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(()=>OnLoose?.Invoke());
        }
        private void Update()
        {
            if (collider.enabled) return;
            if (Physics2DManager.EtienneIntersects2D(collider.bounds, PlayerController.Instance.Collider.bounds))
            {
                Physics2DManager.Remove(collider);
                pickupCue.Play();
                OnDie?.Invoke();
                transform.DOKill();
                Destroy(gameObject);
            }
        }
    }
}
