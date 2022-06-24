using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField]
    GameObject _player;
    Text _text;
    Player.PlayerHealth _playerHealth;


    private void Start()
    {
        _player = GameObject.Find("player");
        _text = gameObject.GetComponent<Text>();
        _playerHealth = _player.GetComponent<Player.PlayerHealth>();
    }

    private void Update()
    {
        _text.text = $"{_player.name}:{_playerHealth.blood}";//"Health:" + _playerHealth.blood;
    }
}