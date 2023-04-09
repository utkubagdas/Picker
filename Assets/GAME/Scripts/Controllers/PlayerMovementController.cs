using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour
{
    
    private PlayerFacade _playerFacade;
    public PlayerFacade PlayerFacade => _playerFacade == null ? _playerFacade = GetComponent<PlayerFacade>() : _playerFacade;
    
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
        EventManager.LevelStartEvent.AddListener(LevelStart);
        EventManager.LevelSuccessEvent.AddListener(LevelSuccess);
        EventManager.LevelSuccessEvent.AddListener(LevelFail);
    }

    private void OnDisable()
    {
        EventManager.LevelStartEvent.RemoveListener(LevelStart);
        EventManager.LevelStartEvent.RemoveListener(LevelSuccess);
        EventManager.LevelStartEvent.RemoveListener(LevelFail);
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
        
        
        ClampTheMovement();
        
        SwerveAmount = SwerveInputSystem.MoveFactorX * SwerveSpeed * Time.fixedDeltaTime;
        SwerveAmount = Mathf.Clamp(SwerveAmount, -MaxSwerveAmount, MaxSwerveAmount);

        Rigidbody.velocity = destination * Time.fixedDeltaTime * MovementSpeed;
        //transform.Translate(destination * Time.deltaTime * MovementSpeed);
    }

    public void SetControlable(bool isActive)
    {
        IsControlable = isActive;
        if (!isActive)
        {
            Rigidbody.velocity = Vector3.zero;
        }
    }

    private void ClampTheMovement()
    {
        Vector3 tempPos = transform.position;
        tempPos.x = Mathf.Clamp(tempPos.x, -1.2f, 1.2f);
        transform.position = tempPos;
    }

    private void LevelStart()
    {
        SetControlable(true);
    }

    private void LevelFail()
    {
        SetControlable(false);
    }
    
    private void LevelSuccess()
    {
        SetControlable(false);
    }
}
