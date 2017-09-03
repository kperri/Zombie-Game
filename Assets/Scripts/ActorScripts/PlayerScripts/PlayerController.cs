﻿using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(ActorMotor))]
public class PlayerController : MonoBehaviour, IDamageable {

    public event Action<float> HealthChangeEvent; // called when the health of the player either increases or decreases
    public event Action DeathEvent; // called when the player dies


    public SpriteRenderer muzzleFlash;
    public float normalMovSpeed; // the normal movement speed set in the inspector
    public float runMovSpeed; // the movement speed when the player is running
    public float movSpeedWhileReloading; // the movement speed when the player is reloading
    public float backpeddleMovSpeed; // the movement speed when the player is walking backwards

    [HideInInspector]
    public int currentHealth;
    public int totalHealth;

    float horizontalDirection;
    float verticalDirection;
    float mouseToPlayerAngle;
    float movSpeed;
    Vector3 mousePosition;
    Vector3 worldPointMousePosition;
    bool isAlive;
    bool isRunning;
    GunController gunController;
    PlayerAnimation animations;
    ActorMotor motor;

    void Start()
    {
        mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.z;
        Vector3 worldPointMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        currentHealth = totalHealth;
        movSpeed = normalMovSpeed;
        gunController = GetComponentInChildren<GunController>();
        animations = GetComponentInChildren<PlayerAnimation>();
        motor = GetComponent<ActorMotor>();
        motor.SetTarget(worldPointMousePosition);
        isAlive = true;
        muzzleFlash.gameObject.SetActive(false);
    }
	
	void Update()
    {
        if (isAlive) // Turn on controls if the player is alive
        {
            if (Input.GetButtonDown("Fire1") && !IsPerformingAction()) // check if the player hit the fire button and is not currently performing another action
            {
                gunController.Fire(); // calls the Fire method from the gun controller
                if (!gunController.chamberIsEmpty)
                {
                    muzzleFlash.gameObject.SetActive(true);
                    animations.SetIsFiring(true);
                    StartCoroutine(AnimationTimer(gunController.fireRate));
                }
            }
            else if (Input.GetButtonDown("Reload") && !IsPerformingAction())
            {
                gunController.Reload();
                if (!gunController.fullChamber && !gunController.outOfBullets)
                {
                    movSpeed = movSpeedWhileReloading;
                    animations.SetIsReloading(true);
                    StartCoroutine(AnimationTimer(gunController.reloadSpeed));
                }
            }
            else if (Input.GetButton("Run") && !IsPerformingAction())
            {
                isRunning = true;
                movSpeed = runMovSpeed;
            }

            if (Input.GetButtonUp("Run"))
                isRunning = false;
        }
        else
        {
            if (DeathEvent != null)
            {
                DeathEvent();
            }
        }
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            // Have player look at cursor at all times
            mousePosition = Input.mousePosition;
            mousePosition.z = transform.position.z;
            Vector3 worldPointMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            motor.SetTarget(worldPointMousePosition);

            horizontalDirection = Input.GetAxis("Horizontal");
            verticalDirection = Input.GetAxis("Vertical");

            if (verticalDirection < 0)
                movSpeed = backpeddleMovSpeed;
            else if (!Input.GetButton("Run"))
                movSpeed = normalMovSpeed;

            motor.MoveActor(verticalDirection, movSpeed);
            motor.Strafe(-horizontalDirection, movSpeed);

            SetMovementAnimation(true);

            if (horizontalDirection == 0 && verticalDirection == 0)
            {
                SetMovementAnimation(false);
            }
        }
    }

    public bool TakeDamage (int damage)
    {
        currentHealth -= damage;
        if (HealthChangeEvent != null)
            HealthChangeEvent(-damage);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Health"))
        {
            Item healthPack = collision.GetComponent<Item>();
            currentHealth += healthPack.GetAmountToDrop();
            if (currentHealth > totalHealth)
                currentHealth = totalHealth;

            healthPack.Destroy();

            if (HealthChangeEvent != null)
                HealthChangeEvent(healthPack.GetAmountToDrop());
        }
        else if (collision.CompareTag("Ammo"))
        {
            Item ammoPack = collision.GetComponent<Item>();
            gunController.totalBulletsRemaining += ammoPack.GetAmountToDrop();
            gunController.SetBulletsText();
            ammoPack.Destroy();
        }
    }

    // Checks if the player is shooting, reloading, or running. Used to block certain actions while doing any of these things
    bool IsPerformingAction ()
    {
        return gunController.isFiring || gunController.isReloading || isRunning;
    }

    void SetMovementAnimation(bool isMoving)
    {
        animations.SetIsMoving(isMoving);
    }

    void Die()
    {
        isAlive = false;
        SetMovementAnimation(false);
        animations.SetIsDead(true);
    }

    // used to time the animations of shooting or reloading, turns off animations when finished and allows the player is move at normal speeds again
    IEnumerator AnimationTimer (float timer)
    {
        yield return new WaitForSeconds(timer);

        muzzleFlash.gameObject.SetActive(false);
        gunController.isFiring = false;
        gunController.isReloading = false;
        animations.SetIsFiring(false);
        animations.SetIsReloading(false);
        movSpeed = normalMovSpeed;
    }
}
