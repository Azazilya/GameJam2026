using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public MaskData oniMask;
    public MaskData kitsuneMask;

    public PlayerStatController playerStat;

    public MaskData CurrentMask { get; private set; }

    void Awake()
    {
        CurrentMask = oniMask;
        ApplyCurrentMask();
    }

    public void SwitchMask()
    {
        CurrentMask = (CurrentMask == oniMask)
            ? kitsuneMask
            : oniMask;

        ApplyCurrentMask();
    }

    void ApplyCurrentMask()
    {
        if (playerStat != null)
            playerStat.ApplyMask(CurrentMask);
    }
}
