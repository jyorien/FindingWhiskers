using UnityEngine;

/// <summary>
///  This class acts as the player's Finite State Machine and holds all the possible states the player can be in.
///  It is responsible for allowing the states to change between one another.
///  It is also responsible for holding and exposing components and data that the states need.
/// </summary>
public class PlayerStateManager : MonoBehaviour
{
    PlayerBaseState currentPlayerState;
    public PlayerStandingState standingState = new PlayerStandingState();
    public PlayerWalkingState walkingState = new PlayerWalkingState();
    public PlayerJumpingState jumpingState = new PlayerJumpingState();
    public PlayerWallSlidingState wallSlidingState = new PlayerWallSlidingState();
    public PlayerWallJumpingState wallJumpingState = new PlayerWallJumpingState();
    private PlayerFrozenState frozenState = new PlayerFrozenState();

    public Rigidbody2D rigidBody2D { get; private set; }
    public BoxCollider2D boxCollider2D { get; private set; }
    public Animator animator { get; private set; }

    public float horizontalMovement { get; private set; }
    public bool isJumpButtonPressed { get; private set; }
    public bool isFacingRight { get; private set; }

    [SerializeField] public PlayerAttributesDataSO playerAttributes;

    private void Awake()
    {
        playerAttributes.OnFrozenStateChanged.AddListener(SetFrozenState);

    }

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        isFacingRight = true;

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
        playerAttributes.OnFrozenStateChanged.RemoveListener(SetFrozenState);
    }

    public void ChangeState(PlayerBaseState state)
    {
        currentPlayerState.ExitState(this);
        currentPlayerState = state;
        state.EnterState(this);
    }

    private void SetFrozenState(bool isFrozen)
    {
        if (isFrozen)
        {
            ChangeState(frozenState);
        }
    }

    public void FlipHorizontally()
    {
        isFacingRight = !isFacingRight;
        transform.localRotation = isFacingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }


}
