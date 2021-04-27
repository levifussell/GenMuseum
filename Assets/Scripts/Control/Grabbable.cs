using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] public Vector3 inventoryPositionOffset = Vector3.zero;
    [SerializeField] public Vector4 inventoryRotationOffset = Vector3.zero;
    [SerializeField] public float inventoryScaleCorrection = 1.0f;
    #endregion

    #region parameters
    public Action<Grabbable> OnGrabbedCallback;


    //AudioSource audioSource;
    //static AudioClip _hitClip = null;
    //static AudioClip hitClip
    //{
    //    get
    //    {
    //        if(_hitClip == null) { _hitClip = Resources.Load<AudioClip>("Audio/Hit_3"); }
    //        return _hitClip;
    //    }
    //}
    //private static bool hitIsPlaying = false;

    //public bool playHitSound = true;
    #endregion

    #region unity methods
    private void Awake()
    {
        //audioSource = this.gameObject.AddComponent<AudioSource>();
        //audioSource.clip = hitClip;
        //audioSource.volume = 0.8f;
        //audioSource.spatialBlend = 1.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(playHitSound && !hitIsPlaying)
        //{
        //    StartCoroutine(PlayHit());
        //}
    }

    //IEnumerator PlayHit()
    //{
    //    hitIsPlaying = true;
    //    audioSource.Play();
    //    while(audioSource.isPlaying)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }

    //    hitIsPlaying = false;
    //}
    #endregion

    #region grab methods
    public void StartGrab()
    {
        OnGrabbedCallback?.Invoke(this);
    }
    #endregion
}
