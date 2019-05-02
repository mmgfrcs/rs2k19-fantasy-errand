using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneChanger : MonoBehaviour {

    public Image fader;
    public Slider loadingSlider;

    public event System.Action OnSceneLoaded;

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        loadingSlider.maxValue = 0.9f;
        loadingSlider.gameObject.SetActive(true);
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        do
        {
            loadingSlider.value = scene.progress;
            yield return null;
        }
        while (scene.progress < 0.9f);
        loadingSlider.value = 0.9f;
        fader.gameObject.SetActive(true);
        var tween = fader.DOFade(1f, 2f);
        yield return tween.WaitForCompletion();
        OnSceneLoaded?.Invoke();
        scene.allowSceneActivation = true;
    }
}
