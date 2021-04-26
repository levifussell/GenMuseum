using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneResponse : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] AudioClip ringAudio;
    [SerializeField] AudioClip voiceAudio;
    #endregion

    #region parameters
    AudioSource audioSource;

    bool jitterOn = true;
    #endregion

    #region unity methods
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = ringAudio;
        audioSource.loop = true;
        audioSource.volume = 0.1f;
        audioSource.Play();

        //AudioListener listener = FindObjectOfType<AudioListener>();
        //UnityEngine.Audio.AudioMixerGroup a = audioSource.outputAudioMixerGroup;
        //audioSource.

        Grabbable grab = this.GetComponent<Grabbable>();
        grab.OnGrabbedCallback += OnGrabbed;
        //grab.playHitSound = false;

        StartCoroutine(Jitter(1.9f, ringAudio.length - 1.9f));
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region phone methods
    IEnumerator Jitter(float lengthSeconds, float breakSeconds)
    {
        ConstantForce jitter = this.gameObject.AddComponent<ConstantForce>();
        float timerOn = lengthSeconds;
        while(jitterOn)
        {
            if(timerOn > 0)
            {
                this.transform.rotation *= Quaternion.AngleAxis(Random.Range(-10.0f, 10.0f), this.transform.right);
                timerOn -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            else
            {
                yield return new WaitForSeconds(breakSeconds);
                timerOn = lengthSeconds;
            }
        }
    }

    void OnGrabbed(Grabbable grab)
    {
        jitterOn = false;

        audioSource.loop = false;
        audioSource.maxDistance = 10.0f;
        audioSource.volume = 1.0f;
        audioSource.clip = voiceAudio;
        audioSource.playOnAwake = false;
        audioSource.Play();

        //grab.playHitSound = true;
    }
    #endregion
}
