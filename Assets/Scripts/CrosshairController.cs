using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class CrosshairController : MonoBehaviour
{   
    [SerializeField] private PlayerController playerController;

    [Header("準心四線")]
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    [Header("擴散設定")]
    public float idleSpacing = 8f;
    public float moveSpacing = 20f;
    public float expandSpeed = 8f;

    [Header("變色設定")]
    public Image[] crosshairImages;
    public Color normalColor = Color.white;
    public Color hitColor = Color.red;
    public float hitFlashTime = 0.15f;

    private float targetSpacing;
    private float currentSpacing;

    void Start()
    {
        currentSpacing = idleSpacing;
        targetSpacing = idleSpacing;
        SetColor(normalColor);

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        bool isMoving = playerController.isMoving;
        bool isJumping = playerController.isJumping;
        bool isShooting = Mouse.current.leftButton.isPressed;

        targetSpacing = (isMoving|| isShooting || isJumping) ? moveSpacing : idleSpacing;

        currentSpacing = Mathf.Lerp(currentSpacing, targetSpacing, Time.deltaTime * expandSpeed);
        UpdateLinePositions();
    }

    void UpdateLinePositions()
    {
        top.anchoredPosition = new Vector2(0, currentSpacing);
        bottom.anchoredPosition = new Vector2(0, -currentSpacing);
        left.anchoredPosition = new Vector2(-currentSpacing, 0);
        right.anchoredPosition = new Vector2(currentSpacing, 0);
    }

    void SetColor(Color color)
    {
        foreach (var img in crosshairImages)
        {
            img.color = color;
        }
    }

    public void FlashRed()
    {   
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        SetColor(hitColor);
        yield return new WaitForSeconds(hitFlashTime);
        SetColor(normalColor);
    }
}
