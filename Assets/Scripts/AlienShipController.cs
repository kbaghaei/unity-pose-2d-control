using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienShipController : MonoBehaviour
{
    public int hitPoints = 3;
    public Transform explosionPrefab;
    public Transform smokePrefab;
    public AudioClip explosionAudio;
    public AudioClip hitAudio;
    private Rigidbody2D thisRigidbody2d;
    private Collider2D col2d;
    private AudioSource audioSource;
    void Start()
    {
        thisRigidbody2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void takeDamage(Vector2[] args)
    {
        Vector2 hitPoint = args[0];
        Vector2 imuplseForce = args[1];
        
        hitPoints -= 1;
        if (hitPoints > 0)
        {
            Transform smokeGo = GameObject.Instantiate(smokePrefab, hitPoint, Quaternion.identity);
            smokeGo.parent = this.transform;
            audioSource.PlayOneShot(hitAudio);
        }
        else // if (hitPoints <= 0)
        {
            col2d.enabled = false;
            thisRigidbody2d.gravityScale = 2.0f;
            thisRigidbody2d.AddForceAtPosition(imuplseForce,hitPoint, ForceMode2D.Impulse);
            GameObject.Destroy(this.gameObject, 5.0f);
            Transform expGo = GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            expGo.parent = this.transform;
            audioSource.PlayOneShot(explosionAudio);
        }
    }
}
