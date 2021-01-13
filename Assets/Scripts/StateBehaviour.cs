using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBehaviour : MonoBehaviour
{
    [Header("== PROPERTIES ==")]
    public float walkSpeed = 5f;
    public LayerMask ignoreLayers;

    [Header("== GROUND BELOW ==")]
    public Transform grndCheck;
    public float gDistanceCheck;

    [Header("== SURFACE CHECK ==")]
    public Transform startObj;
    public float sDistanceCheck;

    [Header("== STATES ==")]
    public bool runState;
    public bool onGround;
    public bool surfaceCheck;


    private float _horizontal;
    private Transform _myTransform;
    private Animator _myAnimator;
    public float groundDistance;

    private Vector3 _movVector;

    private void Awake()
    {
        ignoreLayers = ~ignoreLayers;
        _myAnimator = transform.GetChild(0).GetComponent<Animator>();
        _myTransform = GetComponent<Transform>();
    }

    public void Update()
    {
        GetInputs();
        HandleMovement();
        RayGroundCheck();
        ManageStates();
    }

    private void RayGroundCheck()
    {
        Debug.DrawRay(grndCheck.position, -grndCheck.up * gDistanceCheck, Color.red);

        if (Physics.Raycast(grndCheck.position, -grndCheck.up, out RaycastHit hit, gDistanceCheck, ignoreLayers))
        {
            groundDistance = Vector3.Distance(hit.point, transform.position);
            onGround = groundDistance < 0.25f;
        }
        else
        {
            onGround = false;
        }
    }

    private void HandleMovement()
    {
        if (_horizontal != 0)
        {
            transform.rotation = Quaternion.LookRotation(_movVector);
            _myAnimator.SetFloat("InputX", Mathf.Abs(_horizontal));
            transform.Translate(transform.InverseTransformDirection(transform.forward) * walkSpeed * Time.deltaTime);
        }
    }

    private void ManageStates()
    {
        _myAnimator.SetBool("Drop", !onGround);
    }

    private void GetInputs()
    {
        runState = Input.GetKey(KeyCode.LeftShift);

        _horizontal = Input.GetAxis("Horizontal");

        if (!runState)
            _horizontal = Mathf.Clamp(_horizontal, -0.5f, 0.5f);

        _movVector = new Vector3(0f, 0f, _horizontal);
    }

}
