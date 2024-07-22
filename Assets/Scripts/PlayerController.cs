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
    [Range(0f, 100f)] private float _runSpeed = 10f;
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

    private void Start()
    {

    }

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

        if (GameManager.Instance.HasGameStarted)
        {
            transform.Translate(Vector3.right * _runSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Plays the Animation based on Player State
    /// </summary>
    private void PlayAnimationBasedOnPlayerState(PlayerState playerState)
    {
        _animator.SetTrigger(playerState.ToString());
    }

    /// <summary>
    /// Makes the player Jump
    /// </summary>
    void Jump()
    {
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
        _isGrounded = false;
        PlayAnimationBasedOnPlayerState(PlayerState.JUMP);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is touching the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (GameManager.Instance.HasGameStarted)
            {
                Debug.Log("Ground Touched");
                _isGrounded = true;
                PlayAnimationBasedOnPlayerState(PlayerState.RUN);
            }
        }
    }
}
