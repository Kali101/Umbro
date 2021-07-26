using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string walkAxisName = "Horizontal";
    public string jumpButtonName = "Jump";

    public float walkSpeed = 150f;
    public float jumpHeight = 400f;
    public int maxJumps = 2;

    private Rigidbody2D characterRigidbody;
    private Collider2D characterCollider;
    private int jumpsInProgress = 0;

    private void Awake()
    {
        characterRigidbody = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        var horizontalInput = Input.GetAxis(walkAxisName);
        var xVelocity = walkSpeed * horizontalInput;
        var yVelocity = characterRigidbody.velocity.y;

        if(jumpsInProgress > 0 && CheckCharacterGrounded())
        {
            jumpsInProgress = 0;
        }

        if(jumpsInProgress < maxJumps && Input.GetButtonDown(jumpButtonName))
        {
            yVelocity = jumpHeight;
            StartCoroutine(SetJumpInProgress());
        }

        characterRigidbody.velocity = new Vector2(xVelocity, yVelocity);
    }

    private IEnumerator SetJumpInProgress()
    {
        // Wait until the character has left the ground to count this as a jump
        while(CheckCharacterGrounded())
        {
            yield return new WaitForEndOfFrame();
        }
        jumpsInProgress++;
    }

    bool CheckCharacterGrounded()
    {
        Bounds colliderBounds = characterCollider.bounds;
        var colliderCenter = new Vector2(colliderBounds.center.x, colliderBounds.center.y);
        var colliderSize = new Vector2(colliderBounds.size.x, colliderBounds.size.y);
        
        // Check if player is grounded
        Collider2D[] colliders = Physics2D.OverlapBoxAll(colliderCenter, colliderSize, 0f);

        //Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true
        var isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != characterCollider)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        return isGrounded;
    }
}
