using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    public Transform characterTransform;
    private Vector3 diff;
    void Start()
    {
        transform.parent = null;
        diff = transform.position - characterTransform.position;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = characterTransform.position + diff;
    }
}
