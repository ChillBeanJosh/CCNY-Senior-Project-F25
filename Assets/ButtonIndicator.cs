using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;

public class ButtonIndicator : MonoBehaviour
{
    public MMF_Player EntranceFeedback;
    public MMF_Player ExitFeedback;
    public MMF_Player BumpFeedback;
    public TMP_Text TMPComponent;

    private bool _isEntered;

    public void Appearance(string text)
    {
        if (TMPComponent != null)
        {
            TMPComponent.text = text;
        }

        if (_isEntered)
        {
            if (BumpFeedback != null)
            {
                BumpFeedback.PlayFeedbacks();
            }
        }
        else
        {
            if (EntranceFeedback != null)
            {
                EntranceFeedback.PlayFeedbacks();
            }
            _isEntered = true;
        }
    }

    public void Exit()
    {
        if (ExitFeedback != null)
        {
            ExitFeedback.PlayFeedbacks();
        }
        _isEntered = false;
    }
}
