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
