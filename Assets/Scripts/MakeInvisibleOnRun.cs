// Copyright (c) 2021 Kourosh T. Baghaei
//
// This source code is licensed under the license found in the
// LICENSE file in the root directory of this source tree.



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeInvisibleOnRun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var renderer = this.GetComponent<Renderer>();
        renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
