using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CanvasGroup bgFader;

    private void Awake()
    {
        if (bgFader == null)
        {
            bgFader = GetComponentInChildren<CanvasGroup>();
        }
    }

    private void Start()
    {
        FadeOut();
    }

    public void FadeIn(float duration = 0.5f)
    {
        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence
            .AppendCallback(() => bgFader.blocksRaycasts = true)
            .Append(bgFader.DOFade(1, duration));
    }

    public void FadeOut(float duration = 0.5f)
    {
        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence
            .Append(bgFader.DOFade(0, duration))
            .AppendCallback(() => bgFader.blocksRaycasts = false);
    }

    public void ChangeScene(string sceneName)
    {
        FadeIn(1f);
        DOVirtual.DelayedCall(1f, () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });
    }
}
