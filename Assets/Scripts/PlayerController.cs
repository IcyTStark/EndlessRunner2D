using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
    [Header("Touch Attributes:")]
    private Touch _touch;
    private Vector3 _touchStartPosition;
    private Vector3 _touchEndPosition;

    [Header("Player Attributes:")]
    [SerializeField][Range(0f, 1000f)] private float _runSpeed = 10f;
    [SerializeField][Range(0f, 100f)] private float _jumpForce = 10f;
    [SerializeField] private float _slideDuration = 1f;
    [SerializeField] private float _gravityScale;

    [Header("Player Control Variables:")]
    [SerializeField] private bool _isGrounded = false;
    [SerializeField] private bool _isSliding = false;
    [SerializeField] private bool _isPlayerDead = false;

    [Header("Component References:")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private BoxCollider2D _boxCollider2D;
    [SerializeField] private Animator _animator;

    [Header("Raycast Operations: ")]
    private RaycastHit2D _groundHit;

    [Header("VFX References: ")]
    [SerializeField] private GameObject _walkVFX;
    [SerializeField] private ParticleSystem _groundContactVFX;

    [Header("SFX References: ")]
    private GameObject _jumpSFX;

    [Header("HighScore References")]
    [SerializeField] private Vector3 _initialPosition;

    [Header("Shield Properties: ")]
    [SerializeField] private bool _isShielded = false;
    [SerializeField] private float _shieldDuration = 5f;
    [SerializeField] private float _localTimer = 0f;
    [SerializeField] private GameObject _shield;

    [HideInInspector] public UnityEvent OnRetryClicked;

    private void Start()
    {
        if (_rigidbody2D == null) _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_boxCollider2D == null) _boxCollider2D = GetComponent<BoxCollider2D>();
        if (_animator == null) _animator = GetComponent<Animator>();

        _initialPosition = transform.position;

        OnRetryClicked.AddListener(OnRetry);

        _rigidbody2D.gravityScale = _gravityScale;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            HandleInput();

            UIManager.Instance.UpdatePlayerScore(ReturnCurrentScore());

            if(_isShielded)
            {
                _localTimer -= Time.deltaTime;

                if(_localTimer <= 0)
                {
                    _isShielded = false;
                    _shield.gameObject.SetActive(_isShielded);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.HasGameStarted && !_isPlayerDead && !GameManager.Instance.IsGameOver)
        {
            Move();
        }
    }

    /// <summary>
    /// Handles Input based on a Bool which can make input compatible both of game window and simulator window
    /// </summary>
    private void HandleInput()
    {
        if (!GameManager.Instance.IsGameOver && !_isPlayerDead)
        {
            if (GameManager.Instance.isRunningOnASimulator)
            {
                HandleTouchInput();
            }
            else
            {
                HandleKeyboardInput();
            }
        }
    }

    /// <summary>
    /// Handles Player Mobile Input
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began)
            {
                if (!GameManager.Instance.HasGameStarted)
                {
                    StartGame();
                    return;
                }
                else
                {
                    _touchStartPosition = _touch.position;
                }
            }
            else if (_touch.phase == TouchPhase.Moved)
            {
                if (GameManager.Instance.HasGameStarted)
                {
                    _touchEndPosition = _touch.position;

                    float x = _touchEndPosition.x - _touchStartPosition.x;
                    float y = _touchEndPosition.y - _touchStartPosition.y;

                    // Detect vertical movement
                    if (Mathf.Abs(y) > Mathf.Abs(x))
                    {
                        if (y > 0)
                        {
                            // Slide up to jump
                            if (_isGrounded && !_isSliding)
                            {
                                Jump();
                            }
                        }
                        else if (y < 0)
                        {
                            // Slide down to slide
                            if (_isGrounded && !_isSliding)
                            {
                                StartCoroutine(Slide());
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Handles Player PC Input
    /// </summary>
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.HasGameStarted)
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && _isGrounded && !_isSliding)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && _isGrounded && !_isSliding)
        {
            StartCoroutine(Slide());
        }
    }

    /// <summary>
    /// Starts Game if not started yet
    /// </summary>
    private void StartGame()
    {
        GameManager.Instance.StartGame();
        PlayAnimationBasedOnPlayerState(PlayerState.RUN);
        _walkVFX.gameObject.SetActive(true);
    }

    /// <summary>
    /// Moves player forward
    /// </summary>
    private void Move()
    {
        _rigidbody2D.velocity = new Vector2(_runSpeed * Time.fixedDeltaTime, _rigidbody2D.velocity.y);
        _animator.SetFloat("yVelocity", _rigidbody2D.velocity.y);
    }

    /// <summary>
    /// Player Jump Functionality that handles jump and animation
    /// </summary>
    private void Jump()
    {
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
        _isGrounded = false;
        _animator.SetBool("isJumping", true);

        _walkVFX.SetActive(_isGrounded);
    }

    /// <summary>
    /// Slide Functionality
    /// </summary>
    /// <returns></returns>
    private IEnumerator Slide()
    {
        _isSliding = true;

        float boxSizeY = _boxCollider2D.size.y;
        float halfBoxSizeY = _boxCollider2D.size.y / 2;
        float offsetBoxY = halfBoxSizeY / 2;

        _boxCollider2D.size = new Vector2(_boxCollider2D.size.x, halfBoxSizeY);
        _boxCollider2D.offset = new Vector2(_boxCollider2D.offset.x, _boxCollider2D.offset.y - offsetBoxY);
        _animator.SetBool("isSliding", _isSliding);

        yield return new WaitForSeconds(_slideDuration);

        halfBoxSizeY = _boxCollider2D.size.y * 2;

        _isSliding = false;
        _boxCollider2D.offset = new Vector2(_boxCollider2D.offset.x, _boxCollider2D.offset.y + offsetBoxY);
        _boxCollider2D.size = new Vector2(_boxCollider2D.size.x, halfBoxSizeY);
        _animator.SetBool("isSliding", _isSliding);
    }

    /// <summary>
    /// Function Responsible for Shielding Player on Shield Power Pick Up
    /// </summary>
    public void OnShieldPickedUp()
    {
        _localTimer = _shieldDuration;
        _isShielded = true;
        _shield.gameObject.SetActive(_isShielded);
    }

    /// <summary>
    /// Plays the Trigger Animation based on Player State
    /// </summary>
    /// <param name="playerState"></param>
    private void PlayAnimationBasedOnPlayerState(PlayerState playerState)
    {
        _animator.SetTrigger(playerState.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && GameManager.Instance.HasGameStarted)
        {
            if (!_isGrounded)
            {
                _groundContactVFX.Play();
            }

            _isGrounded = true;

            _animator.SetBool("isJumping", false);

            _walkVFX.SetActive(_isGrounded);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!_isShielded)
            {
                _isGrounded = true;
                _isSliding = false;
                _isPlayerDead = true;
                _animator.SetBool("isJumping", false);
                _animator.SetBool("isSliding", _isSliding);
                PlayAnimationBasedOnPlayerState(PlayerState.DEAD);
                _walkVFX.SetActive(false);
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Change Game State and Pass Current Score
    /// </summary>
    private void OnPlayerDead()
    {
        GameManager.Instance.TriggerGameOver(ReturnCurrentScore());
    }

    /// <summary>
    /// Returns Current Score based on the Initial position 
    /// </summary>
    /// <returns></returns>
    private long ReturnCurrentScore()
    {
        float DistanceTravelled = Vector3.Distance(_initialPosition, transform.position);

        long score = (long)DistanceTravelled;

        return score;
    }

    /// <summary>
    /// Resets Player On Game Reset
    /// </summary>
    private void OnRetry()
    {
        PlayAnimationBasedOnPlayerState(PlayerState.IDLE);

        _rigidbody2D.velocity = Vector2.zero;

        transform.position = _initialPosition;

        _isPlayerDead = false;
    }
}
