using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI loadingText;

    private void Awake()
    {
        if (loadingText == null)
        {
            loadingText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        Debug.Log("Loading scene: " + SceneLoadManager.sceneToLoad);
        StartCoroutine(LoadSceneAsync(SceneLoadManager.sceneToLoad));
    }

    public IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        int i = 0;

        while (!loading.isDone)
        {
            if (Time.deltaTime % 1 == 0)
            {
                i++;
            }
            loadingText.text = "Loading" + new string('.', i % 3);
            yield return null;
        }
    }
}
