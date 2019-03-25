﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : Character
{
    private Transform m_target;

    private LevelTimer m_levelTimer;

    [SerializeField]
    protected CharacterBody2D m_characterBody;

    [SerializeField]
    private ParticleSystem m_deathParticles;

    [SerializeField]
    private SoundClip m_deathSound;

    protected override void Awake()
    {
        base.Awake();

        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            m_target = obj.transform;

        m_levelTimer = FindObjectOfType<LevelTimer>();
    }

    private void Update()
    {
        if (m_target == null)
            return;

        Vector3 targetDirection = m_target.position - transform.position;
        int layerMask = ~((1 << m_target.gameObject.layer) | (1 << gameObject.layer));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection.normalized, targetDirection.magnitude, layerMask);

        if (hit.collider != null)
            return;

        float h = Mathf.Clamp(m_target.position.x - transform.position.x, -1, 1) * m_movementSpeed;
        Move(new Vector2(h, 0));
    }

    public override float TakeDamage(float amount)
    {
        return base.TakeDamage(amount);
    }

    public override void Destroy()
    {
        m_deathSound.Play();
        m_deathParticles.transform.parent = null;
        m_deathParticles.Play();
        base.Destroy();
    }

    protected override void Move(Vector2 direction)
    {
        m_characterBody.Move(direction);

        if (m_viewController != null)
            m_viewController.Move(direction);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) {
            IDamageable damageable = collision.collider.GetComponent<IDamageable>();
            damageable.Destroy();
        }
    }
}
