using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public abstract class EnemmiesControllerBase : MonoBehaviour
{
    protected Rigidbody2D _enemyRb;
    protected Animator _enemyAnimator;
    protected Vector2 _startPoint;
    protected EnemyState _currentState;

    protected float _lastStateChange;
    protected float _timeToNextChange;

    [SerializeField] private float _maxStateTime;
    [SerializeField] private float _minStateTime;
    [SerializeField] private EnemyState[] _availablesState;

    protected Player_controller _player;

    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _range;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _whatIsGround;

    private bool _faceRight = true;


    void Start()
    {
        _startPoint = transform.position;
        _enemyRb = GetComponent<Rigidbody2D>();
        _enemyAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isGroundEnding())
        {
            Flip();
        }
        if (_currentState == EnemyState.Move)
        {
            Move();
        }

    }

    protected void Update()
    {
        if(Time.time - _lastStateChange > _timeToNextChange)
        {
            GetRandomState();
        }
    }

    protected virtual void Move()
    {
        _enemyRb.velocity = transform.right * new Vector2(_speed, _enemyRb.velocity.y);
    }

    protected void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_range * 2, 0.5f, 0));
    }

    private bool isGroundEnding()
    {
        return !Physics2D.OverlapPoint(_groundCheck.position, _whatIsGround);
    }

    protected void GetRandomState()
    {
        int state = Random.Range(0, _availablesState.Length);
        if (_currentState == EnemyState.Idel && _availablesState[state] == EnemyState.Idel)
        {
            GetRandomState();
        }
        _lastStateChange = Random.Range(_minStateTime, _maxStateTime);
        ChangeState(_availablesState[state]);

    }

    protected virtual void ChangeState(EnemyState state)
    {
        if(_currentState != EnemyState.Idel)
            _enemyAnimator.SetBool(_currentState.ToString(), false);

        if (state != EnemyState.Idel)
            _enemyAnimator.SetBool(state.ToString(), true);

        _currentState = state;
        _lastStateChange = Time.time;

    }

}

public enum EnemyState
{
    Idel, 
    Move,
}
