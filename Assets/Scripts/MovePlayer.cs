using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class MovePlayer : MonoBehaviour
{
    private PlayerController _player;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerController>();
    }

    public void Move()
    {
        _player.MovePlayer(1);
    }

    public void Roll()
    {
        _player.MovePlayer(2.3f);
    }
}
