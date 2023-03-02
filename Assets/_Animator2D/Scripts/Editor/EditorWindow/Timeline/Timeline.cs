using Etienne.Animator2D;
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class Timeline : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Timeline> { }

        public event Action<Sprite> OnSelectedSpriteChanged
        {
            add => previewHolder.OnSelectedSpriteChanged += value;
            remove => previewHolder.OnSelectedSpriteChanged -= value;
        }
        public float Value { get => previewHolder.value; set => previewHolder.value = value; }
        public Sprite SelectedSprite => previewHolder.SelectedSprite;

        private SpritePreviewHolder previewHolder;
        private bool isDragging = false;
        private Bar selection;
        private Animation2D currentAnimation;
        private Toolbar eventbar;
        private List<Bar> eventBars = new List<Bar>();
        private Bar selectedEventBar;

        public Timeline()
        {
            Toolbar timebar = new Toolbar();
            Add(timebar);
            eventbar = new Toolbar();
            Add(eventbar);

            previewHolder = new SpritePreviewHolder();
            Add(previewHolder);

            selection = new Bar(Color.white, "Selection");
            selection.SetLengthUnit(LengthUnit.Percent);
            Add(selection);
            previewHolder.OnValueChanged += selection.SetValue;
            SetEnabled(false);
        }


        public override void HandleEvent(EventBase evt)
        {
            if (!enabledSelf) return;
            if (evt is MouseDownEvent downEvent)
            {
                if (downEvent.button == 0)
                {
                    isDragging = true;
                    foreach (Bar bar in eventBars)
                    {
                        float mouseX = downEvent.localMousePosition.x;
                        if (mouseX < bar.layout.xMin || mouseX > bar.layout.xMax)
                        {
                            selectedEventBar = null;
                            continue;
                        }
                        selectedEventBar = bar;
                        break;
                    }

                    if (selectedEventBar != null)
                    {
                        selectedEventBar.SetPixelValue(downEvent.localMousePosition.x);
                    }
                    else
                    {
                        previewHolder.SetPixelValue(downEvent.localMousePosition.x);
                    }
                }
            }
            if (evt is MouseUpEvent upEvent)
            {
                if (upEvent.button == 0) isDragging = false;
            }
            if (evt is MouseMoveEvent moveEvent)
            {
                if (isDragging)
                {
                    if (selectedEventBar != null)
                    {
                        selectedEventBar.SetPixelValue(moveEvent.localMousePosition.x);
                    }
                    else
                    {
                        previewHolder.SetPixelValue(moveEvent.localMousePosition.x);
                    }
                }
            }
            if (evt is MouseLeaveEvent leaveEvent)
            {
                isDragging = false;
            }
        }

        public void SetAnimation(Animation2D animation, float? percentValue)
        {
            SetEnabled(true);
            previewHolder.SetAnimation(animation, percentValue);
            if (percentValue.HasValue) selection.SetValueWithoutNotify(percentValue.Value);
            selectedEventBar = null;
            eventBars.Clear();
            eventbar.Clear();
            foreach (TimedAnimationEvent timedEvent in animation.TimedAnimationEvents)
            {
                Bar bar = new Bar(Color.yellow, timedEvent.EventData.Name, true);
                bar.style.width = 5;
                bar.SetLengthUnit(LengthUnit.Percent);
                bar.SetValue(timedEvent.Timing * 100);
                Label label = new Label(timedEvent.EventData.Name);
                label.StretchToParentSize();
                label.style.left = new StyleLength(new Length(4, LengthUnit.Pixel));
                label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);
                bar.OnValueChanged += v => timedEvent.SetTiming(v * .01f);
                bar.Add(label);
                eventbar.Add(bar);
                eventBars.Add(bar);
            }
        }

        public void FirstFrame()
        {
            if (enabledSelf) previewHolder.FirstFrame();
        }
        public void LastFrame()
        {
            if (enabledSelf) previewHolder.LastFrame();
        }
        public void PreviousFrame()
        {
            if (enabledSelf) previewHolder.PreviousFrame();
        }
        public void NextFrame()
        {
            if (enabledSelf) previewHolder.NextFrame();
        }
        public void TogglePlay()
        {
            throw new NotImplementedException();
        }
    }

}
