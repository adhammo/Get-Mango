using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CharacterController2D : MonoBehaviour
{
    [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
    public LayerMask groundedLayerMask;
    [Tooltip("The distance down to check for ground.")]
    public float groundedRaycastDistance = 0.1f;

    Rigidbody2D m_Rigidbody2D;
    CapsuleCollider2D m_Capsule;
    ContactFilter2D m_ContactFilter;
    Vector2 m_PreviousPosition;
    Vector2 m_CurrentPosition;
    Vector2 m_NextMovement;
    Vector2[] m_RaycastPositions = new Vector2[3];
    RaycastHit2D[] m_HitBuffer = new RaycastHit2D[5];

    public Rigidbody2D Rigidbody2D { get { return m_Rigidbody2D; } }
    public ContactFilter2D ContactFilter { get { return m_ContactFilter; } }
    public bool IsGrounded { get; protected set; }
    public bool IsCeilinged { get; protected set; }
    public Vector2 Velocity { get; protected set; }

    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Capsule = GetComponent<CapsuleCollider2D>();

        m_ContactFilter.layerMask = groundedLayerMask;
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = false;

        m_CurrentPosition = m_Rigidbody2D.position;
        m_PreviousPosition = m_Rigidbody2D.position;

        Physics2D.queriesStartInColliders = false;
    }

    void FixedUpdate()
    {
        m_PreviousPosition = m_Rigidbody2D.position;
        m_CurrentPosition = m_PreviousPosition + m_NextMovement;
        m_Rigidbody2D.MovePosition(m_CurrentPosition);

        Velocity = m_NextMovement / Time.deltaTime;
        m_NextMovement = Vector2.zero;

        CheckCapsuleEndCollisions();
        CheckCapsuleEndCollisions(false);
    }

    /// <summary>
    /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
    /// </summary>
    /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
    public void Move(Vector2 movement)
    {
        m_NextMovement += movement;
    }

    /// <summary>
    /// This moves the character without any implied velocity.
    /// </summary>
    /// <param name="position">The new position of the character in global space.</param>
    public void Teleport(Vector2 position)
    {
        Vector2 delta = position - m_CurrentPosition;
        m_PreviousPosition += delta;
        m_CurrentPosition = position;
        m_Rigidbody2D.MovePosition(position);
    }

    void CheckCapsuleEndCollisions(bool bottom = true)
    {
        Vector2 raycastDirection = bottom ? Vector2.down : Vector2.up;

        Vector2 raycastStartDelta = raycastDirection * (m_Capsule.size.y * 0.5f - m_Capsule.size.x * 0.5f);

        Vector2 raycastStart = m_Rigidbody2D.position + m_Capsule.offset + raycastStartDelta;
        float raycastDistance = m_Capsule.size.x * 0.5f + groundedRaycastDistance * 2f;

        m_RaycastPositions[0] = raycastStart + Vector2.left * m_Capsule.size.x * 0.5f;
        m_RaycastPositions[1] = raycastStart;
        m_RaycastPositions[2] = raycastStart + Vector2.right * m_Capsule.size.x * 0.5f;

        bool blocked = false;
        for (int i = 0; i < m_RaycastPositions.Length; i++)
        {
            int count = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);

            for (int j = 0; j < count; j++)
            {
                if (m_HitBuffer[j].collider != null)
                {
                    blocked = true;
                    break;
                }
            }
        }

        if (bottom) IsGrounded = blocked;
        else IsCeilinged = blocked;
    }
}
