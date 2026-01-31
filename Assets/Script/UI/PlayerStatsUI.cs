using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("References")]
    public PlayerStatController playerStat;
    public MaskManager maskManager;

    [Header("Text UI")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI maskText;

    [Header("Mask Cut-In UI")]
    public GameObject maskCutInPanel;
    public Image maskCutInImage;

    [Header("Mask Sprites")]
    public Sprite oniSprite;
    public Sprite kitsuneSprite;

    [Header("Cut-In Timing")]
    public float cutInDuration = 0.8f;

    private Coroutine cutInRoutine;

    void Start()
    {
        if (maskCutInPanel != null)
            maskCutInPanel.SetActive(false);

        UpdateStatsUI();
        UpdateMaskText();
    }

    void Update()
    {
        UpdateStatsUI();
        UpdateMaskText();
        HandleCutInInput();
    }

    // =========================
    // STAT UI
    // =========================
    void UpdateStatsUI()
    {
        if (playerStat == null) return;

        if (attackText != null)
            attackText.text = $"ATK : {playerStat.Attack:F1}";

        if (defenseText != null)
            defenseText.text = $"DEF : {playerStat.Defense:F1}";

        if (moveSpeedText != null)
            moveSpeedText.text = $"SPEED : {playerStat.MoveSpeed:F1}";

        if (attackSpeedText != null)
            attackSpeedText.text = $"ASPD : {playerStat.AttackSpeed:F2}";
    }

    // =========================
    // MASK TEXT
    // =========================
    void UpdateMaskText()
    {
        if (maskText == null || maskManager == null) return;

        MaskData mask = maskManager.CurrentMask;
        if (mask == null) return;

        maskText.text = mask.maskName.ToUpper();
    }

    // =========================
    // INPUT Q → CUT-IN
    // =========================
    void HandleCutInInput()
    {
        if (maskManager == null) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 1. Switch Mask dulu
            maskManager.SwitchMask();

            // 2. Ambil mask yang BARU
            MaskData currentMask = maskManager.CurrentMask;

            // 3. Play cut-in sesuai mask baru
            if (currentMask != null)
                PlayMaskCutIn(currentMask);
        }
    }


    // =========================
    // CUT-IN
    // =========================
    void PlayMaskCutIn(MaskData mask)
    {
        if (maskCutInPanel == null || maskCutInImage == null) return;

        if (cutInRoutine != null)
            StopCoroutine(cutInRoutine);

        cutInRoutine = StartCoroutine(CutInRoutine(mask));
    }

    IEnumerator CutInRoutine(MaskData mask)
    {
        maskCutInPanel.SetActive(true);

        // Mapping sprite berdasarkan nama mask
        string maskName = mask.maskName.ToLower();

        if (maskName.Contains("oni"))
            maskCutInImage.sprite = oniSprite;
        else if (maskName.Contains("kitsune"))
            maskCutInImage.sprite = kitsuneSprite;

        yield return new WaitForSeconds(cutInDuration);

        maskCutInPanel.SetActive(false);
    }
}
