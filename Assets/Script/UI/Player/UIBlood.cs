using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlood : UISpaced
{
    private bool initd = false;
    public void init(Player.PlayerAttribute playerAttribute)
    {
        if (playerAttribute == null)
            return;
        playerAttribute.playerHealth.onPlayerBloodChange += this.OnBloodChange;
        this.OnBloodChange(playerAttribute.playerHealth.blood);
        initd = true;
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
