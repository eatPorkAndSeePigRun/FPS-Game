using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform m_transform;
    CharacterController m_ch;

    float m_movSpeed = 3.0f;
    float m_gravity = 2.0f;
    public int m_life = 5;

    Transform m_camTransform;
    Vector3 m_camRot;
    float m_camHeight = 1.4f;

    Transform m_muzzlepoint;
    public LayerMask m_layer;
    public Transform m_fx;
    public AudioClip m_audio;
    float m_shootTimer = 0;

    void Start()
    {
        m_transform = this.transform;
        m_ch = this.GetComponent<CharacterController>();

        m_camTransform = Camera.main.transform;
        m_camTransform.position = m_transform.TransformPoint(0, m_camHeight, 0);
        m_camTransform.rotation = m_transform.rotation;
        m_camRot = m_camTransform.eulerAngles;

        m_muzzlepoint = m_camTransform.FindChild("M16/weapon/muzzlepoint").transform;

        Screen.lockCursor = true;
    }

    void Update()
    {
        if (m_life < 0)
            return;
        Control();
    }

    void Control()
    {
        float rh = Input.GetAxis("Mouse X");
        float rv = Input.GetAxis("Mouse Y");
        m_camRot.x -= rv;
        m_camRot.y += rh;
        m_camTransform.eulerAngles = m_camRot;

        Vector3 camrot = m_camTransform.eulerAngles;
        camrot.x = 0;
        camrot.z = 0;
        m_transform.eulerAngles = camrot;

        m_shootTimer -= Time.deltaTime;
        if (Input.GetMouseButton(0) && m_shootTimer <= 0)
        {
            m_shootTimer = 0.1f;
            this.GetComponent<AudioSource>().PlayOneShot(m_audio);
            GameManager.Instance.SetAmmo(1);
            RaycastHit info;
            bool hit = Physics.Raycast(m_muzzlepoint.position,
                m_camTransform.TransformDirection(Vector3.forward), out info, 100, m_layer);
            if (hit)
            {
                if (info.transform.tag.CompareTo("enemy") == 0)
                {
                    Enemy enemy = info.transform.GetComponent<Enemy>();
                    enemy.OnDamage(1);
                }
                Instantiate(m_fx, info.point, info.transform.rotation);
            }
        }

        Vector3 motion = Vector3.zero;
        motion.x = Input.GetAxis("Horizontal") * m_movSpeed * Time.deltaTime;
        motion.z = Input.GetAxis("Vertical") * m_movSpeed * Time.deltaTime;
        motion.y -= m_gravity * Time.deltaTime;
        m_ch.Move(m_transform.TransformDirection(motion));

        m_camTransform.position = m_transform.TransformPoint(0, m_camHeight, 0);
    }

    public void OnDamage(int damage)
    {
        m_life -= damage;
        GameManager.Instance.SetLife(m_life);
        if (m_life < 0)
            Screen.lockCursor = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.m_transform.position, "Spawn.tif");
    }
}
