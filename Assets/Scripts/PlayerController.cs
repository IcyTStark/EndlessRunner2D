using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Player Attributes:")]
    [SerializeField][Range(0f, 1000f)] private float _runSpeed = 10f;
    [SerializeField][Range(0f, 100f)] private float _jumpForce = 10f;
    [SerializeField] private float _slideDuration = 1f;

    [Header("Player Control Variables:")]
    [SerializeField] private bool _isGrounded = false;
    [SerializeField] private bool _isSliding = false;

    [Header("Component References:")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private BoxCollider2D _boxCollider2D;
    [SerializeField] private Animator _animator;

    private RaycastHit2D _groundHit;

    private void Start()
    {
        if (_rigidbody2D == null) _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_boxCollider2D == null) _boxCollider2D = GetComponent<BoxCollider2D>();
        if (_animator == null) _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.HasGameStarted && !GameManager.Instance.IsGameOver)
        {
            Move();
            //CheckGround();
            AlignPlayerAngleBasedOnSurface();
        }
    }

    private void HandleInput()
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
                }
                else
                {
                    Jump();
                }
            }
        }
    }

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

    private void StartGame()
    {
        GameManager.Instance.StartGame();
        PlayAnimationBasedOnPlayerState(PlayerState.RUN);
    }

    private void Move()
    {
        _rigidbody2D.velocity = new Vector2(_runSpeed * Time.fixedDeltaTime, _rigidbody2D.velocity.y);
        _animator.SetFloat("yVelocity", _rigidbody2D.velocity.y);
    }

    private void Jump()
    {
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
        _isGrounded = false;
        _animator.SetBool("isJumping", true);
    }

    private IEnumerator Slide()
    {
        _isSliding = true;
        // Adjust player collider and position for sliding
        _boxCollider2D.size = new Vector2(_boxCollider2D.size.x, _boxCollider2D.size.y / 2);
        _boxCollider2D.offset = new Vector2(_boxCollider2D.offset.x, _boxCollider2D.offset.y / 2);
        PlayAnimationBasedOnPlayerState(PlayerState.SLIDE);

        yield return new WaitForSeconds(_slideDuration);

        _isSliding = false;
        // Reset player collider and position after sliding
        _boxCollider2D.size = new Vector2(_boxCollider2D.size.x, _boxCollider2D.size.y * 2);
        _boxCollider2D.offset = new Vector2(_boxCollider2D.offset.x, _boxCollider2D.offset.y * 2);
        PlayAnimationBasedOnPlayerState(PlayerState.RUN);
    }

    private void CheckGround()
    {
        _groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        _isGrounded = _groundHit.collider != null;
        _animator.SetBool("isJumping", !_isGrounded);
    }

    private void AlignPlayerAngleBasedOnSurface()
    {
        if (_groundHit.collider != null)
        {
            Vector2 surfaceNormal = _groundHit.normal;
            float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg + 90;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5);
        }
    }

    private void PlayAnimationBasedOnPlayerState(PlayerState playerState)
    {
        _animator.SetTrigger(playerState.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && GameManager.Instance.HasGameStarted)
        {
            _isGrounded = true;
            _animator.SetBool("isJumping", false);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            PlayAnimationBasedOnPlayerState(PlayerState.DEAD);
            GameManager.Instance.TriggerGameOver();
        }
    }
}
