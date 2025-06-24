using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class TTSPlayer : MonoBehaviour
{
    const int MaxChunk = 200;                // hard limit per request
    AudioSource audioSource;

    void Awake() => audioSource = GetComponent<AudioSource>();

    /// <summary>
    ///  Speaks <paramref name="text"/> in the requested <paramref name="lang"/> (e.g. "de", "de-DE", "es").
    /// </summary>
    public IEnumerator Speak(string text, string lang = "de")
    {
        foreach (string chunk in SplitIntoChunks(text, MaxChunk))
        {
            string url =
                "https://translate.google.com/translate_tts" +
                "?ie=UTF-8" +
                "&client=tw-ob" +                 // unofficial but still accepted
                $"&tl={lang}" +                   // target language
                $"&q={UnityWebRequest.EscapeURL(chunk, Encoding.UTF8)}";

            using UnityWebRequest req =
                UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);

            req.SetRequestHeader("User-Agent",
                "Mozilla/5.0 (Unity3D; compatible; Android)");     // ↓ avoids 403

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"TTS error: {req.error}");
                yield break;
            }

            AudioClip clip =
                DownloadHandlerAudioClip.GetContent(req);

            audioSource.clip = clip;
            audioSource.Play();

            // Wait until the current chunk finishes before fetching the next
            yield return new WaitForSeconds(clip.length);
        }
    }

    static IEnumerable<string> SplitIntoChunks(string s, int max)
    {
        for (int i = 0; i < s.Length; i += max)
        {
            int len = Mathf.Min(max, s.Length - i);
            yield return s.Substring(i, len);
        }
    }
}
