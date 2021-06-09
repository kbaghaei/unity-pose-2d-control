// Copyright (c) 2021 Kourosh T. Baghaei
//
// This source code is licensed under the license found in the
// LICENSE file in the root directory of this source tree.



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
