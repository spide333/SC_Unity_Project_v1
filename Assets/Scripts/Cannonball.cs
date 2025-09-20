using UnityEngine;

public class Cannonball : MonoBehaviour
{
    [Header("Cannonball Settings")]
    public float damage = 10f;              // 데미지
    public float explosionRadius = 1f;      // 폭발 반경
    public GameObject explosionEffect;      // 폭발 이펙트 (옵션)
    public AudioClip explosionSound;       // 폭발 사운드 (옵션)
    public LayerMask targetLayers = -1;     // 타겟 레이어
    
    [Header("Visual Trail")]
    public bool useTrail = true;           // 궤적 사용 여부
    public Color trailColor = Color.yellow; // 궤적 색상
    
    private Rigidbody2D rb;
    private TrailRenderer trail;
    private bool hasExploded = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        
        // 트레일 설정
        if (useTrail && trail != null)
        {
            trail.color = trailColor;
            trail.time = 1f;
            trail.widthCurve = AnimationCurve.Linear(0, 0.1f, 1, 0.01f);
        }
        
        // 물리 설정
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb.gravityScale = 1f;
        rb.drag = 0.1f; // 공기 저항
    }
    
    void Update()
    {
        // 화면 밖으로 나가면 삭제
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.x < -100 || screenPos.x > Screen.width + 100 || 
            screenPos.y < -100 || screenPos.y > Screen.height + 100)
        {
            DestroyCannonball();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;
        
        Explode(collision.contacts[0].point);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;
        
        // 특정 태그나 레이어와 충돌했을 때만 폭발
        if (other.CompareTag("Enemy") || other.CompareTag("Target"))
        {
            Explode(transform.position);
        }
    }
    
    public void Explode(Vector2 explosionPosition)
    {
        if (hasExploded) return;
        hasExploded = true;
        
        // 폭발 반경 내 오브젝트 찾기
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, targetLayers);
        
        foreach (Collider2D hit in hitObjects)
        {
            // 적이나 파괴 가능한 오브젝트에 데미지
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            
            // 물리적 충격 적용
            Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
            if (hitRb != null)
            {
                Vector2 explosionDirection = (hit.transform.position - (Vector3)explosionPosition).normalized;
                float explosionForce = 10f;
                hitRb.AddForce(explosionDirection * explosionForce, ForceMode2D.Impulse);
            }
        }
        
        // 폭발 이펙트 생성
        CreateExplosionEffect(explosionPosition);
        
        // 사운드 재생
        PlayExplosionSound();
        
        // 포탄 제거
        DestroyCannonball();
    }
    
    void CreateExplosionEffect(Vector2 position)
    {
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, position, Quaternion.identity);
            Destroy(effect, 2f); // 2초 후 삭제
        }
        else
        {
            // 기본 폭발 이펙트 (파티클 없이)
            CreateSimpleExplosionEffect(position);
        }
    }
    
    void CreateSimpleExplosionEffect(Vector2 position)
    {
        // 간단한 원형 폭발 이펙트
        GameObject explosionCircle = new GameObject("ExplosionEffect");
        explosionCircle.transform.position = position;
        
        SpriteRenderer sr = explosionCircle.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = new Color(1f, 0.5f, 0f, 0.7f); // 주황색
        
        // 폭발 애니메이션
        StartCoroutine(AnimateExplosion(explosionCircle));
    }
    
    System.Collections.IEnumerator AnimateExplosion(GameObject explosionObj)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * explosionRadius * 2;
        SpriteRenderer sr = explosionObj.GetComponent<SpriteRenderer>();
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            explosionObj.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 1f - t; // 페이드 아웃
                sr.color = color;
            }
            
            yield return null;
        }
        
        Destroy(explosionObj);
    }
    
    void PlayExplosionSound()
    {
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }
    }
    
    void DestroyCannonball()
    {
        Destroy(gameObject);
    }
    
    Sprite CreateCircleSprite()
    {
        // 간단한 원 스프라이트 생성
        int resolution = 64;
        Texture2D texture = new Texture2D(resolution, resolution);
        Color[] pixels = new Color[resolution * resolution];
        
        Vector2 center = new Vector2(resolution / 2f, resolution / 2f);
        float radius = resolution / 2f - 2;
        
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                if (distance <= radius)
                {
                    float alpha = 1f - (distance / radius);
                    pixels[x + y * resolution] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    pixels[x + y * resolution] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f));
    }
    
    void OnDrawGizmos()
    {
        // 폭발 반경 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

// 데미지를 받을 수 있는 오브젝트를 위한 인터페이스
public interface IDamageable
{
    void TakeDamage(float damage);
}
