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
    public Color gDebugColor;

    [Header("== SURFACE CHECK ==")]
    public Transform startObj;
    public float sDistanceCheck;
    public Color sDebugColor;

    [Header("== STATES ==")]
    public bool runState = false;
    public bool groundBelow;
    public bool surfaceCheck;

    private float _horizontal;
    private Transform _myTransform;
    private Animator _myAnimator;

    public Vector3 _movVector;

    private void Awake()
    {
        _myAnimator = transform.GetChild(0).GetComponent<Animator>();
        _myTransform = GetComponent<Transform>();
    }

    public void Update()
    {
        GetInputs();
        HandleMovement();
        RayGroundCheck();
        //RaySurfaceCheck();
    }

    private void RayGroundCheck()
    {
        Debug.DrawRay(grndCheck.localPosition, Vector3.up * gDistanceCheck, gDebugColor);
        if (Physics.Raycast(grndCheck.localPosition, - grndCheck.up, out RaycastHit hit, gDistanceCheck, ignoreLayers))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
    }

    private void RaySurfaceCheck()
    {

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

    private void GetInputs()
    {
        runState = Input.GetKey(KeyCode.LeftShift);


        _horizontal = Input.GetAxis("Horizontal");

        if (!runState)
            _horizontal = Mathf.Clamp(_horizontal, -0.5f, 0.5f);

        _movVector = new Vector3(0f, 0f, _horizontal);
    }

}
