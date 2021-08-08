using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Animator))]
public class PlayerCharacter : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float groundAcceleration = 100f;
    public float groundDeceleration = 100f;

    [Range(0f, 1f)] public float airborneAccelProportion;
    [Range(0f, 1f)] public float airborneDecelProportion;
    public float gravity = 50f;
    public float jumpSpeed = 20f;
    public float jumpAbortSpeedReduction = 100f;

    public bool spriteOriginallyFacesLeft = false;

    Vector2 m_InputVector;
    float m_JumpInput;
    Vector2 m_MoveVector;
    CharacterController2D m_CharacterController2D;
    Animator m_Animator;
    CapsuleCollider2D m_Capsule;
    Transform m_Transform;

    const float k_GroundedStickingVelocityMultiplier = 3f;

    void Awake()
    {
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
        m_Capsule = GetComponent<CapsuleCollider2D>();
        m_Transform = transform;
    }

    /// <summary>
    /// This sets the state of the player.
    /// </summary>
    /// <param name="horizontal">The max velocity of the player in y direction. (no delta time)</param>
    /// <param name="jump">Whether the player should jump or not (keep as true for higher jump).</param>
    public void SetState(float horizontal, bool jump)
    {
        m_InputVector.x = horizontal;
        m_InputVector.y = 0f;
        m_JumpInput = jump ? 1f : 0f;
    }

    void Update()
    {
        UpdateFacing();
        UpdateHorizontalMovement();
        UpdateVerticalMovement();
        UpdateJump();
    }

    void UpdateFacing()
    {
        if (!!Mathf.Approximately(m_InputVector.x, 0f))
            return;

        bool faceLeft = m_InputVector.x < 0f;
        bool flip = faceLeft ^ spriteOriginallyFacesLeft;
        transform.rotation = Quaternion.Euler(0f, flip ? 180f : 0f, 0f);
    }

    void UpdateHorizontalMovement()
    {
        bool receivedInput = !Mathf.Approximately(m_InputVector.x, 0f);

        float desiredSpeed = m_InputVector.x * maxSpeed;
        float acceleration = receivedInput ? groundAcceleration : groundDeceleration;

        if (!m_CharacterController2D.IsGrounded)
            acceleration *= receivedInput ? airborneAccelProportion : airborneDecelProportion;

        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);
    }

    void UpdateVerticalMovement()
    {
        float gravityAcc = gravity * Time.deltaTime;

        if (!m_CharacterController2D.IsGrounded)
        {
            bool hitCeiling = m_CharacterController2D.IsCeilinged && m_MoveVector.y > 0f;
            if (Mathf.Approximately(m_MoveVector.y, 0f) || hitCeiling)
                m_MoveVector.y = 0f;
            m_MoveVector.y -= gravityAcc;
        }
        else
        {
            m_MoveVector.y -= gravityAcc;
            m_MoveVector.y = Mathf.Max(m_MoveVector.y, -gravityAcc * k_GroundedStickingVelocityMultiplier);
        }
    }

    void UpdateJump()
    {
        bool receivedInput = !Mathf.Approximately(m_JumpInput, 0f);

        if (m_CharacterController2D.IsGrounded && receivedInput)
            m_MoveVector.y = jumpSpeed;
        else if (!receivedInput && m_MoveVector.y > 0f)
            m_MoveVector.y -= jumpAbortSpeedReduction * Time.deltaTime;
    }

    void FixedUpdate()
    {
        m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
    }
}
