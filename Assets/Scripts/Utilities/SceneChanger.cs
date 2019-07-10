using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour {

    public Image fader;
    public Slider loadingSlider;

    public static event System.Action OnSceneLoaded;

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
        var tween = fader.DOFade(1f, 2f).SetUpdate(true);
        yield return tween.WaitForCompletion();
        OnSceneLoaded?.Invoke();
        scene.allowSceneActivation = true;
    }
}
