using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    public static PlayerCharacterController Instance;

    public Camera playerCamera;
    public float gravityDownForce = 20f;
    public float maxSpeedOnGround = 8f;
    public float moveSharpnessOnGround = 15f;
    public float rotationSpeed = 200f;

    public float cameraHeightRatio = 0.9f;

    private CharacterController _characterController;
    private PlayerInputHandler _inputHandler;
    private float _targetCharacterHeight = 1.8f;
    private float _cameraVerticalAngle = 0f;
    
    public Vector3 CharacterVelocity { get; set; }

    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
        _characterController = GetComponent<CharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();

        _characterController.enableOverlapRecovery = true;

        UpdateCharacterHeight();
    }

    private void Update()
    {
        HandleCharacterMovement();
    }

    private void UpdateCharacterHeight()
    {
        _characterController.height = _targetCharacterHeight;
        _characterController.center = Vector3.up * _characterController.height * 0.5f;

        playerCamera.transform.localPosition = Vector3.up * _characterController.height * 0.9f;
    }

    // ReSharper restore Unity.ExpensiveCode
    private void HandleCharacterMovement()
    {
        // Camera rotate horizontal
        transform.Rotate(new Vector3(0, _inputHandler.GetMouseLookHorizontal() * rotationSpeed, 0), 
            Space.Self);
        
        // Camera rotate vertical
        _cameraVerticalAngle += _inputHandler.GetMouseLookVertical() * rotationSpeed;

        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

        playerCamera.transform.localEulerAngles = new Vector3(-_cameraVerticalAngle, 0, 0);
        
        // Move 
        Vector3 worldSpaceMoveInput = transform.TransformVector(_inputHandler.GetMoveInput());

        if (_characterController.isGrounded)
        {
            Vector3 targetVelocity = worldSpaceMoveInput * maxSpeedOnGround;

            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                moveSharpnessOnGround * Time.deltaTime);
        }
        else
        {
            CharacterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }

        _characterController.Move(CharacterVelocity * Time.deltaTime);
    }

}
