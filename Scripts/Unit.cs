using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Unit : BaseObject, ISetDamage
{
    [SerializeField] private int _health;
    private bool _dead;

    public int Health { get => _health; set => _health = value; }
    public bool Dead { get => _dead; set => _dead = value; }

    [PunRPC]
    public void SetDamageRPC(int damage)
    {
        _health -= damage;
    }


        public void SetDamage(int damage)
    {
        if (_health > 0)
        {
            //_health -= damage;
            Photon.RPC("SetDamageRPC", RpcTarget.All, damage);
        }
        if(_health <= 0)
        {
            _health = 0;

            if(tag != "Player")
            {
                _dead = true;
                //Anim death
            }
            else
            {
                _dead = true;
            }
        }
            
    }

}
