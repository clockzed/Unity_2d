using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(MovementControler))]
public class PC_InputController : MonoBehaviour
{

    MovementControler _playerMovement;
    DateTime _strikeClickTime;
    float _move;
    bool _jump;
    bool _crawling;
    bool _canAtak;

    private void Start()
    {
        _playerMovement = GetComponent<MovementControler>();
    }

    void Update()
    {
        _move = Input.GetAxisRaw("Horizontal");     
        if (Input.GetButtonUp("Jump")) 
        {
            _jump = true;
        }

        _crawling = Input.GetKey(KeyCode.C); 

        if(Input.GetKey(KeyCode.E))
        {
            _playerMovement.StartCasting();
        }

        if(Input.GetButtonDown("Fire1"))
        {
            _strikeClickTime = DateTime.Now;
            _canAtak = true;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            float holdTime = (float)(DateTime.Now - _strikeClickTime).TotalSeconds;
            if (_canAtak)
            {
                _playerMovement.StartStrike(holdTime);
            }
            _canAtak = false;
        }

        if ((DateTime.Now - _strikeClickTime).TotalSeconds >= _playerMovement.ChargeTime * 1 && _canAtak) 
        {
            _playerMovement.StartStrike(_playerMovement.ChargeTime);
            _canAtak = false;
        }


    }

    private void FixedUpdate()
    {
        _playerMovement.Move(_move, _jump, _crawling);
        _jump = false;  
    }

}
