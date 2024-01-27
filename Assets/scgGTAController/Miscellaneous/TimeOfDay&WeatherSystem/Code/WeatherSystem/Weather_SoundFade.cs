using UnityEngine;
using System.Collections;

public class Weather_SoundFade : MonoBehaviour 
{
    /// <summary>
    /// Do we want to fade the sound in our out
    /// </summary>
    private enum Fade
    {
        IN,
        OUT
    };

    /// <summary>
    /// Function to fade sounds in
    /// </summary>
    /// <param name="fTimeToFadeIn">How long it should take to fade the sound in as a float</param>
    /// <param name="fEndVolume">The volume of the sound when the fade is finished. <i>*For max volume it should be set to 1.0f</i></param>
    public void FadeAudioIn(float fTimeToFadeIn, float fEndVolume)
    {
        StartCoroutine(FadeAudio(fTimeToFadeIn, fEndVolume, Fade.IN));
    }

    /// <summary>
    /// Function to fade sounds out
    /// </summary>
    /// <param name="fTimeToFadeIn">How long it should take to fade the sound out as a float</param>
    /// <param name="fEndVolume">The volume of the sound when the fade is finished. <i>*For lowering the sound completly it should be set to 0.0f</i></param>
    public void FadeAudioOut(float fTimeToFadeIn, float fEndVolume)
    {
        StartCoroutine(FadeAudio(fTimeToFadeIn, fEndVolume, Fade.OUT));
    }

    /// <summary>
    /// Private function that we use to fade the sound and make sure it can not be canceld after starting. This is started by either FadeAudioIn or FadeAudioOut
    /// </summary>
    /// <param name="fTimeToFade">How long it should take for the sound to fade</param>
    /// <param name="fSoundVolume">How loud we want the sound to be</param>
    /// <param name="fadeType">Should it fade <b>in</b> our <b>out</b></param>
    /// <returns></returns>
    IEnumerator FadeAudio(float fTimeToFade, float fSoundVolume, Fade fadeType)
    {
        float start = fadeType == Fade.IN ? 0.0F : fSoundVolume; // Change the higher value to the value of the sounds volume
        float end = fadeType == Fade.IN ? fSoundVolume : 0.0F; // Change the higher value to the value of the sounds volume
        float i = 0.0F;
        float step = 1.0f / fTimeToFade;

        while (i < 1.0f)
        {
            i += Time.deltaTime * step;
            this.GetComponent<AudioSource>().volume = Mathf.Lerp(start, end, i);
            yield return new WaitForSeconds(step * Time.deltaTime);
        }
    } // IEnumerator end
}

