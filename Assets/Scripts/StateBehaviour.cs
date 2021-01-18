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
    public float jumpVerticalDist = 4f;
    public float jumpFrwdDist = 4f;

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
    public bool jumping;


    private float _horizontal;
    private Animator _myAnimator;
    private Rigidbody _myRigidbody;
    private CapsuleCollider _cCollider;
    private Transform _playerModel;
    public float groundDistance;

    private Vector3 _movVector;

    private void Awake()
    {
        pCurrentState = CurrentState.Normal;

        ignoreLayers = ~ignoreLayers;

        _myRigidbody = GetComponent<Rigidbody>();
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
        Debug.DrawRay(gCheckPoint.position, -gCheckPoint.up * grndCheckDistance, Color.red); // for debug

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

                item.Interaction();
            }
            else
            {
                ledgeWalking = false;
            }

            // Change player's Capsule collider size 
            _cCollider.height = Mathf.Lerp(_cCollider.height, 1.79f, Time.deltaTime * 6f);
        }
        else
        {
            onGround = false;
            // shrink player's Capsule collider size 
            _cCollider.height = Mathf.Lerp(_cCollider.height, 1.3f, Time.deltaTime * 6f);
        }
    }

    /// <summary>
    /// Front Raycast check for interacting items.
    /// </summary>
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
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            jumping = true;
        }

        runState = Input.GetKey(KeyCode.LeftShift);
        _horizontal = Input.GetAxis("Horizontal");
        _movVector = new Vector3(0f, 0f, _horizontal);
    }

    private void HandleMovement()
    {
        if (_horizontal != 0 && !jumping)
        {
            transform.rotation = Quaternion.LookRotation(_movVector);
            _myAnimator.SetFloat("InputX", Mathf.Abs(_horizontal));
            walkSpeed = walkCarefully ? Mathf.Clamp(walkSpeed, 0, 0.65f) : 3f;
            transform.Translate(transform.InverseTransformDirection(transform.forward) * walkSpeed * Time.deltaTime);
        }

        // jump key pressed and player is onGround
        if (jumping)
        {
            StartCoroutine(JumpLogic());
            jumping = false;
        }
    }

    IEnumerator JumpLogic()
    {
        if (_horizontal != 0)
        {
            _myAnimator.Play("Jump Running");
            yield return new WaitForSeconds(0f);

            _myRigidbody.AddForce(transform.up * jumpVerticalDist, ForceMode.Acceleration);
            _myRigidbody.AddForce(transform.forward * jumpFrwdDist, ForceMode.Acceleration);
        }
        else
        {
            _myAnimator.Play("Jump Idle");
            yield return new WaitForSeconds(0.4f);
            Debug.Log("Idle Jump!!");
            _myRigidbody.AddForce(transform.up * jumpVerticalDist, ForceMode.Acceleration);
        }

        yield return null;
    }
    
    private void ManageStates()
    {
        walkCarefully = ledgeWalking;
        _myAnimator.SetBool("Grounded", onGround);
        _myAnimator.SetBool("On Ledge", ledgeWalking);
    }

}