using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_controller : MonoBehaviour
{
   private ServiceManager _serviceManager;
    [SerializeField] private int _maxHP;
    private int _currentHP;
    [SerializeField] private int _maxMP;
    private int _currentMP;

    Vector2 _startPosition;

    void Start()
    {
        _currentHP = _maxHP;
        _currentMP = _maxMP;
        _startPosition = transform.position;
        _serviceManager = ServiceManager.Instanse;
    }

    public void ChangeHp(int value)
    {
        _currentHP += value;
        if (_currentHP > _maxHP)
        {
            _currentHP = _maxHP;
        }
        else if(_currentHP < 0)
        {
            OnDeath();
        }

        Debug.Log("Value = " + value);
        Debug.Log("Current HP = " + _currentHP);

    }

    public bool ChangeMP(int value)
    {
        Debug.Log("HP value = " + value);
        if (value < 0 && _currentMP < Mathf.Abs(value))
        {
            return false; 
        }

        _currentMP += value;
        if (_currentMP > _maxMP)
        {
            _currentMP = _maxMP;
        }
        Debug.Log("MP value = " + _currentMP);
        return true;
    }




    public void OnDeath()
    {
        _serviceManager.Restart();
        _currentHP = _maxHP;
        _currentMP = _maxMP;
    }

}
