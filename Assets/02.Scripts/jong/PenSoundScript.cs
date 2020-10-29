using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenSoundScript : MonoBehaviour
{
    AudioSource penSound;
    // Start is called before the first frame update
    void Start()
    {
        penSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PenSound")
        {
            Debug.Log("소리 ON");
            penSound.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PenSound")
        {
            Debug.Log("소리 OFF");
            penSound.Stop();
        }

    }
}
