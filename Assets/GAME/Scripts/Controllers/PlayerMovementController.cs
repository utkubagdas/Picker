using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour
{
    
    private SwerveInputSystem _swerveInputSystem;
    public SwerveInputSystem SwerveInputSystem => _swerveInputSystem == null ? _swerveInputSystem = GetComponent<SwerveInputSystem>() : _swerveInputSystem;

    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody == null ? _rigidbody = GetComponent<Rigidbody>() : _rigidbody;

    [Title("Movement Settings")]
    public bool IsControlable = false;
    public float SwerveSpeed = 0.5f;
    public float MaxSwerveAmount = 1f;
    public float MovementSpeed = 10f;
    [ReadOnly]
    public float SwerveAmount;

    private void OnEnable()
    {
        EventManager.LevelStartEvent.AddListener(() => IsControlable = true);
    }

    private void OnDisable()
    {
        EventManager.LevelStartEvent.RemoveListener(() => IsControlable = true);
    }

    private void Start()
    {
        IsControlable = true;
    }

    private void FixedUpdate()
    {
        Move(CalculateMovementDirection());
    }

    private Vector3 CalculateMovementDirection()
    {
        Vector3 velocity = Vector3.zero;
        var transform1 = transform;
        velocity += transform1.right * SwerveAmount;
        velocity += transform1.forward * 1;

        return velocity;
    }

    private void Move(Vector3 destination)
    {
        if (!IsControlable)
            return;

        SwerveAmount = SwerveInputSystem.MoveFactorX * SwerveSpeed * Time.fixedDeltaTime;
        SwerveAmount = Mathf.Clamp(SwerveAmount, -MaxSwerveAmount, MaxSwerveAmount);

        Rigidbody.velocity = destination * Time.fixedDeltaTime * MovementSpeed;
        //transform.Translate(destination * Time.deltaTime * MovementSpeed);
    }
}
