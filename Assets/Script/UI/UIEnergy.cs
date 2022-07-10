using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergy : UISpaced
{
    private Player.PlayerUIManager playerUIManager;
    protected override void Awake()
    {
        base.Awake();
        playerUIManager = GetComponentInParent<Player.PlayerUIManager>();
        Debug.Assert(playerUIManager != null);
    }
    private void Start()
    {
        playerUIManager.targetPlayer.onEneryChange += this.OnChangeEnergy;
    }
    void OnChangeEnergy(int energy)
    {
        while (energy > images.Count)
        {
            var img = Instantiate<Image>(this.images[0], this.transform);
            images.Add(img);
        }
        for (var i = 0; i < images.Count; i++)
        {
            if (i < energy)
                images[i].sprite = OnImage;
            else
                images[i].sprite = OffImage;
        }
    }
}
