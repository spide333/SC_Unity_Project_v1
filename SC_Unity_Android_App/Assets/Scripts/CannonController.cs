using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour
{
    [Header("Cannon Settings")]
    public GameObject cannonballPrefab;  // 포탄 프리팹
    public Transform firePoint;          // 발사 위치
    public float fireForce = 25f;        // 발사 힘 (고정)
    public float fireAngle = 30f;        // 발사 각도 (도 단위)
    
    [Header("Debug Settings")]
    public bool clickAnywhereToFire = false; // 화면 아무 곳이나 클릭해도 발사
    
    [Header("Reload System")]
    public bool isReloading = false;         // 재장전 중인가?
    public float reloadTime = 0.5f;          // 재장전 시간 (초)
    
    private Camera mainCamera;
    
    public static CannonController Instance { get; private set; }
    
    void Start()
    {
        Debug.Log("🎪 CannonController Start() 함수 실행됨!");
        Debug.Log("🎮 Unity 버전: " + Application.unityVersion);
        Debug.Log("📱 플랫폼: " + Application.platform);
        
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("⚠️ 다중 CannonController 발견! 하나만 사용해야 합니다.");
        }
        
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("❌ Main Camera를 찾을 수 없습니다!");
        }
        else
        {
            Debug.Log("✅ Main Camera 찾음: " + mainCamera.name);
        }
        
        // firePoint가 없으면 자동으로 생성
        if (firePoint == null)
        {
            Debug.Log("🔧 FirePoint 자동 생성 중...");
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(1f, 0f, 0f); // 대포 앞쪽 (오른쪽)
            firePoint = firePointObj.transform;
            Debug.Log("✅ FirePoint 생성 완료!");
        }
        
        if (cannonballPrefab == null)
        {
            Debug.LogError("❌ Cannonball Prefab이 설정되지 않음!");
        }
        else
        {
            Debug.Log("✅ Cannonball Prefab 연결됨: " + cannonballPrefab.name);
        }
        
        Debug.Log("🎪 CannonController 초기화 완료!");
    }
    
    void Update()
    {
        // 키보드 테스트 (스페이스바로 발사)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isReloading)
            {
                Debug.Log("⌨️ 스페이스바 눌림! 하지만 재장전 중입니다.");
            }
            else
            {
                Debug.Log("⌨️ 스페이스바 눌림! 강제 발사!");
                FireCannonball();
            }
        }
        
        HandleInput();
    }
    
    void HandleInput()
    {
        // 터치 입력 (모바일) 또는 마우스 입력
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("🎯 클릭 감지됨!");
            
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            
            Debug.Log($"📱 스크린 좌표: {mouseScreenPos}");
            Debug.Log($"🌍 월드 좌표: {mouseWorldPos}");
            Debug.Log($"🎪 대포 위치: {transform.position}");
            
            // 재장전 중이면 발사 불가
            if (isReloading)
            {
                Debug.Log("🔄 재장전 중입니다! 잠시 기다려주세요.");
                return;
            }
            
            // 디버그 모드: 화면 아무 곳이나 클릭해도 발사
            if (clickAnywhereToFire)
            {
                Debug.Log("🔥 디버그 모드: 화면 클릭으로 발사!");
                FireCannonball();
                return;
            }
            
            // 정상 모드: 대포 클릭 체크
            if (IsClickOnCannon(mouseWorldPos))
            {
                Debug.Log("✅ 대포 클릭 성공! 포탄 발사!");
                FireCannonball();
            }
            else
            {
                float distance = Vector3.Distance(mouseWorldPos, transform.position);
                Debug.Log($"❌ 대포 클릭 실패. 거리: {distance:F2} (필요: 3.0 이하)");
                Debug.Log("💡 힌트: Inspector에서 'Click Anywhere To Fire' 체크하면 화면 아무 곳이나 클릭해도 발사됩니다!");
            }
        }
    }
    
    bool IsClickOnCannon(Vector3 worldPos)
    {
        // 대포와의 거리 체크 (간단한 방법)
        float distance = Vector3.Distance(worldPos, transform.position);
        return distance < 3f; // 3유닛 이내면 클릭으로 인정 (더 크게)
    }
    
    Vector3 GetFireDirection()
    {
        // 설정된 각도로 발사 (라디안으로 변환)
        // 30도 = 오른쪽 위로 발사
        float angleInRadians = fireAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians), 0).normalized;
    }
    
    void FireCannonball()
    {
        Debug.Log("🚀 FireCannonball 함수 시작!");
        
        // 이미 재장전 중이면 발사하지 않음
        if (isReloading)
        {
            Debug.Log("🔄 이미 재장전 중입니다!");
            return;
        }
        
        // GameManager 체크
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !gameManager.CanFireCannonball())
        {
            Debug.Log("❌ 포탄을 모두 사용했습니다!");
            return;
        }
        
        if (cannonballPrefab == null)
        {
            Debug.LogError("❌ Cannonball Prefab이 설정되지 않았습니다!");
            return;
        }
        
        // 재장전 시작
        isReloading = true;
        Debug.Log("🔄 재장전 시작!");
        
        // 고정된 방향과 힘으로 발사
        Vector3 direction = GetFireDirection();
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position + Vector3.right;
        
        Debug.Log($"🎯 발사 위치: {spawnPosition}");
        Debug.Log($"➡️ 발사 방향: {direction}");
        
        // 포탄 생성
        GameObject cannonball = Instantiate(cannonballPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"💥 포탄 생성됨: {cannonball.name} at {spawnPosition}");
        
        // 포탄 크기를 더 크게 (테스트용)
        cannonball.transform.localScale = Vector3.one * 1.2f;
        
        Rigidbody2D rb = cannonball.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            // 천천히 움직이도록 힘 줄이기 (테스트용)
            Vector3 slowForce = direction * (fireForce * 0.5f);
            rb.AddForce(slowForce, ForceMode2D.Impulse);
            Debug.Log($"⚡ 포탄에 힘 적용: {slowForce}");
            
            // 중력 줄이기 (테스트용)
            rb.gravityScale = 0.5f;
        }
        else
        {
            Debug.LogError("❌ 포탄에 Rigidbody2D가 없습니다!");
        }
        
        // 포탄을 5초 후 자동 삭제
        Destroy(cannonball, 5f);
        
        // GameManager에 발사 알림
        if (gameManager != null)
        {
            gameManager.OnCannonballFired();
        }
        
        Debug.Log($"✅ 대포 발사 완료! 각도: {fireAngle}도, 힘: {fireForce}");
    }
    
    // 포탄이 터지거나 사라질 때 호출
    public void CompleteReload()
    {
        if (isReloading)
        {
            Debug.Log("✅ 재장전 완료! 다시 발사 가능합니다.");
            StartCoroutine(ReloadCoroutine());
        }
    }
    
    System.Collections.IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        Debug.Log("🔫 대포 준비 완료!");
    }
    
    void OnDrawGizmos()
    {
        // Scene 뷰에서 firePoint 표시
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.2f);
            
            // 발사 방향 표시
            Vector3 direction = GetFireDirection();
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(firePoint.position, direction * 2f);
        }
    }
}
