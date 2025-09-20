using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText;
    public Text cannonballCountText;
    public Button resetButton;
    
    [Header("Game Settings")]
    public int maxCannonballs = 10;    // 최대 포탄 수
    public int score = 0;              // 현재 점수
    public int cannonballsUsed = 0;    // 사용한 포탄 수
    
    public static GameManager Instance { get; private set; }
    
    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        Debug.Log("🎮 GameManager Start() 실행됨!");
        Debug.Log($"📊 초기 설정 - 최대 포탄: {maxCannonballs}, 점수: {score}");
        
        UpdateUI();
        
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetGame);
            Debug.Log("✅ Reset 버튼 연결됨");
        }
        else
        {
            Debug.Log("⚠️ Reset 버튼이 설정되지 않음 (OnGUI 사용)");
        }
        
        Debug.Log("🎮 GameManager 초기화 완료!");
    }
    
    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
        Debug.Log($"점수 추가: +{points}, 총 점수: {score}");
    }
    
    public bool CanFireCannonball()
    {
        return cannonballsUsed < maxCannonballs;
    }
    
    public void UseCannonball()
    {
        if (CanFireCannonball())
        {
            cannonballsUsed++;
            UpdateUI();
            
            if (cannonballsUsed >= maxCannonballs)
            {
                Debug.Log("포탄을 모두 사용했습니다!");
                // 게임 오버 로직 추가 가능
            }
        }
    }
    
    public void ResetGame()
    {
        score = 0;
        cannonballsUsed = 0;
        UpdateUI();
        
        // 모든 포탄과 타겟 제거
        CleanupObjects();
        
        Debug.Log("게임 리셋!");
    }
    
    void CleanupObjects()
    {
        // 포탄들 제거
        Cannonball[] cannonballs = FindObjectsOfType<Cannonball>();
        foreach (Cannonball cannonball in cannonballs)
        {
            if (cannonball != null)
                Destroy(cannonball.gameObject);
        }
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"점수: {score}";
        }
        
        if (cannonballCountText != null)
        {
            int remaining = maxCannonballs - cannonballsUsed;
            cannonballCountText.text = $"포탄: {remaining}/{maxCannonballs}";
        }
    }
    
    // CannonController에서 호출할 수 있도록 public 메서드
    public void OnCannonballFired()
    {
        UseCannonball();
    }
    
    void OnGUI()
    {
        // 간단한 디버그 UI (UI 요소가 없을 때)
        if (scoreText == null)
        {
            GUI.Label(new Rect(10, 10, 200, 30), $"점수: {score}");
            GUI.Label(new Rect(10, 50, 200, 30), $"포탄: {maxCannonballs - cannonballsUsed}/{maxCannonballs}");
            
            if (GUI.Button(new Rect(10, 90, 100, 30), "리셋"))
            {
                ResetGame();
            }
        }
    }
}
