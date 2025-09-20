using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Cannon Settings")]
    public GameObject cannonballPrefab;  // 포탄 프리팹
    public Transform firePoint;          // 발사 위치
    public float fireForce = 20f;        // 발사 힘
    public float minForce = 5f;          // 최소 힘
    public float maxForce = 30f;         // 최대 힘
    
    [Header("Visual Effects")]
    public LineRenderer trajectoryLine;  // 궤적 라인 (옵션)
    public int trajectoryPoints = 50;    // 궤적 점 개수
    
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 startMousePos;
    private Vector3 currentMousePos;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        // firePoint가 없으면 자동으로 생성
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(1f, 0f, 0f); // 대포 앞쪽
            firePoint = firePointObj.transform;
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // 터치 입력 (모바일) 또는 마우스 입력
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            
            // 대포 클릭 체크
            if (IsClickOnCannon(mouseWorldPos))
            {
                isDragging = true;
                startMousePos = mouseWorldPos;
                
                if (trajectoryLine != null)
                    trajectoryLine.enabled = true;
            }
        }
        
        if (isDragging && Input.GetMouseButton(0))
        {
            currentMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos.z = 0;
            
            // 궤적 미리보기 표시
            ShowTrajectoryPreview();
        }
        
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            
            if (trajectoryLine != null)
                trajectoryLine.enabled = false;
                
            FireCannonball();
        }
    }
    
    bool IsClickOnCannon(Vector3 worldPos)
    {
        // 대포와의 거리 체크 (간단한 방법)
        float distance = Vector3.Distance(worldPos, transform.position);
        return distance < 2f; // 2유닛 이내면 클릭으로 인정
    }
    
    void ShowTrajectoryPreview()
    {
        if (trajectoryLine == null) return;
        
        Vector3 direction = (startMousePos - currentMousePos).normalized;
        float distance = Vector3.Distance(startMousePos, currentMousePos);
        float force = Mathf.Clamp(distance * 5f, minForce, maxForce);
        
        // 궤적 계산 및 표시
        Vector3[] points = CalculateTrajectory(firePoint.position, direction * force, trajectoryPoints);
        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.SetPositions(points);
    }
    
    void FireCannonball()
    {
        // GameManager 체크
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !gameManager.CanFireCannonball())
        {
            Debug.Log("포탄을 모두 사용했습니다!");
            return;
        }
        
        if (cannonballPrefab == null)
        {
            Debug.LogError("Cannonball Prefab이 설정되지 않았습니다!");
            return;
        }
        
        // 발사 방향과 힘 계산
        Vector3 direction = (startMousePos - currentMousePos).normalized;
        float distance = Vector3.Distance(startMousePos, currentMousePos);
        float force = Mathf.Clamp(distance * 5f, minForce, maxForce);
        
        // 포탄 생성
        GameObject cannonball = Instantiate(cannonballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = cannonball.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
        
        // 포탄을 5초 후 자동 삭제
        Destroy(cannonball, 5f);
        
        // GameManager에 발사 알림
        if (gameManager != null)
        {
            gameManager.OnCannonballFired();
        }
        
        // 사운드 효과 (옵션)
        // AudioSource.PlayClipAtPoint(fireSound, firePoint.position);
        
        Debug.Log($"대포 발사! 힘: {force}, 방향: {direction}");
    }
    
    Vector3[] CalculateTrajectory(Vector3 startPoint, Vector3 startVelocity, int steps)
    {
        Vector3[] points = new Vector3[steps];
        float timestep = 0.1f;
        Vector3 gravityAccel = Physics2D.gravity;
        
        for (int i = 0; i < steps; i++)
        {
            float t = i * timestep;
            points[i] = startPoint + startVelocity * t + 0.5f * gravityAccel * t * t;
        }
        
        return points;
    }
    
    void OnDrawGizmos()
    {
        // Scene 뷰에서 firePoint 표시
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.2f);
        }
    }
}
