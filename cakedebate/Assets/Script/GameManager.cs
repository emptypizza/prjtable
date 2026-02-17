using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Ready,
    PlayerTurn,
    AITurn,
    Gameover
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public GameState GS;
    public int p1Hp = 100;
    public int p2Hp = 100;
    public float cakePos = 50f; // 0(Player) ~ 100(AI)

    [Header("References")]
    public UIManager uiManager;
    public PC2AI aiOpponent;
    
    // 크리티컬 확률 (0.8 ~ 1.4배 변동)
    private float minVar = 0.8f;
    private float maxVar = 1.4f;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        GS = GameState.PlayerTurn;
        p1Hp = 100;
        p2Hp = 100;
        cakePos = 50f;
        
        // UI 초기화
        if (uiManager != null)
        {
            uiManager.UpdateUI(p1Hp, p2Hp, cakePos);
            uiManager.ShowTurnIndicator("당신의 턴!");
        }
    }

    // 플레이어가 버튼을 눌렀을 때 호출 (UIManager에서 연결)
    public void OnPlayerAction(string actionType)
    {
        if (GS != GameState.PlayerTurn) return;

        // 코스트 체크
        int cost = 0;
        if (actionType == "chat") cost = 2;
        else if (actionType == "attack") cost = 12;
        
        if (p1Hp < cost)
        {
            Debug.Log("멘탈 부족!"); // 추후 UI 알림으로 변경 가능
            return;
        }

        StartCoroutine(ExecuteTurn(true, actionType));
    }

    // 턴 실행 로직 (플레이어/AI 공용)
    public IEnumerator ExecuteTurn(bool isPlayer, string actionType)
    {
        // 1. 행동 처리 (계산 및 적용)
        ProcessAction(isPlayer, actionType);
        
        // 2. 승패 체크
        if (CheckWinCondition()) yield break;

        // 3. 턴 넘기기
        if (isPlayer)
        {
            GS = GameState.AITurn;
            uiManager.ShowTurnIndicator("상대방의 턴...");
            yield return new WaitForSeconds(1.5f); // 생각하는 척 딜레이
            
            if (aiOpponent != null)
                aiOpponent.DecideNextMove(); // AI에게 행동 지시
        }
        else
        {
            GS = GameState.PlayerTurn;
            uiManager.ShowTurnIndicator("당신의 턴!");
        }
    }

    private void ProcessAction(bool isPlayer, string actionType)
    {
        // 기본 수치 정의
        float baseCakeMove = 0f;
        int baseSelfHp = 0;
        int baseEnemyHp = 0;

        // 타입별 기본값 (밸런스 패치 적용됨)
        switch (actionType)
        {
            case "chat": // 잡담
                baseCakeMove = isPlayer ? -12 : 12;
                baseSelfHp = -2; // 코스트 (고정)
                break;
            case "flatter": // 칭찬
                baseCakeMove = isPlayer ? -28 : 28;
                baseEnemyHp = 12; // 적 회복 (변동)
                break;
            case "attack": // 비난
                baseCakeMove = isPlayer ? -5 : 5;
                baseEnemyHp = -20; // 공격 (변동)
                baseSelfHp = -10; // 코스트 (고정)
                break;
            case "cry": // 슬픔 (회복)
                baseCakeMove = isPlayer ? 5 : -5; // 밀림 (변동)
                baseSelfHp = 24; // 회복 (변동)
                break;
        }

        // --- 랜덤 변동성(크리티컬) 적용 계산 ---
        
        // 1. 케이크 이동 (무조건 변동)
        float cakeMove = ApplyVariance(baseCakeMove);
        
        // 2. 내 HP 변화 (양수=회복일 때만 변동, 음수=코스트는 고정)
        int selfHpChange = baseSelfHp;
        if (baseSelfHp > 0) 
            selfHpChange = Mathf.RoundToInt(ApplyVariance(baseSelfHp));

        // 3. 상대 HP 변화 (무조건 변동)
        int enemyHpChange = baseEnemyHp;
        if (baseEnemyHp != 0) 
            enemyHpChange = Mathf.RoundToInt(ApplyVariance(baseEnemyHp));


        // 실제 데이터 적용
        if (isPlayer)
        {
            p1Hp += selfHpChange;
            p2Hp += enemyHpChange;
        }
        else
        {
            p2Hp += selfHpChange;
            p1Hp += enemyHpChange;
        }

        // HP 0~100 제한
        p1Hp = Mathf.Clamp(p1Hp, 0, 100);
        p2Hp = Mathf.Clamp(p2Hp, 0, 100);

        // 케이크 이동 적용
        cakePos += cakeMove;
        cakePos = Mathf.Clamp(cakePos, 0f, 100f);

        // UI 갱신
        if (uiManager != null)
        {
            uiManager.UpdateUI(p1Hp, p2Hp, cakePos);
            
            // 플로팅 텍스트 (데미지/회복 표시)
            if (enemyHpChange != 0) uiManager.SpawnFloatingText(false, enemyHpChange); // 상대방 머리 위
            if (baseSelfHp > 0) uiManager.SpawnFloatingText(true, selfHpChange); // 내 머리 위 (회복일때만)
        }
    }

    // 랜덤 변동값 계산 함수 (0.8 ~ 1.4배)
    private float ApplyVariance(float baseVal)
    {
        if (baseVal == 0) return 0;
        float factor = Random.Range(minVar, maxVar);
        return baseVal * factor;
    }

    private bool CheckWinCondition()
    {
        bool isGameOver = false;
        bool playerWin = false;

        // 1. HP 조건 (0 이하 발생 시)
        if (p1Hp <= 0 || p2Hp <= 0)
        {
            isGameOver = true;
            // 둘 다 0이하면 더 높은 쪽 승리
            if (p1Hp > p2Hp) playerWin = true;
            else if (p2Hp > p1Hp) playerWin = false;
            else playerWin = cakePos < 50; // 완전 동점시 케이크 위치로 판별
        }
        // 2. 케이크 위치 조건
        else if (cakePos <= 0) { isGameOver = true; playerWin = true; }
        else if (cakePos >= 100) { isGameOver = true; playerWin = false; }

        if (isGameOver)
        {
            GS = GameState.Gameover;
            if (uiManager != null)
                uiManager.ShowResult(playerWin);
            return true;
        }
        return false;
    }
}