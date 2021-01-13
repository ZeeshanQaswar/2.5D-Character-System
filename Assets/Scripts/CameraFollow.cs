using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform followTarget;
    public float followSpeed;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
    }

}
