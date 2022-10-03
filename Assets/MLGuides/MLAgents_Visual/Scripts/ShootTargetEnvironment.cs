using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Cinemachine;

public class ShootTargetEnvironment : MonoBehaviour {

    public event EventHandler OnBirdRespawned;

    [SerializeField] private Transform birdTransform;
    [SerializeField] private Transform pfShootFlash;
    [SerializeField] private Transform pfBirdDead;

    private Rigidbody2D birdRigidbody;
    private SpriteRenderer birdSpriteRenderer;
    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake() {
        birdRigidbody = birdTransform.GetComponent<Rigidbody2D>();
        birdSpriteRenderer = birdTransform.GetComponent<SpriteRenderer>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start() {
        SpawnBird();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            ShootAt(UtilsClass.GetMouseWorldPositionZeroZ());
        }

        if (birdTransform.localPosition.y < -25f) {
            // Reset
            SpawnBird();
        }
    }

    private void SpawnBird() {
        bool leftSide = UnityEngine.Random.Range(0, 100) < 50;
        birdSpriteRenderer.flipX = !leftSide;
        float sideMultiplier = leftSide ? -1f : +1f;

        birdTransform.localPosition = new Vector3(28 * sideMultiplier, -18f + UnityEngine.Random.Range(0, 20f));

        birdRigidbody.velocity = Vector2.zero;

        float force = 30f;
        birdRigidbody.AddForce(new Vector2(-1f * sideMultiplier, UnityEngine.Random.Range(.5f, 1.6f)) * force, ForceMode2D.Impulse);

        OnBirdRespawned?.Invoke(this, EventArgs.Empty);
    }

    public bool ShootAt(Vector3 shootPosition) {
        Collider2D collider = Physics2D.OverlapPoint(shootPosition);

        Transform shootFlashTransform = Instantiate(pfShootFlash, shootPosition, Quaternion.identity);
        shootFlashTransform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360f));
        Destroy(shootFlashTransform.gameObject, 1f);

        cinemachineImpulseSource.GenerateImpulse();

        if (collider != null) {
            if (collider.TryGetComponent<Goal>(out Goal goal)) {
                // Win!
                Transform birdDeadTransform = Instantiate(pfBirdDead, birdTransform.position, Quaternion.identity);
                Destroy(birdDeadTransform.gameObject, 1f);
                birdDeadTransform.GetComponent<SpriteRenderer>().flipX = birdSpriteRenderer.flipX;
                birdDeadTransform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 30f);

                SpawnBird();
                return true;
            }
        }

        return false;
    }

}
