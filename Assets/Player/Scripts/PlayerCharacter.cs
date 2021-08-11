using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController2D))]
public class PlayerCharacter : MonoBehaviour
{
    public Vector2 crouchCapsuleOffset;
    public Vector2 crouchCapsuleSize;

    public float maxSpeed = 10f;
    public float groundAcceleration = 100f;
    public float groundDeceleration = 100f;

    [Range(0f, 1f)] public float airborneAccelProportion;
    [Range(0f, 1f)] public float airborneDecelProportion;
    public float gravity = 50f;
    public float jumpSpeed = 20f;
    public float jumpAbortSpeedReduction = 100f;

    public bool spriteOriginallyFacesLeft = false;

    public Vector2 Movement { get { return m_MoveVector; } }
    public bool IsCrouched { get { return m_IsCrouched; } }

    float m_HorizontalInput;
    bool m_JumpInput;
    bool m_CrouchInput;
    Vector2 m_MoveVector;
    bool m_IsCrouched;
    CharacterController2D m_CharacterController2D;
    CapsuleCollider2D m_Capsule;
    Vector2 m_CapsuleOffset;
    Vector2 m_CapsuleSize;
    Transform m_Transform;

    const float k_GroundedStickingVelocityMultiplier = 3f;

    void Awake()
    {
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_Capsule = GetComponent<CapsuleCollider2D>();
        m_CapsuleOffset = m_Capsule.offset;
        m_CapsuleSize = m_Capsule.size;
        m_Transform = transform;
    }

    /// <summary>
    /// This sets the state of the player.
    /// </summary>
    /// <param name="horizontal">The direction of movement in x axis (-1f=left, 0f=stop, 1f=right)</param>
    /// <param name="jump">Whether the player should jump or not (keep as true for higher jump)</param>
    /// <param name="crouch">Whether the player should crouch or not (keep as true to stay crouched)</param>
    public void SetState(float horizontal, bool jump, bool crouch)
    {
        m_HorizontalInput = horizontal;
        m_JumpInput = jump;
        m_CrouchInput = crouch;
    }

    void Update()
    {
        UpdateFacing();
        UpdateCrouch();
        UpdateHorizontalMovement();
        UpdateVerticalMovement();
        UpdateJump();
    }

    void UpdateFacing()
    {
        if (!!Mathf.Approximately(m_HorizontalInput, 0f))
            return;

        bool faceLeft = m_HorizontalInput < 0f;
        bool flip = faceLeft ^ spriteOriginallyFacesLeft;
        transform.rotation = Quaternion.Euler(0f, flip ? 180f : 0f, 0f);
    }

    void UpdateCrouch()
    {
        m_IsCrouched = m_CharacterController2D.IsGrounded && m_CrouchInput;
        m_Capsule.offset = !m_IsCrouched ? m_CapsuleOffset : crouchCapsuleOffset;
        m_Capsule.size = !m_IsCrouched ? m_CapsuleSize : crouchCapsuleSize;
    }

    void UpdateHorizontalMovement()
    {
        bool receivedInput = !Mathf.Approximately(m_HorizontalInput, 0f);

        float desiredSpeed = !m_IsCrouched ? m_HorizontalInput * maxSpeed : 0f;
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
        if (m_CharacterController2D.IsGrounded && !m_IsCrouched && m_JumpInput)
            m_MoveVector.y = jumpSpeed;
        else if (!m_JumpInput && m_MoveVector.y > 0f)
            m_MoveVector.y -= jumpAbortSpeedReduction * Time.deltaTime;
    }

    void FixedUpdate()
    {
        m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
    }
}
