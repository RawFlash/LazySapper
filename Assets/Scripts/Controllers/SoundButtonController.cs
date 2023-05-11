using UnityEngine;
using UnityEngine.UI;

public class SoundButtonController : MonoBehaviour
{
    public Sprite soundOn;
    public Sprite soundOff;

    public Image buttonImage;

    void Start()
    {
        if (SoundController.instance)
        {
            SetSoundButtonImage();
        }
        else
        {
            SoundController.Init += SetSoundButtonImage;
        }
    }

    public void ChangeSoundStatus()
    {
        SoundController.instance.SetStatusMusic(!SoundController.instance.GetStatusSound());
        SetSoundButtonImage();
    }

    private void OnDestroy()
    {
        SoundController.Init -= SetSoundButtonImage;
    }

    private void SetSoundButtonImage() => buttonImage.sprite = SoundController.instance.GetStatusSound() ? soundOn : soundOff;
}
