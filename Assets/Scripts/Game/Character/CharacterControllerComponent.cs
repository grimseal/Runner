using Game.Helper;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Character
{
    [DisallowMultipleComponent, RequireComponent(typeof(Animator)), RequireComponent(typeof(Rigidbody))]
    public class CharacterControllerComponent : MonoBehaviour
    {
        public float speed;

        public Vector3 position
        {
            get => rb.position;
            set => rb.position = value;
        }

        [HideInInspector] public UnityEvent onFailEvent;

        private float horizontalDir;
        private Animator animator;
        private Rigidbody rb;

        private Coroutine jumpCoroutine;

        private enum State
        {
            Run,
            Slide,
            Jump
        }

        private State state;
        private Collider[][] stateCollider;
        [SerializeField] private Collider[] runCollider;
        [SerializeField] private Collider[] slideCollider;
        [SerializeField] private Collider[] jumpCollider;


        /// <summary>
        /// Jump input handler
        /// </summary>
        public void JumpHandler()
        {
            if (state == State.Jump) return;
            SetJumpState();
        }

        /// <summary>
        /// Slide input handler
        /// </summary>
        /// <param name="slide"></param>
        public void SlideHandler(bool slide)
        {
            if (state == State.Jump) return;
            if (slide) SetSlideState();
            else SetRunState();
        }
        
        /// <summary>
        /// Horizontal move input handler
        /// </summary>
        /// <param name="direction"></param>
        public void MoveHandler(float direction)
        {
            horizontalDir = direction;
        }

        /// <summary>
        /// Failure handler. Call it when player collide with obstacle
        /// </summary>
        public void ObstacleCollideHandler()
        {
            // Stop all animations
            if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
            SetRunState();
            animator.enabled = false;
            
            // Call fail event
            onFailEvent?.Invoke();
            
            // Turn on gravity and add force. Baaam!1
            foreach (var defaultCollider in runCollider) defaultCollider.sharedMaterial = null;
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.AddForce(new Vector3(0, 30, -100));

            enabled = false;
        }

        public void SubscribeToInput(PlayerInput playerInput)
        {
            // Sub to input events
            playerInput.onMoveInputEvent.AddListener(MoveHandler);
            playerInput.onJumpInputEvent.AddListener(JumpHandler);
            playerInput.onSlideInputEvent.AddListener(SlideHandler);
        }

        public void UnsubscribeFromInput(PlayerInput playerInput)
        {
            // Unsub from input
            playerInput.onMoveInputEvent.RemoveListener(MoveHandler);
            playerInput.onJumpInputEvent.RemoveListener(JumpHandler);
            playerInput.onSlideInputEvent.RemoveListener(SlideHandler);
        }
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            stateCollider = new[]
            {
                runCollider,
                slideCollider,
                jumpCollider
            };
            SetRunState();
        }

        private void FixedUpdate()
        {
            var x = horizontalDir * HorizontalSpeedMultiplier * speed * 0.33f * Time.fixedDeltaTime;
            rb.velocity = new Vector3(x, 0, speed);
        }

        private void SetRunState()
        {
            state = State.Run;
            animator.SetBool(Slide, false);
            ToggleCollider();
        }
        
        private void SetSlideState()
        {
            if (state == State.Jump || state == State.Slide) return;
            state = State.Slide;
            animator.SetBool(Slide, true);
            ToggleCollider();
        }

        private void SetJumpState()
        {
            if (state == State.Jump) return;
            state = State.Jump;
            animator.Rebind();
            animator.SetBool(Slide, false);
            animator.SetTrigger(Jump);
            ToggleCollider();
            jumpCoroutine = CoroutineHelper.WaitForSeconds(() =>
            {
                jumpCoroutine = null;
                SetRunState();
            }, 0.5f, this);
        }

        private void ToggleCollider()
        {
            var index = (int) state;
            for (var i = 0; i < stateCollider.Length; i++)
            {
                var en = i == index;
                foreach (var col in stateCollider[i])
                    col.enabled = en;
            }
        }

        private const float HorizontalSpeedMultiplier = 100;
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Slide = Animator.StringToHash("Slide");

    }
}