using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalNinjaController : MonoBehaviour
{
//Componentes
private CharacterController _controller; 
private Animator _animator;
private Transform _camera;

//Inputs
private float _horizontal;
private float _vertical;
private float _turnSmoothVelocity;
[SerializeField] private float _jumpHeight = 2f;
[SerializeField] private float _speed = 0.5f;
[SerializeField] private float _turnSmoothTime = 0.1f;

//GroundSensor
[SerializeField] private Transform _sensorPosition;
[SerializeField] private float _radius = 0.5f;
[SerializeField] private LayerMask _layer;

//Gravity
[SerializeField] private float _gravity = -9.81f;

[SerializeField] private Vector3 _playerGravity;

//Movimiento
private Vector3 _moveDirection;

void Awake()
{
    _controller = GetComponent<CharacterController>();

    _animator = GetComponentInChildren<Animator>();

    _camera = Camera.main.transform;
}

void Update()
{
    _horizontal = Input.GetAxis("Horizontal");
    _vertical = Input.GetAxis("Vertical");

    if(Input.GetButtonDown("Jump") && IsGrounded())
    {
        Jump();
    }
    Movimiento();
    Gravity();
}

void Movimiento()
{
    Vector3 direction = new Vector3(_horizontal, 0, _vertical);

    _animator.SetFloat("VelZ", direction.magnitude);
    _animator.SetFloat("VelX", 0);

    if (direction != Vector3.zero)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

        _moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        _controller.Move(_moveDirection * _speed * Time.deltaTime);
    }
}

    void Gravity()
{
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _animator.SetBool("IsJumping", false);
            _playerGravity.y = -1;
        }

        _controller.Move(_playerGravity * Time.deltaTime);
}

bool IsGrounded()
{
    return Physics.CheckSphere(_sensorPosition.position, _radius, _layer);
}

void Jump()
{
    _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
    _animator.SetBool("IsJumping", true);
}

void OnDrawGizmos()
{
    Gizmos.color = Color.blue;
    Gizmos.DrawSphere(_sensorPosition.position, _radius);
}

}
