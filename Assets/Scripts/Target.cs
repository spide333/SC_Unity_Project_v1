using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    [Header("Target Settings")]
    public float maxHealth = 30f;
    public int points = 100;               // 파괴 시 획득 점수
    public Color damageColor = Color.red;  // 데미지 받을 때 색상
    public float damageFlashDuration = 0.2f; // 데미지 표시 시간
    
    [Header("Visual Effects")]
    public GameObject destroyEffect;       // 파괴 이펙트 (옵션)
    public AudioClip destroySound;        // 파괴 사운드 (옵션)
    
    private float currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private GameManager gameManager;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // GameManager 찾기 (점수 관리용)
        gameManager = FindObjectOfType<GameManager>();
        
        // 기본 스프라이트가 없으면 간단한 사각형 생성
        if (spriteRenderer != null && spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = CreateSquareSprite();
            spriteRenderer.color = Color.cyan;
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        Debug.Log($"{gameObject.name}이(가) {damage} 데미지를 받았습니다. 남은 체력: {currentHealth}");
        
        // 데미지 시각 효과
        StartCoroutine(FlashDamageColor());
        
        if (currentHealth <= 0)
        {
            DestroyTarget();
        }
    }
    
    System.Collections.IEnumerator FlashDamageColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(damageFlashDuration);
            spriteRenderer.color = originalColor;
        }
    }
    
    void DestroyTarget()
    {
        // 점수 추가
        if (gameManager != null)
        {
            gameManager.AddScore(points);
        }
        else
        {
            Debug.Log($"타겟 파괴! +{points} 점수");
        }
        
        // 파괴 이펙트
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
        
        // 파괴 사운드
        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }
        
        // 오브젝트 파괴
        Destroy(gameObject);
    }
    
    Sprite CreateSquareSprite()
    {
        // 간단한 사각형 스프라이트 생성
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    void OnMouseDown()
    {
        // 타겟 클릭 시 (테스트용)
        TakeDamage(10f);
    }
}
