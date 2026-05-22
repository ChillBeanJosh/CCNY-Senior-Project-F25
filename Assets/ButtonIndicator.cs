using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;

public class ButtonIndicator : MonoBehaviour
{
    public MMF_Player EntranceFeedback;
    public MMF_Player ExitFeedback;
    public MMF_Player BumpFeedback;
    public TMP_Text TMPComponent;
    public GameObject LMBImage;
    public GameObject RMBImage;

    private bool _isEntered;

    public void Appearance(string text)
    {
        if (text == "LMB")
        {
            if (TMPComponent != null) TMPComponent.transform.localScale = Vector3.zero;
            if (LMBImage != null) LMBImage.transform.localScale = Vector3.one;
            if (RMBImage != null) RMBImage.transform.localScale = Vector3.zero;
        }
        else if (text == "RMB")
        {
            if (TMPComponent != null) TMPComponent.transform.localScale = Vector3.zero;
            if (LMBImage != null) LMBImage.transform.localScale = Vector3.zero;
            if (RMBImage != null) RMBImage.transform.localScale = Vector3.one;
        }
        else
        {
            if (TMPComponent != null)
            {
                TMPComponent.transform.localScale = Vector3.one;
                TMPComponent.text = text;
            }
            if (LMBImage != null) LMBImage.transform.localScale = Vector3.zero;
            if (RMBImage != null) RMBImage.transform.localScale = Vector3.zero;
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

        if (LMBImage != null)
        {
            LMBImage.transform.localScale = Vector3.zero;
        }

        if (RMBImage != null)
        {
            RMBImage.transform.localScale = Vector3.zero;
        }

        _isEntered = false;
    }
}
