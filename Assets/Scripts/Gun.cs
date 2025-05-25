using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Gun : MonoBehaviour
{
    public PlayerController playerController;

    [Header("射擊參數")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 5f;
    public int maxAmmo = 30;
    public float reloadTime = 1.5f;
    [Range(0f, 10f)] public float spreadAngle = 3f;

    [Header("光線子彈效果")]
    public GameObject tracerPrefab;
    public float tracerSpeed = 100f;
    public float tracerLength = 1.0f;

    [Header("參考組件")]
    public Camera fpsCam;
    public Transform firePoint;
    public ParticleSystem muzzleFlash;
    public TextMeshProUGUI ammoText;
    [SerializeField] private CrosshairController crosshair;

    [Header("音效")]
    public AudioSource gunAudioSource;
    public AudioClip gunShotClip;
    public AudioClip dryFireClip;
    public AudioClip reloadClip;

    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;
    private bool isFirstShot = true;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (Mouse.current == null || Keyboard.current == null) return;
        if (isReloading) return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
            StartCoroutine(Reload());

        if (currentAmmo <= 0)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && dryFireClip && gunAudioSource)
                gunAudioSource.PlayOneShot(dryFireClip);
            return;
        }

        if (Mouse.current.leftButton.isPressed && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            isFirstShot = false;
        }

        // 放開射擊鍵後，重設為下一發是第一發
        if (!Mouse.current.leftButton.isPressed)
            isFirstShot = true;
    }

    void Shoot()
    {
        currentAmmo--;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (gunAudioSource && gunShotClip)
            gunAudioSource.PlayOneShot(gunShotClip);

        Vector3 shootDirection = GetSpreadDirection();
        Ray ray = new Ray(firePoint.position, shootDirection);
        Vector3 targetPoint = firePoint.position + shootDirection * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            targetPoint = hit.point;

            float actualDamage = 0f;
            Debug.Log($"Hit: {hit.collider.name}");

            switch (hit.collider.tag.ToLower())
            {
                case "head": actualDamage = damage * 5f; break;
                case "body": actualDamage = damage; break;
                case "leg": actualDamage = damage * 0.5f; break;
            }

            var enemy = hit.collider.GetComponentInParent<ZombieAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(actualDamage);
                if (crosshair != null)
                    crosshair.FlashRed();
            }
        }

        CreateMovingTracer(firePoint.position, shootDirection, targetPoint);
        UpdateAmmoUI();
    }

    Vector3 GetSpreadDirection()
    {
        float dynamicSpread = spreadAngle;

        if (playerController != null)
        {
            if (playerController.isJumping)
                dynamicSpread *= 2.5f;  // 跳躍擴散最多
            else if (playerController.isMoving)
                dynamicSpread *= 1.5f;  // 移動擴散中等
            // 靜止狀態使用原本 spreadAngle
        }

        if (isFirstShot)
            return fpsCam.transform.forward;

        float spreadX = Random.Range(-dynamicSpread, dynamicSpread);
        float spreadY = Random.Range(-dynamicSpread, dynamicSpread);
        Quaternion spreadRotation = Quaternion.Euler(spreadY, spreadX, 0);
        return spreadRotation * fpsCam.transform.forward;
    }


    void CreateMovingTracer(Vector3 startPos, Vector3 direction, Vector3 endPoint)
    {
        if (tracerPrefab == null) return;

        GameObject tracer = Instantiate(tracerPrefab, startPos, Quaternion.identity);
        LineRenderer lr = tracer.GetComponent<LineRenderer>();

        if (lr == null) return;
        lr.positionCount = 2;

        Vector3 offset = direction.normalized * tracerLength;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, startPos + offset);

        StartCoroutine(AnimateTracer(tracer, direction, endPoint));
    }

    IEnumerator AnimateTracer(GameObject tracer, Vector3 direction, Vector3 target)
    {
        LineRenderer lr = tracer.GetComponent<LineRenderer>();
        Vector3 dirNorm = direction.normalized;
        float traveled = 0f;

        while (traveled < range)
        {
            Vector3 delta = dirNorm * tracerSpeed * Time.deltaTime;
            traveled += delta.magnitude;

            if (lr)
            {
                lr.SetPosition(0, lr.GetPosition(0) + delta);
                lr.SetPosition(1, lr.GetPosition(1) + delta);

                float alpha = Mathf.Clamp01(1f - (traveled / range));
                Color c = new Color(1f, 1f, 0f, alpha);
                lr.startColor = c;
                lr.endColor = c;
            }

            yield return null;
        }

        Destroy(tracer);
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (gunAudioSource && reloadClip)
            gunAudioSource.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText)
            ammoText.text = $"Ammo: {currentAmmo} / {maxAmmo}";
    }
}
