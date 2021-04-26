using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeView : MonoBehaviour
{
    #region parameters
    Image fadeImage = null;

    Color onColor = Color.black;
    Color offColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    public Action OnFadeOutCallback;
    public Action OnFadeInCallback;
    #endregion

    #region unity methods
    private void Awake()
    {
        fadeImage = this.GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region fade methods
    public void FadeIn(float timeSeconds)
    {
        StartCoroutine(DoFadeIn(timeSeconds));
    }
    IEnumerator DoFadeIn(float timeSeconds)
    {
        fadeImage.color = offColor;
        float fadeRate = 1.0f / timeSeconds;

        while(timeSeconds > 0)
        {
            fadeImage.color = Color.Lerp(offColor, onColor, 1.0f - timeSeconds * fadeRate);
            timeSeconds -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        fadeImage.color = onColor;

        OnFadeInCallback?.Invoke();
    }

    public void FadeOut(float timeSeconds)
    {
        StartCoroutine(DoFadeOut(timeSeconds));
    }
    IEnumerator DoFadeOut(float timeSeconds)
    {
        fadeImage.color = onColor;
        float fadeRate = 1.0f / timeSeconds;

        while(timeSeconds > 0)
        {
            fadeImage.color = Color.Lerp(onColor, offColor, 1.0f - timeSeconds * fadeRate);
            timeSeconds -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        fadeImage.color = offColor;

        OnFadeOutCallback?.Invoke();
    }
    #endregion
}
