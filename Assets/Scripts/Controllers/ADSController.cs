using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADSController : MonoBehaviour
{
    public static ADSController instance;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        DontDestroyOnLoad(this);
        IronSource.Agent.init("17eb5b23d");
        IronSource.Agent.validateIntegration();
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);

        instance = this;
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
    }

    void BannerAdLoadedEvent()
    {
        IronSource.Agent.displayBanner();
    }

    /************* RewardedVideo AdInfo Delegates *************/
    // Indicates that there’s an available ad.
    // The adInfo object includes information about the ad that was loaded successfully
    // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        GameController.instance.ActiveRewardButton();
    }

    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        GameController.instance.RewardAds();
    }

    public void InitRewardAds()
    {
        try
        {
            IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
        }
        catch { }

        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
    }
}
