using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAudio : MonoBehaviour
{
    private AudioSource aSource;
    public AudioClip backGroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        aSource = GetComponent<AudioSource>();
        if (backGroundMusic != null)
        {
            aSource.clip = backGroundMusic;
            aSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
