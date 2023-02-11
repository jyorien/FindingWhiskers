using UnityEngine;

/// <summary>
///  This class acts as the player's Finite State Machine and holds all the possible states the player can be in.
///  It is responsible for allowing the states to change between one another.
///  It is also responsible for holding and exposing components and data that the states need.
/// </summary>
public class PlayerStateManager : MonoBehaviour
{
    PlayerBaseState currentPlayerState;

    // all possible player states
    public PlayerStandingState standingState = new PlayerStandingState();
    public PlayerWalkingState walkingState = new PlayerWalkingState();
    public PlayerJumpingState jumpingState = new PlayerJumpingState();
    public PlayerWallSlidingState wallSlidingState = new PlayerWallSlidingState();
    public PlayerWallJumpingState wallJumpingState = new PlayerWallJumpingState();
    private PlayerFrozenState frozenState = new PlayerFrozenState();

    // components
    public Rigidbody2D rigidBody2D { get; private set; }
    public BoxCollider2D boxCollider2D { get; private set; }
    public Animator animator { get; private set; }

    // keypress states
    public float horizontalMovement { get; private set; }
    public bool isJumpButtonPressed { get; private set; }

    // keeps track of which direction the player is facing
    public bool isFacingRight { get; private set; }

    public PlayerAttributesDataSO playerAttributes;

    private void Awake()
    {
        // subscribe to when player changes their frozen state
        playerAttributes.OnFrozenStateChanged.AddListener(SetFrozenState);
    }

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();

        // by default, player faces right
        isFacingRight = true;

        // the Finite State Machine's initial state is the Standing State
        currentPlayerState = standingState;
        currentPlayerState.EnterState(this);
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        isJumpButtonPressed = Input.GetButtonDown("Jump");
        currentPlayerState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentPlayerState.FixedUpdateState(this);
    }

    private void OnDestroy()
    {
        // stop subscribing to the event
        playerAttributes.OnFrozenStateChanged.RemoveListener(SetFrozenState);
    }

    /// <summary>
    /// Transitions from the current state to the new state.
    /// ExitState() from the current state and EnterState() from the new state gets called while changing states.
    /// </summary>
    /// <param name="state">The new state to change to.</param>
    public void ChangeState(PlayerBaseState state)
    {
        currentPlayerState.ExitState(this);
        currentPlayerState = state;
        currentPlayerState.EnterState(this);
    }

    /// <summary>
    /// If player is frozen, this method changes the player's current state to the Frozen State.
    /// </summary>
    /// <param name="isFrozen">Determines whether to change player state to frozen.</param>
    private void SetFrozenState(bool isFrozen)
    {
        if (isFrozen)
        {
            ChangeState(frozenState);
        }
    }

    /// <summary>
    /// Change the GameObject transform's y rotation based on which direction the player is facing
    /// </summary>
    public void FlipHorizontally()
    {
        isFacingRight = !isFacingRight;
        transform.localRotation = isFacingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }
}
