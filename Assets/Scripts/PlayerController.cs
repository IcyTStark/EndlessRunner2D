using UnityEngine;

public enum PlayerState
{
    IDLE,
    RUN,
    SLIDE,
    JUMP,
    DEAD
}

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Touch Attributes: ")]
    private Touch _touch;

    [Header("Player Attributes: ")]
    [SerializeField]
    [Range(0f, 1000f)] private float _runSpeed = 10f;
    [SerializeField]
    [Range(0f, 100f)] private float _jumpForce = 10f;

    [Header("Player Control Variables: ")]
    [SerializeField]
    private bool _isGrounded = false;
    [SerializeField]
    private bool _isSliding = false;

    [Header("Component References: ")]
    [SerializeField]
    private Rigidbody2D _rigidbody2D;
    [SerializeField]
    private BoxCollider2D _boxCollider2D;
    [SerializeField]
    private Animator _animator;

    private void Update()
    {
        if (GameManager.Instance.isRunningOnASimulator)
        {
            if (Input.touchCount > 0)
            {
                _touch = Input.GetTouch(0);

                if (_touch.phase == TouchPhase.Began)
                {
                    if (!GameManager.Instance.HasGameStarted)
                    {
                        GameManager.Instance.StartGame();

                        PlayAnimationBasedOnPlayerState(PlayerState.RUN);
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!GameManager.Instance.HasGameStarted)
                {
                    GameManager.Instance.StartGame();

                    PlayAnimationBasedOnPlayerState(PlayerState.RUN);
                }
            }
        }

        // Jump when UpArrow key is pressed
        if (Input.GetKeyDown(KeyCode.UpArrow) && _isGrounded && !_isSliding)
        {
            Jump();
        }

        // Slide when DownArrow key is pressed
        //if (Input.GetKeyDown(KeyCode.DownArrow) && _isGrounded && !_isSliding)
        //{
        //    StartCoroutine(Slide());
        //}
    }


    /// <summary>
    /// Makes the player Jump
    /// </summary>
    void Jump()
    {
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
        _isGrounded = false;
        _animator.SetBool("isJumping", !_isGrounded);
        //PlayAnimationBasedOnPlayerState(PlayerState.JUMP);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.HasGameStarted && !GameManager.Instance.IsGameOver)
        {
            _rigidbody2D.velocity = new Vector2(_runSpeed * Time.fixedDeltaTime, _rigidbody2D.velocity.y);
            _animator.SetFloat("yVelocity", _rigidbody2D.velocity.y);
        }
    }

    //IEnumerator Slide()
    //{
    //    _isSliding = true;
    //    // Adjust player collider and position for sliding here if needed
    //    // Example: transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);

    //    yield return new WaitForSeconds(slideDuration);

    //    _isSliding = false;
    //    // Reset player collider and position after sliding
    //    // Example: transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
    //}

    /// <summary>
    /// Plays the Animation based on Player State
    /// </summary>
    private void PlayAnimationBasedOnPlayerState(PlayerState playerState)
    {
        _animator.SetTrigger(playerState.ToString());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (GameManager.Instance.HasGameStarted)
            {
                _isGrounded = true;
                _animator.SetBool("isJumping", !_isGrounded);
            }
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            PlayAnimationBasedOnPlayerState(PlayerState.DEAD);
            GameManager.Instance.TriggerGameOver();
        }
    }
}
