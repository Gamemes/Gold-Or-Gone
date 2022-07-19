using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergy : UISpaced
{
    private bool initd = false;
    public void init(Player.PlayerAttribute playerAttribute)
    {
        if (playerAttribute == null)
            return;
        playerAttribute.playerHealth.onEneryChange += this.OnChangeEnergy;
        this.OnChangeEnergy(playerAttribute.playerHealth.energy);
        initd = true;
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
