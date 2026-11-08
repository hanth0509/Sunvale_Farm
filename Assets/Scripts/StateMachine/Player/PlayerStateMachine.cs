
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace StateMachine.Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
        #region Animation
        private int _fallingAnimationHash;
        public int FallingAnimationHash { get { return _fallingAnimationHash; } }
        private int _walkingAnimationHash;
        public int WalkingAnimationHash { get { return _walkingAnimationHash; } }
        private int _runningAnimationHash;
        public int RunningAnimationHash { get { return _runningAnimationHash; } }
        private int _jumpingAnimationHash;
        public int JumpingAnimationHash { get { return _jumpingAnimationHash; } }
        public int ToolAnimationHash { get; private set; }
        public int SleepAnimationHash { get; private set; }
        public int SleepStandAnimationHash { get; private set; }

        public int PourAnimationHash { get; private set; }
        public int WorkAnimationHash { get; private set; }
        public int PlanAnimationHash { get; private set; }
        #endregion



        private InputControls _inputControls;
        private CharacterController _characterController;
        public CharacterController CharacterController { get { return _characterController; } }
        private Animator _animator;
        public Animator Animator { get { return _animator; } }


        #region Move
        [SerializeField] private FixedJoystick _fixedJoystick;
        private Vector2 _keyboardMoveInput;
        private Vector2 _moveInput;
        private Vector3 _currentMovement;
        public float CurrentMovementX
        {
            get { return _currentMovement.x; }
            set { _currentMovement.x = value; }
        }
        public float CurrentMovementY 
        { 
            get { return _currentMovement.y; } 
            set { _currentMovement.y = value; }
        }
        public float CurrentMovementZ
        {
            get { return _currentMovement.z; }
            set { _currentMovement.z = value; }
        }

        private Vector3 _applyMovement;
        public float ApplyMovementX
        {
            get { return _applyMovement.x; }
            set { _applyMovement.x = value; }
        }
        public float ApplyMovementY
        {
            get { return _applyMovement.y; }
            set { _applyMovement.y = value; }
        }
        public float ApplyMovementZ
        {
            get { return _applyMovement.z; }
            set { _applyMovement.z = value; }
        }

        private bool _moveInputPress;
        public bool MoveInputPress { get { return _moveInputPress; } }
        private bool _walkInputPress;
        public bool WalkInputPress { get { return _walkInputPress; } }
        #endregion

        #region Jump
        private bool _jumpInputPress = false;
        public bool JumpInputPress { get { return _jumpInputPress; } }
        private bool _isJumping;
        public bool IsJumping
        {
            get { return _isJumping; }
            set { _isJumping = value; }
        }
        private float _initialJumpVelocity;
        public float InitialJumpVelocity { get { return _initialJumpVelocity; } }

        private float _maxJumpHeight = 1.5f;
        private float _maxJumpTime = 0.8f;
        private bool _requireJumpAgain = false;
        public bool RequireJumpAgain 
        { 
            get { return _requireJumpAgain; } 
            set { _requireJumpAgain = value; }
        }
        #endregion

        private float _rotationFactor = 7.0f;
        private float _runMultiplier = 3.0f;
        public float RunMultiplier { get { return _runMultiplier; } }

        #region Gravity
        /// <summary>
        /// Gravity force pulling down the player
        /// </summary>
        private float _gravity = -9.8f;
        /// <summary>
        /// Gravity force pulling down the player
        /// </summary>
        public float Gravity { get { return _gravity; } }
        /// <summary>
        /// Gravity changes when falling while jumpping
        /// </summary>
        public float JumpGravity { get; private set; }

        ///// <summary>
        ///// Gravity force pulling down the player when grounded
        ///// </summary>
        //private float _groundedGravity = -0.5f;
        ///// <summary>
        ///// Gravity force pulling down the player when grounded
        ///// </summary>
        //public float GroundedGravity { get { return _groundedGravity; } }

        /// <summary>
        /// Multiplier multiply the gravity when falling down
        /// </summary>
        private float _fallMultiplier = 2.0f;
        /// <summary>
        /// Multiplier multiply the gravity when falling down
        /// </summary>
        public float FallMultiplier { get { return _fallMultiplier; } }

        /// <summary>
        /// Maximum falling speed allow for the player
        /// </summary>
        private float _maximumFallingSpeed = 11.5f;
        /// <summary>
        /// Maximum falling speed allow for the player
        /// </summary>
        public float MaximumFallingSpeed { get { return _maximumFallingSpeed; } }
        #endregion

        #region Interaction
        private bool _pickupInputPress;
        /// <summary>
        /// Ket press when player is request to pickup something interactable
        /// </summary>
        public bool PickupInputPress 
        {
            get { return _pickupInputPress; }
            set { _pickupInputPress = value; }
        }
        private bool _lifting = false;
        public bool Lifting
        {
            get { return _lifting; }
            set { _lifting = value; }
        }

        private bool _interactInputPress;
        /// <summary>
        /// Key press when player is request to interact with something
        /// </summary>
        public bool InteractInputPress 
        { 
            get { return _interactInputPress; }
            set { _interactInputPress = value; }
        }
        /// <summary>
        /// Player is using tool
        /// </summary>
        public bool UsingTool { get; set; } = false;
   

        [Header("Events")]
        [Tooltip("Events to fire when tool is using finish")]
        public UnityEvent ToolEvent;
        [Tooltip("Events to fire when interaction is finish")]
        public UnityEvent InteractEvent;
        /// <summary>
        /// Player is picking up/down item
        /// </summary>
        public bool PickingItem { get; set; } = false;
        public bool DroppingItem { get; set; } = false;
        #endregion

        #region State Machine
        /// <summary>
        /// Current state reference
        /// </summary>
        private PlayerBaseState _currentState;
        private PlayerStateFactory _stateFactory;
        /// <summary>
        /// Allow access to state factory to change the state
        /// </summary>
        public PlayerStateFactory StateFactory { get { return _stateFactory; } }
        /// <summary>
        /// Current state of the player
        /// </summary>
        public PlayerBaseState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
        #endregion

        public bool Sleeping { get; set; } = false;
        private bool _pause = false;

        #region Debug Properties
        /// <summary>
        /// Final move vector3
        /// </summary>
        public Vector3 Move { get; private set; }
        public Vector3 CurrentMove { get { return _currentMovement; } }
        #endregion

        public void SwitchState(PlayerBaseState state)
        {
            _currentState.Switch(state);
        }

        // Initialize values
        private void Awake()
        {
            _inputControls = new(); // Initialize input controls (Input asset created by the developer)

            // Subcribe input events to the input controls
            #region Input Subscription
            _inputControls.CharacterControls.Move.started += OnInputMove;
            _inputControls.CharacterControls.Move.canceled += OnInputMove;
            _inputControls.CharacterControls.Move.performed += OnInputMove;

            _inputControls.CharacterControls.Run.started += OnInputRun;
            _inputControls.CharacterControls.Run.canceled += OnInputRun;

            _inputControls.CharacterControls.Jump.started += OnInputJump;
            _inputControls.CharacterControls.Jump.canceled += OnInputJump;

            _inputControls.CharacterControls.Pickup.started += OnItemPickup;
            _inputControls.CharacterControls.Pickup.canceled += OnItemPickup;

            _inputControls.CharacterControls.Interact.started += OnInteract;
            _inputControls.CharacterControls.Interact.canceled += OnInteract;
            #endregion

            // Get component
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            if (_animator == null) _animator = GetComponentInChildren<Animator>();

            // Define animation hash
            #region Animation Hash Values
            _walkingAnimationHash = Animator.StringToHash("Walking");
            _runningAnimationHash = Animator.StringToHash("Running");
            _jumpingAnimationHash = Animator.StringToHash("Jumping");
            _fallingAnimationHash = Animator.StringToHash("Falling");
            ToolAnimationHash = Animator.StringToHash("Tool");
            SleepAnimationHash = Animator.StringToHash("Sleep");
            SleepStandAnimationHash = Animator.StringToHash("SleepStand");
            PourAnimationHash = Animator.StringToHash("Pour");
            WorkAnimationHash = Animator.StringToHash("Work");
            PlanAnimationHash = Animator.StringToHash("Plan");
            #endregion

            InitializeJumpValues(); // Initialize jump variabels

            // Initialize state machine
            _stateFactory = new PlayerStateFactory(this);
            _currentState = _stateFactory.Grounded(); // Initial state is grounded state
            _currentState.Enter();
        }

        private void Start()
        {
            _characterController.Move(_applyMovement * Time.deltaTime);
        }

        private void Update()
        {
            if (_pause) return;
            // Ưu tiên joystick khi đang kéo, dùng bàn phím khi joystick ở giữa.
            Vector2 joystickInput = _fixedJoystick != null
                ? _fixedJoystick.Direction
                : Vector2.zero;

            _moveInput = joystickInput.sqrMagnitude > 0f
                ? joystickInput
                : _keyboardMoveInput;

            _moveInputPress = _moveInput.sqrMagnitude > 0f;
            OnRotate();
            _currentState.UpdateStates(); // Update the current state

            if (!UsingTool && !PickingItem && !DroppingItem && !Sleeping)
                PlayerRotation();
            _characterController.Move(_applyMovement * Time.deltaTime);
        }

        private void OnEnable()
        {
            Enable();
        }

        private void OnDisable()
        {
            Disable();
        }
        public void Disable()
        {
            _inputControls.CharacterControls.Disable(); // Disable when object is not active to not listen to any events
            _pause = true;
            _animator.speed = 0;
        }
        public void Enable()
        {
            _inputControls.CharacterControls.Enable(); // Enable to listen to the events
            _pause = false;
            _animator.speed = 1f;
        }

        private void InitializeJumpValues()
        {
            float timeToApex = _maxJumpTime / 2f;
            JumpGravity = (-2f * _maxJumpHeight) / Mathf.Pow(timeToApex, 2f);
            _initialJumpVelocity = (2f * _maxJumpHeight) / timeToApex;
        }

        private void OnRotate()
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right = right.normalized;

            Vector3 forwardMove = _moveInput.y * forward;
            Vector3 rightMove = _moveInput.x * right;

            Vector3 move = forwardMove + rightMove;

            _currentMovement = new Vector3(move.x, _currentMovement.y, move.z); // Update the current movement
        }

        private void PlayerRotation()
        {
            Vector3 posLookAt; // Look at position

            // Change the position to look at
            posLookAt = new Vector3(_currentMovement.x, 0f, _currentMovement.z);

            Quaternion currentRot = transform.rotation;
            if (_moveInputPress)
            {
                Quaternion targetRot = Quaternion.LookRotation(posLookAt);
                transform.rotation = Quaternion.Slerp(currentRot, targetRot, _rotationFactor * Time.deltaTime);
            }
        }

        #region New Input System callback
        private void OnInputMove(InputAction.CallbackContext context)
        {
            _keyboardMoveInput = context.ReadValue<Vector2>();
        }

        private void OnInputRun(InputAction.CallbackContext context)
        {
            _walkInputPress = context.ReadValueAsButton();
        }

        private void OnInputJump(InputAction.CallbackContext context)
        {
            _jumpInputPress = context.ReadValueAsButton();
            _requireJumpAgain = false; // Make it false when player is pressing the jump button
        }

        private void OnItemPickup(InputAction.CallbackContext context)
        {
            _pickupInputPress = context.ReadValueAsButton();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            _interactInputPress = context.ReadValueAsButton();
        }
        #endregion

        #region Joystick Input

        public void JoyStickJump(bool cd)
        {
            _jumpInputPress = cd;
            _requireJumpAgain = false; // Make it false when player is pressing the jump button
        }

        public void JoyStickPickup(bool cd)
        {
            _pickupInputPress = cd;
        }
        public void JoyStickInteract(bool cd)
        {
            _interactInputPress = cd;
        }
        #endregion
    }
}
