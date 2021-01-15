using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentState
{
    Normal, Teeter
}

public class StateBehaviour : MonoBehaviour
{
    [Header("== PROPERTIES ==")]
    public float walkSpeed = 5f;
    public LayerMask ignoreLayers;

    [Header("== GROUND BELOW ==")]
    public Transform gCheckPoint;
    public float grndCheckDistance;

    [Header("== FRONT CHECK ==")]
    public Transform fCheckPoint;
    public float frontCheckDistance;
    
    //[Header("== FRONT CHECK ==")]
    //public Transform distCheckPoint;
    //public float distCheckDistance;


    [Header("== STATES ==")]
    public CurrentState pCurrentState;
    public bool runState;
    public bool walkCarefully;
    public bool onGround;
    public bool ledgeWalking;
    public bool itemInFront;
    public bool surfaceCheck;


    private float _horizontal;
    private Animator _myAnimator;
    private CapsuleCollider _cCollider;
    private Transform _playerModel;
    public float groundDistance;

    private Vector3 _movVector;

    private void Awake()
    {
        pCurrentState = CurrentState.Normal;

        ignoreLayers = ~ignoreLayers;

        _playerModel = transform.GetChild(0);
        _cCollider = _playerModel.GetComponent<CapsuleCollider>();
        _myAnimator = _playerModel.GetComponent<Animator>();
    }

    public void Update()
    {
        GroundCheck();
        FrontItemCheck();

        GetInputs();
        HandleMovement();
        ManageStates();
    }

    private void GroundCheck()
    {
        Debug.DrawRay(gCheckPoint.position, -gCheckPoint.up * grndCheckDistance, Color.red);

        if (Physics.Raycast(gCheckPoint.position, -gCheckPoint.up, out RaycastHit hit, grndCheckDistance, ignoreLayers))
        {
            groundDistance = Vector3.Distance(hit.point, transform.position);
            onGround = true;

            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable item))
            {
                if (item.GetType() == typeof(TeeterPlatform_Prop))
                {
                    ledgeWalking = true;
                }
                else
                {
                    ledgeWalking = false;
                }

                item.Interaction();
            }

            // Change Capsule collider size 
            _cCollider.height = Mathf.Lerp(_cCollider.height, 1.79f, Time.deltaTime * 6f);
        }
        else
        {
            onGround = false;

            // Change Capsule collider size 
            _cCollider.height = Mathf.Lerp(_cCollider.height, 1.3f, Time.deltaTime * 6f);
        }
    }

    private void FrontItemCheck()
    {
        Debug.DrawRay(fCheckPoint.position, fCheckPoint.forward * frontCheckDistance, Color.blue);
        if (Physics.Raycast(fCheckPoint.position, fCheckPoint.forward, out RaycastHit hit, frontCheckDistance, ignoreLayers))
        {
            itemInFront = true;
            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable item))
            {
                item.Interaction();
            }
        }
        else
        {
            itemInFront = false;
        }
    }


    private void GetInputs()
    {
        runState = Input.GetKey(KeyCode.LeftShift);
        _horizontal = Input.GetAxis("Horizontal");
        _movVector = new Vector3(0f, 0f, _horizontal);
    }

    private void HandleMovement()
    {
        if (_horizontal != 0)
        {
            transform.rotation = Quaternion.LookRotation(_movVector);
            _myAnimator.SetFloat("InputX", Mathf.Abs(_horizontal));
            walkSpeed = walkCarefully ? Mathf.Clamp(walkSpeed,0, walkSpeed/2f) : walkSpeed;
            transform.Translate(transform.InverseTransformDirection(transform.forward) * walkSpeed * Time.deltaTime);
        }
    }
    
    private void ManageStates()
    {
        walkCarefully = ledgeWalking;
        _myAnimator.SetBool("Grounded", onGround);
        _myAnimator.SetBool("On Ledge", ledgeWalking);
    }

}