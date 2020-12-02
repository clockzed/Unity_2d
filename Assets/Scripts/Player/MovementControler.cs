using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class MovementControler : MonoBehaviour
{
    private Rigidbody2D _player2D;
    private Animator _playerAnimator;
    private Player_controller _player_Controller;

    [Header("Horizontal movement")]
    [SerializeField] private float _speed;

    private bool _faceRight = true;
    private bool _canMove = true;


    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _radius;
    [SerializeField] private bool _airControll;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _whatIsGround;

    private bool _grounded;

    [Header("Crawling")]
    [SerializeField] private Transform _cellCheck;
    [SerializeField] private Collider2D _headColliader;

    [Header("Casting")]
    [SerializeField] private GameObject _fireBall;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireBallSpeed;
    [SerializeField] private int _castCost;

    private bool _canStand;

    private bool _isCasting;

    [Header("Strike")]
    [SerializeField] private Transform _strikePoint;
    [SerializeField] private int _damage;
    [SerializeField] private float _strikeRange;
    [SerializeField] private LayerMask _enemies;
    private bool _isStriking;

    [Header("PowerStrike")]
    [SerializeField] private float _chargetTime;
    public float ChargeTime => _chargetTime;
    [SerializeField] private float _powerStrikeSpeed;
    [SerializeField] private Collider2D _strikeCollider;
    [SerializeField] private int _powerStrikeDamage;
    [SerializeField] private int _powerStrikeCost;
    private List<EnemiesController> _damageEnemies= new List<EnemiesController>();


    void Start()
    {
        _player2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _player_Controller = GetComponent<Player_controller>();
    }


    public void Move(float move, bool jump, bool crawling)
    {
        if (!_canMove)
        {
            return;
        }



        if (move != 0 && (_grounded || _airControll))
            _player2D.velocity = new Vector2(_speed * move, _player2D.velocity.y); 

        if (move > 0 && !_faceRight)
        {
            Flip();
        }
        else if (move < 0 && _faceRight)
        {
            Flip();
        }

        _grounded = Physics2D.OverlapCircle(_groundCheck.position, _radius, _whatIsGround);

        if (jump && _grounded) 
        {
            _player2D.AddForce(Vector2.up * +_jumpForce);
            jump = false;
        }


        _canStand = !Physics2D.OverlapCircle(_cellCheck.position, _radius, _whatIsGround); 

        if (crawling)           
        {
            _headColliader.enabled = false;
        }
        else if (!crawling && _canStand)
        {
            _headColliader.enabled = true;
        }

        _playerAnimator.SetFloat("Speed", Mathf.Abs(move));
        _playerAnimator.SetBool("Jump", !_grounded);
        _playerAnimator.SetBool("Crouch", !_headColliader.enabled);
    }


    private void OnDrawGizmus()
    {
        Gizmos.DrawWireSphere(_groundCheck.position, _radius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_cellCheck.position, _radius);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(_strikePoint.position, _strikeRange);
    } 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemiesController enemy = collision.collider.GetComponent<EnemiesController>();
        if(enemy == null || _damageEnemies.Contains(enemy))
        {
            return; 
        }
        enemy.TakeDamage(_powerStrikeDamage);
        _damageEnemies.Add(enemy);

    }

    void Flip() 
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }


    public void StartCasting()
    {
        if (_isCasting || !_player_Controller.ChangeMP(- _castCost))
            return;
        _isCasting = true;
        _playerAnimator.SetBool("Casting", true);

    }

    public void StartPowerStrike()
    {
        _player2D.velocity = transform.right * _powerStrikeSpeed;
        _strikeCollider.enabled = true;
    }

    private void DisablePowerStrike()
    {
        _player2D.velocity = Vector2.zero;
        _strikeCollider.enabled = false;
        _damageEnemies.Clear();
    }

    public void EndPowerStrike()
    {
        _playerAnimator.SetBool("PowerStrike", false);
        _canMove = false;
        _isStriking = false;
    }

    private void CastFire()
    {
        GameObject fireBall = Instantiate(_fireBall, _firePoint.position, Quaternion.identity);
        fireBall.GetComponent<Rigidbody2D>().velocity = transform.right * _fireBallSpeed;
        fireBall.GetComponent<SpriteRenderer>().flipX = !_faceRight;
        Destroy(fireBall, 5f);
    }


    private void EndCasting()
    {
        _isCasting = false;
        _playerAnimator.SetBool("Casting", false);
    }



    public void StartStrike(float holdTime)
    {
        if(_isStriking)
        {
            return;
        }

        if (holdTime >= _chargetTime)
        {
            if (!_player_Controller.ChangeMP(-_powerStrikeCost))
                return;

            _playerAnimator.SetBool("PowerStrike", true); 
        }
        else
        {
            _playerAnimator.SetBool("Strike", true);
        }
        _isStriking = true;
    }

    private void Strike()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(_strikePoint.position, _strikeRange, _enemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            EnemiesController enemy = enemies[i].GetComponent<EnemiesController>();
            enemy.TakeDamage(_damage);
        }
    }

    private void EndStrike()
    {
        _playerAnimator.SetBool("Strike", false);
        _isStriking = false;
    }



}
