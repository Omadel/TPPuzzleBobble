using System.Linq;
using UnityEngine;

namespace Etienne.Animator2D
{
    public class Animation2D : ScriptableObject
    {
        public float Duration => duration;
        public float FrameDuration => 1f / fps;
        public int FPS => fps;
        public Frame[] Frames => frames;
        public TimedAnimationEvent[] TimedAnimationEvents => timedAnimationEvents;

        [SerializeField, ReadOnly] private float duration;
        [SerializeField, Range(1, 90), Delayed] private int fps = 12;
        [SerializeField] private Frame[] frames;
        [SerializeField] private TimedAnimationEvent[] timedAnimationEvents;

        public void SetSprites(Sprite[] sprites)
        {
            frames = new Frame[sprites.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new Frame(sprites[i], 1f);
            }
        }

        private void OnValidate()
        {
            duration = Frames.Select(f => f.Duration).Sum() * FrameDuration;
            if (timedAnimationEvents != null)
            {
                timedAnimationEvents = timedAnimationEvents.OrderBy(e => e.Timing).ToArray();
                foreach (TimedAnimationEvent timedAnimationEvent in timedAnimationEvents)
                {
                    timedAnimationEvent.EventData.ComputeHash();
                }
            }
        }
    }

    [System.Serializable]
    public class TimedAnimationEvent
    {
        public IAnimationEvent.AnimationEventData EventData => eventData;
        public float Timing => timing;
        [SerializeField] private IAnimationEvent.AnimationEventData eventData;
        [SerializeField, Range(0, 1)] private float timing;

        public void SetTiming(float newTiming)
        {
            timing = newTiming;
        }
    }

    [System.Serializable]
    public class Frame
    {
        public Sprite Sprite => sprite;
        public float Duration => duration;

        [SerializeField] private Sprite sprite;
        [SerializeField, Min(.01f)] private float duration;

        private Frame() { duration = 1f; }

        public Frame(Sprite sprite, float duration)
        {
            this.sprite = sprite;
            this.duration = duration;
        }

        public void SetSprite(Sprite sprite) => this.sprite = sprite;
    }
}
