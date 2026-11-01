using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    
    [Header("Camera")]
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    
    private CharacterController _controller;
    private Transform _camPivot;
    private Transform _character;
    
    // input
    private Vector3 _moveDirection = Vector3.zero;
    private bool _looking;

    private Vector2 _lookVector;
    private float characterAngle;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camPivot = transform.Find("CameraPivot");
        _character = transform.Find("Character");
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    
    private  void FixedUpdate()
    {
        
    }

    private void Update()
    {
        _controller.SimpleMove(_character.rotation * _moveDirection);

        UpdateRotation();
    }

    
    private void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();
        _moveDirection.x = move.x * speed;
        _moveDirection.z = move.y * speed;
    }

    private void OnMouseMove(InputValue value)
    {
        _lookVector += value.Get<Vector2>() * cameraSpeed;
        if (_lookVector.y < minAngle) _lookVector.y = minAngle;
        else if (_lookVector.y > maxAngle) _lookVector.y = maxAngle;
        _camPivot.rotation = Quaternion.Euler(-_lookVector.y, _lookVector.x, 0f);
    }


    private void UpdateRotation()
    {
        float delta = _lookVector.x - characterAngle;
        delta %= 360f;
        characterAngle = _lookVector.x - delta;
        
        characterAngle += delta * Time.deltaTime * turnSpeed;
        _character.rotation = Quaternion.Euler(0, characterAngle, 0);
    }
}
