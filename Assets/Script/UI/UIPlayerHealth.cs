using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField]GameObject _player;

    private void Start()
    {
        _player = GameObject.Find("player");
    }
    private void Update()
    {
        gameObject.GetComponent<Text>().text = "Health:" + _player.GetComponent<Player.PlayerHealth>().blood;
    }
}

