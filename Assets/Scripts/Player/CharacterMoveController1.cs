using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Debug = UnityEngine.Debug;

public class CharacterMoveController1 : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float speed;
    public float rotationSpeed;

    [Header("Ground Check")]
    public bool debug;
    public bool isGrounded;
    public float _rayOffset;
    public float _raycastLength;
    public float _heightOffset;
    public float speedToGround = 1;
    public LayerMask _groundLayer;
    private readonly Ray[] _groundDetectRaycasts = new Ray[5];
    private Vector3 averageGroundHeight;


    public float jumpForce;
    public Vector3 moveVector;
    public Quaternion moveRotation;

    private Vector3 targetVector;
    

    [Header("Dependencies")]
    public PlayerInput input;
    public Rigidbody playerRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        CalculateMoveVector();
        CalculateMoveRotation();

    }

    void FixedUpdate()
    {
        //RigidbodyToGround();
        playerRigidbody.linearVelocity = moveVector;
        playerRigidbody.MoveRotation(moveRotation.normalized);
    }

    public void CheckGround()
    {
        _groundDetectRaycasts[0] = new Ray(transform.position + Vector3.up, Vector3.down);
        _groundDetectRaycasts[1] = new Ray(transform.position + Vector3.up + (Vector3.forward * _rayOffset), Vector3.down);
        _groundDetectRaycasts[2] = new Ray(transform.position + Vector3.up - (Vector3.forward * _rayOffset), Vector3.down);
        _groundDetectRaycasts[3] = new Ray(transform.position + Vector3.up + (Vector3.right * _rayOffset), Vector3.down);
        _groundDetectRaycasts[4] = new Ray(transform.position + Vector3.up - (Vector3.right * _rayOffset), Vector3.down);

        int _hitCount = 0;
        averageGroundHeight = Vector3.zero;

        for (int i = 0; i < _groundDetectRaycasts.Length; i++)
        {
            Ray _currentRay = _groundDetectRaycasts[i];

            if (debug)
            {
                Debug.DrawRay(_currentRay.origin, _currentRay.direction * _raycastLength, Color.blue);
            }

            if (Physics.Raycast(_currentRay, out RaycastHit _hit, _raycastLength, _groundLayer))
            {
                averageGroundHeight += _hit.point;
                _hitCount++;
                isGrounded = true;
                
            }
        }
    }

    public void RigidbodyToGround()
    {
        if (isGrounded)
        {
            playerRigidbody.useGravity = false;
            Vector3 _position = playerRigidbody.position;
            _position.y = averageGroundHeight.y;
            _position.y = Mathf.MoveTowards(_position.y, averageGroundHeight.y, speed * Time.deltaTime);
            playerRigidbody.position = _position;
        }
        else
        {
            playerRigidbody.useGravity = true;
        }
    }

    public void CalculateMoveVector()
    {
        float currentSpeed;
        if (input.move == Vector2.zero)
        { currentSpeed = 0f; }
        else { currentSpeed = speed; }

        float currentHorizontalSpeed = new Vector3(playerRigidbody.linearVelocity.x, 0.0f, playerRigidbody.linearVelocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        targetVector.x = input.move.x;
       targetVector.y = playerRigidbody.linearVelocity.y;
        targetVector.z = input.move.y;
        targetVector = targetVector.normalized;
        moveVector = targetVector * currentSpeed * Time.deltaTime;
    }

    public void CalculateMoveRotation()
    {
        Vector3 targetDir = moveVector;

        if (targetDir == Vector3.zero)
        {
            targetDir = transform.forward;
        }

        Quaternion tr = Quaternion.LookRotation(targetDir);
        tr = Quaternion.RotateTowards(transform.rotation, tr, rotationSpeed * Time.deltaTime);
        moveRotation = tr;


    }

    public void Jump()
    {
        if (isGrounded)
        {
            if (input.jump)
            {
                playerRigidbody.AddForce(Vector3.up * jumpForce * Time.deltaTime);
            }
        }
    }
}
