using UnityEngine;
using UnityEngine.Events;

namespace Game.Character
{
    public class PlayerInput : MonoBehaviour
    {
        [HideInInspector]
        public MoveEvent onMoveInputEvent = new MoveEvent();
        [HideInInspector]
        public SlideEvent onSlideInputEvent = new SlideEvent();
        [HideInInspector]
        public UnityEvent onJumpInputEvent = new UnityEvent();
        [HideInInspector]
        public UnityEvent onPressAnyKeyEvent = new UnityEvent();

        private float horizontalAxis;

        public void RemoveAllListeners()
        {
            onJumpInputEvent?.RemoveAllListeners();
            onSlideInputEvent?.RemoveAllListeners();
            onMoveInputEvent?.RemoveAllListeners();
            onPressAnyKeyEvent?.RemoveAllListeners();
        }
        
        private void Update () 
        {
            if (Input.anyKey) onPressAnyKeyEvent?.Invoke();

            var horizontalInput = Input.GetAxisRaw(Horizontal);
            if (Mathf.Abs(horizontalInput - horizontalAxis) > AxisTolerance)
            {
                horizontalAxis = horizontalInput;
                onMoveInputEvent.Invoke(horizontalAxis);
            }
            if (Input.GetButtonDown(Jump)) onJumpInputEvent.Invoke();
            if (Input.GetButtonDown(Slide)) onSlideInputEvent.Invoke(true);
            else if (Input.GetButtonUp(Slide)) onSlideInputEvent.Invoke(false);
        }

        private const float AxisTolerance = 0.001f;


        public class MoveEvent : UnityEvent<float> {}

        public class SlideEvent : UnityEvent<bool> {}
        
        private const string Horizontal = "Horizontal";
        private const string Jump = "Jump";
        private const string Slide = "Slide";
    }
}