using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoSingleton<Player>
{
    public float m_moveAccel = (0.12f * 60.0f);
    public float accelSmoothing = 3f; 
    public float m_groundFriction = 0.85f;
    public float m_gravity = (-0.05f * 60.0f);
    public float jumpgravity = (-0.05f * 60.0f);
    public float m_jumpVel = 0.75f;
    public float m_jumpMinTime = 0.06f;
    public float m_jumpMaxTime = 0.20f;
    public float m_airFallFriction = 0.975f;
    public float m_airMoveFriction = 0.85f;
    public float timeFallForShake = 0.2f;
    public CameraShake _cameraShake;
    
    private Rigidbody2D m_rigidBody = null;
    private bool m_jumpPressed = false;
    private bool m_jumpHeld = false;
    private bool m_wantsRight = false;
    private bool m_wantsLeft = false;
    private bool m_shootPressed = false;
    private bool m_fireRight = true;
    private bool m_hasWeapon = false;
    private float m_stateTimer = 0.0f;
    private Vector2 m_vel = new Vector2(0, 0);
    private List<GameObject> m_groundObjects = new List<GameObject>();
    private PlayerHealth _playerHealth;
    private float timerFall;
    private enum State
    {
        Idle = 0,
        Falling,
        Jumping,
        Walking,
        Knock
    };

    private State m_state = State.Idle;

    // Use this for initialization
    void Start ()
    {
        _playerHealth = this.GetComponent<PlayerHealth>();
        m_rigidBody = transform.GetComponent<Rigidbody2D>();
        timerFall = 0.1f;
    }

    private void Update()
    {
        UpdateInput();

        if (m_shootPressed && m_hasWeapon)
        {
            //Fire
            GameObject projectileGO = ObjectPooler.Instance.GetObject("Bullet");
            if (projectileGO)
            {
                projectileGO.GetComponent<Bullet>().Fire(transform.position, m_fireRight);
            }
        }
    }

    void FixedUpdate()
    {
        switch (m_state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Falling:
                Falling();
                break;
            case State.Jumping:
                Jumping();
                break;
            case State.Walking:
                Walking();
                break;
            case State.Knock:
                KnockUp();
                break;
            default:
                break;
        }

        if(m_wantsRight == true)
        {
            m_fireRight = true;
        }
        else if(m_wantsLeft == true)
        {
            m_fireRight = false;
        }
        if(_playerHealth.pushBack) m_state = State.Knock;
    }

    public void GiveWeapon()
    {
        m_hasWeapon = true;
    }

    void Idle()
    {
        m_vel = Vector2.zero;
        //Check to see whether to go into movement of some sort
        if (m_groundObjects.Count == 0)
        {
            //No longer on the ground, fall.
            m_state = State.Falling;
            return;
        }

        //Check input for other state transitions
        if (m_jumpPressed || m_jumpHeld)
        {
            m_stateTimer = 0;
            m_state = State.Jumping;
            return;
        }

        //Test for input to move
        if (m_wantsLeft || m_wantsRight)
        {
            m_state = State.Walking;
            return;
        }
    }

    void  Falling()
    {
        timerFall += Time.deltaTime;
        m_vel.y += m_gravity * Time.fixedDeltaTime;
        m_vel.y *= m_airFallFriction;
        if (m_wantsLeft)
        {
            m_vel.x -= m_moveAccel * Time.fixedDeltaTime;
        }
        else if (m_wantsRight)
        {
            m_vel.x += m_moveAccel * Time.fixedDeltaTime;
        }

        m_vel.x *= m_airMoveFriction;

        ApplyVelocity();
    }

    void Jumping()
    {
        timerFall = 0f;
        m_stateTimer += Time.fixedDeltaTime;

        if (m_stateTimer < m_jumpMinTime || (m_jumpHeld && m_stateTimer < m_jumpMaxTime))
        {
            m_vel.y = m_jumpVel;
        }

        m_vel.y += jumpgravity * Time.fixedDeltaTime;

        if (m_vel.y <= 0)
        {
            m_state = State.Falling;
        }

        
        if (m_wantsLeft)
        {
            if (currentAccel < m_moveAccel / 2)
            {
                currentAccel = -m_moveAccel / 2.5f;
                m_vel.x += currentAccel * Time.fixedDeltaTime;
            }
            else
            {
                m_vel.x += currentAccel * Time.fixedDeltaTime;
            }
            
        }
        else if (m_wantsRight)
        {
            if (currentAccel < m_moveAccel / 2) currentAccel = m_moveAccel / 2.5f;
            m_vel.x += currentAccel * Time.fixedDeltaTime;
        }

        m_vel.x *= m_airMoveFriction;

        ApplyVelocity();
    }
    private float currentAccel = 0f;
    void Walking()
    {
        if (m_wantsLeft)
        {
            if (currentAccel > 0)
            {
                currentAccel = 0f;  // Resetear la aceleración si cambia de dirección
            }
            // Usamos Lerp para hacer que la aceleración hacia la izquierda sea gradual
            currentAccel = Mathf.Lerp(currentAccel, -m_moveAccel, Time.fixedDeltaTime * accelSmoothing);
            
            m_vel.x += currentAccel * Time.fixedDeltaTime;
        }
        else if (m_wantsRight)
        {
            if (currentAccel < 0)
            {
                currentAccel = 0f;  // Resetear la aceleración si cambia de dirección
            }
            // Usamos Lerp para hacer que la aceleración hacia la derecha sea gradual
            currentAccel = Mathf.Lerp(currentAccel, m_moveAccel, Time.fixedDeltaTime * accelSmoothing);
            m_vel.x += currentAccel * Time.fixedDeltaTime;
        }
        else if (m_vel.x >= -0.05f && m_vel.x <= 0.05)
        {
            m_state = State.Idle;
            m_vel.x = 0;
            currentAccel = 0f;
        }
        else
        {
            m_vel.x = 0;
            currentAccel = 0f;
        }

        m_vel.y = 0;
        m_vel.x *= m_groundFriction;

        ApplyVelocity();

        if (m_groundObjects.Count == 0)
        {
            //No longer on the ground, fall.
            m_state = State.Falling;
            return;
        }

        if (m_jumpPressed || m_jumpHeld)
        {
            m_stateTimer = 0;
            m_state = State.Jumping;
            return;
        }
    }

    void KnockUp()
    {
        // Calcula la dirección de retroceso desde el punto de ataque hacia el jugador
        Vector2 knockbackDirection = (transform.position - new Vector3(_playerHealth.attackPosition.x, _playerHealth.attackPosition.y)).normalized;

        // Aplica el retroceso usando la dirección calculada y la fuerza del retroceso
        m_vel.x = knockbackDirection.x * 0.75f;
        m_vel.y = knockbackDirection.y * 0.75f;

        ApplyVelocity();
    
        // Si quieres que el retroceso tenga un tiempo limitado, puedes usar un temporizador
        StartCoroutine(EndKnockbackAfterDelay(0.2f)); // 0.2f es la duración del retroceso
    }
    IEnumerator EndKnockbackAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        _playerHealth.pushBack = false;
        // Detiene el retroceso restableciendo la velocidad a cero
        m_vel = Vector2.zero;

        // Después del retroceso, puede volver al estado adecuado, como Idle o Walking
        if (m_groundObjects.Count > 0)
        {
            m_state = State.Idle; // Si está en el suelo
        }
        else
        {
            m_state = State.Falling; // Si está en el aire
        }
    }
    
    void ApplyVelocity()
    {
        Vector3 pos = m_rigidBody.transform.position;
        pos.x += m_vel.x;
        pos.y += m_vel.y;
        m_rigidBody.transform.position = pos;
    }

    void UpdateInput()
    {
        m_wantsLeft = Input.GetKey(KeyCode.LeftArrow);
        m_wantsRight = Input.GetKey(KeyCode.RightArrow);
        m_jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);
        m_jumpHeld = Input.GetKey(KeyCode.UpArrow);
        m_shootPressed = Input.GetKeyDown(KeyCode.Space);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_groundObjects.Remove(collision.gameObject);
    }

    private void ProcessCollision(Collision2D collision)
    {
        m_groundObjects.Remove(collision.gameObject);
        Vector3 pos = m_rigidBody.transform.position;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            //Push back out
            Vector2 impulse = contact.normal * (contact.normalImpulse / Time.fixedDeltaTime);
            pos.x += impulse.x;
            pos.y += impulse.y;

            if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x))
            {
                //Hit ground
                if (contact.normal.y > 0)
                {
                    if (m_groundObjects.Contains(contact.collider.gameObject) == false)
                    {
                        m_groundObjects.Add(contact.collider.gameObject);
                    }
                    if (m_state == State.Falling)
                    {
                        //If we've been pushed up, we've hit the ground.  Go to a ground-based state.
                        if (m_wantsRight || m_wantsLeft)
                        {
                            m_state = State.Walking;
                        }
                        else
                        {
                            m_state = State.Idle;
                        }

                        if (timerFall >= timeFallForShake)
                        {
                            _cameraShake.ShakeCamera();
                            timerFall = 0f;
                        }
                        
                    }
                }
                //Hit Roof
                else
                {
                    m_vel.y = 0;
                    m_state = State.Falling;
                }
            }
            else
            {
                if ((contact.normal.x > 0 && m_vel.x < 0) || (contact.normal.x < 0 && m_vel.x > 0))
                {
                    m_vel.x = 0;
                }
            }
        }
        m_rigidBody.transform.position = pos;
    }
}
