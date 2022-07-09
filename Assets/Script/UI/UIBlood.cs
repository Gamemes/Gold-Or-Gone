using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlood : UISpaced
{
    private Player.PlayerUIManager playerUIManager;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        playerUIManager = GetComponentInParent<Player.PlayerUIManager>();
        Debug.Assert(playerUIManager != null);
        playerUIManager.targetPlayer.playerHealth.onPlayerBloodChange += this.OnBloodChange;
    }
    void OnBloodChange(int blood)
    {
        while (blood > images.Count)
        {
            var img = Instantiate<Image>(this.images[0], this.transform);
            images.Add(img);
        }
        for (var i = 0; i < images.Count; i++)
        {
            if (i < blood)
                images[i].sprite = OnImage;
            else
                images[i].sprite = OffImage;
        }
    }
}
