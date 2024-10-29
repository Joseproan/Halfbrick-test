using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;
    public float m_moveSpeed = (0.05f * 60.0f);
    public float m_changeSpeed = 0.2f * 60.0f;
    public float m_moveDuration = 3.0f;
    public float m_holdDuration = 0.5f;
    public float m_chargeCooldownDuration = 2.0f;
    public float m_chargeMinRange = 1.0f;
    public float m_maxHealth = 4.0f;

    public Player m_player = null;
    public EnemyWeakPoint _enemyWeakPoint;
    
    private Rigidbody2D m_rigidBody = null;
    private float m_health = 100.0f;
    private float m_timer = 0.0f;
    private float m_lastPlayerDiff = 0.0f;
    private Vector2 m_vel = new Vector2(0, 0);
    
    //Bullets
    internal Vector3 bulletPosition;
    [SerializeField] private float pushX = 1f;
    [SerializeField] private float pushY = 1f;
    [SerializeField] private Color damagedColor;
    private SpriteRenderer enemyRenderer;
    private Color mainColor;
    [SerializeField] private GameObject getHitFx;
    
    private enum WallCollision
    {
        None = 0,
        Left,
        Right
    };
    private WallCollision m_wallFlags = WallCollision.None;

    private enum State
    {
        Idle = 0,
        Walking,
        Charging,
        ChargingCooldown,
        KnockUp
    };
    private State m_state = State.Idle;

    // Start is called before the first frame update
    void Start()
    {
        m_health = m_maxHealth;
        m_rigidBody = transform.GetComponent<Rigidbody2D>();
        gameManager = GameManager.instance;
        enemyRenderer = GetComponent<SpriteRenderer>();
        mainColor = enemyRenderer.color;
    }

    private void Update()
    {
        if (m_player == null)
        {
            m_player = gameManager.playerClone.GetComponent<Player>();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        switch (m_state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Walking:
                Walking();
                break;
            case State.Charging:
                Charging();
                break;
            case State.ChargingCooldown:
                ChargingCooldown();
                break;
            case State.KnockUp:
                KnockUp();
                break;
            default:
                break;
        }

        m_wallFlags = WallCollision.None;
    }
    public void InflictDamage(float damageAmount)
    {
        Debug.Log("entra");
        m_health -= damageAmount;
        if(m_health <= 0.0f)
        {
            GameObject.Destroy(gameObject);
        }
    }
    void KnockUp()
    {
        // Calcula la dirección de retroceso desde el punto de ataque hacia el jugador
        Vector2 knockbackDirection = (transform.position - new Vector3(bulletPosition.x, bulletPosition.y)).normalized;

        // Aplica el retroceso usando la dirección calculada y la fuerza del retroceso
        m_vel.x = knockbackDirection.x * pushX;
        m_vel.y = knockbackDirection.y * pushY;

        ApplyVelocity();
    
        // Si quieres que el retroceso tenga un tiempo limitado, puedes usar un temporizador
        StartCoroutine(EndKnockbackAfterDelay(0.2f)); // 0.2f es la duración del retroceso
    }
    IEnumerator EndKnockbackAfterDelay(float duration)
    {
        enemyRenderer.color = damagedColor;
        
        yield return new WaitForSeconds(duration);
        enemyRenderer.color = mainColor;
        m_vel = Vector2.zero;
        
        m_state = State.Idle; // Si está en el suelo
    }
    void Idle()
    {
        m_vel = Vector2.zero;

        float yDiff = m_player.transform.position.y - transform.position.y;
        if(Mathf.Abs(yDiff) <= m_chargeMinRange)
        {
            //Charge at the player!
            m_lastPlayerDiff = m_player.transform.position.x - transform.position.x;
            m_vel.x = m_changeSpeed * Mathf.Sign(m_lastPlayerDiff);
            m_timer = 0;
            m_state = State.Charging;
            return;
        }

        m_timer += Time.deltaTime;
        if(m_timer >= m_holdDuration)
        {
            m_timer = 0;
            m_state = State.Walking;

            if(m_wallFlags == WallCollision.None)
            {
                //Randomly choose.
                m_vel.x = (Random.Range(0.0f, 100.0f) < 50.0f) ? m_moveSpeed : -m_moveSpeed;
            }
            else
            {
                m_vel.x = (m_wallFlags == WallCollision.Left) ? m_moveSpeed : -m_moveSpeed;
            }
            return;
        }
    }

    void Walking()
    {
        ApplyVelocity();

        float yDiff = m_player.transform.position.y - transform.position.y;
        if (Mathf.Abs(yDiff) <= m_chargeMinRange)
        {
            //Charge at the player!
            m_lastPlayerDiff = m_player.transform.position.x - transform.position.x;
            m_vel.x = m_changeSpeed * Mathf.Sign(m_lastPlayerDiff);
            m_timer = 0;
            m_state = State.Charging;
            return;
        }

        m_timer += Time.deltaTime;
        if (m_timer >= m_moveDuration)
        {
            //No longer on the ground, fall.
            m_timer = 0.0f;
            m_state = State.Idle;
            return;
        }
    }

    void Charging()
    {
        //Charge towards player until you pass it's x position.
        ApplyVelocity();

        float xDiff = m_player.transform.position.x - transform.position.x;
        if (Mathf.Sign(m_lastPlayerDiff) != Mathf.Sign(xDiff))
        {
            //Charge at the player!
            m_vel.x = 0.0f;
            m_timer = 0;
            m_state = State.ChargingCooldown;
            return;
        }
    }

    void ChargingCooldown()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_chargeCooldownDuration)
        {
            //No longer on the ground, fall.
            m_timer = 0.0f;
            m_state = State.Idle;
            return;
        }
    }

    void ApplyVelocity()
    {
        Vector3 pos = m_rigidBody.transform.position;
        pos.x += m_vel.x * Time.fixedDeltaTime;
        pos.y += m_vel.y * Time.fixedDeltaTime;
        m_rigidBody.transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision);

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Instantiate(getHitFx, this.transform.position, Quaternion.identity);
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            bulletPosition = bullet.owner.transform.position;
            m_state = State.KnockUp;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
    }

    private void ProcessCollision(Collision2D collision)
    {
        Vector3 pos = m_rigidBody.transform.position;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            //Push back out
            Vector2 impulse = contact.normal * (contact.normalImpulse / Time.fixedDeltaTime);
            pos.x += impulse.x;
            pos.y += impulse.y;

            if (Mathf.Abs(contact.normal.y) < Mathf.Abs(contact.normal.x))
            {
                if ((contact.normal.x > 0 && m_vel.x < 0) || (contact.normal.x < 0 && m_vel.x > 0))
                {
                    m_vel.x = 0;
                    //Stop us.
                    m_wallFlags = (contact.normal.x < 0) ? WallCollision.Left : WallCollision.Right;

                    m_state = State.Idle;
                    m_timer = 0;
                }
            }
        }
        m_rigidBody.transform.position = pos;
    }
}
