using System.Collections;
using UnityEngine;

public enum GameState { Ready, PlayerTurn, AITurn, Gameover }

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
    public Player p1Effect; // Player.cs 연결
    public Player p2Effect; // Player.cs 연결

    // 크리티컬 확률 (0.8 ~ 1.4배 변동)
    private float minVar = 0.8f;
    private float maxVar = 1.4f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // [테스트용] 바로 게임 시작
        StartGame();
    }

    public void StartGame()
    {
        GS = GameState.PlayerTurn;
        p1Hp = 100; p2Hp = 100; cakePos = 50f;
        uiManager.UpdateUI(p1Hp, p2Hp, cakePos);
        uiManager.ShowTurnIndicator(true); // "당신의 턴"
    }

    public void OnPlayerAction(string actionType)
    {
        if (GS != GameState.PlayerTurn) return;

        int cost = 0;
        if (actionType == "chat") cost = 2;
        else if (actionType == "attack") cost = 12;

        if (p1Hp < cost) {
            Debug.Log("멘탈 부족!"); 
            return;
        }
        
        StartCoroutine(ExecuteTurn(true, actionType));
    }

    public IEnumerator ExecuteTurn(bool isPlayer, string actionType)
    {
        ProcessAction(isPlayer, actionType);
        
        if (CheckWinCondition()) yield break;

        if (isPlayer)
        {
            GS = GameState.AITurn;
            uiManager.ShowTurnIndicator(false); // "상대 턴"
            yield return new WaitForSeconds(1.5f);
            aiOpponent.DecideNextMove();
        }
        else
        {
            GS = GameState.PlayerTurn;
            uiManager.ShowTurnIndicator(true); // "내 턴"
        }
    }

    private void ProcessAction(bool isPlayer, string actionType)
    {
        float baseCakeMove = 0f;
        int baseSelfHp = 0;
        int baseEnemyHp = 0;

        // 밸런스 패치 적용됨
        switch (actionType)
        {
            case "chat": // 잡담
                baseCakeMove = isPlayer ? -12 : 12;
                baseSelfHp = -2; 
                break;
            case "flatter": // 칭찬 (10% 버프 적용됨: 25 -> 28)
                baseCakeMove = isPlayer ? -28 : 28;
                baseEnemyHp = 12; 
                break;
            case "attack": // 비난
                baseCakeMove = isPlayer ? -5 : 5;
                baseEnemyHp = -20; 
                baseSelfHp = -10; 
                break;
            case "cry": // 슬픔 (회복 20% 너프: 30 -> 24, 케이크 밀림 페널티 추가)
                baseCakeMove = isPlayer ? 5 : -5; 
                baseSelfHp = 24; 
                break;
        }

        // 크리티컬(랜덤 보정) 계산
        float cakeMove = ApplyVariance(baseCakeMove);
        int selfHpChange = baseSelfHp;
        if (baseSelfHp > 0) selfHpChange = Mathf.RoundToInt(ApplyVariance(baseSelfHp)); // 회복만 변동
        
        int enemyHpChange = baseEnemyHp;
        if (baseEnemyHp != 0) enemyHpChange = Mathf.RoundToInt(ApplyVariance(baseEnemyHp)); // 적 영향은 무조건 변동

        // 데이터 적용
        if (isPlayer) { p1Hp += selfHpChange; p2Hp += enemyHpChange; }
        else { p2Hp += selfHpChange; p1Hp += enemyHpChange; }

        p1Hp = Mathf.Clamp(p1Hp, 0, 100);
        p2Hp = Mathf.Clamp(p2Hp, 0, 100);
        cakePos += cakeMove;
        cakePos = Mathf.Clamp(cakePos, 0f, 100f);

        // 연출 실행
        if (isPlayer) p1Effect.PlayPopEffect();
        else p2Effect.PlayPopEffect();

        if (actionType == "attack") {
            if (isPlayer) p2Effect.PlayShakeEffect();
            else p1Effect.PlayShakeEffect();
        }

        uiManager.UpdateUI(p1Hp, p2Hp, cakePos);
    }

    private float ApplyVariance(float baseVal)
    {
        if (baseVal == 0) return 0;
        float factor = Random.Range(minVar, maxVar); // 0.8 ~ 1.4
        return baseVal * factor;
    }

    private bool CheckWinCondition()
    {
        bool isGameOver = false;
        bool playerWin = false;

        if (p1Hp <= 0 || p2Hp <= 0) {
            isGameOver = true;
            if (p1Hp > p2Hp) playerWin = true;
            else if (p2Hp > p1Hp) playerWin = false;
            else playerWin = cakePos < 50;
        }
        else if (cakePos <= 0) { isGameOver = true; playerWin = true; }
        else if (cakePos >= 100) { isGameOver = true; playerWin = false; }

        if (isGameOver) {
            GS = GameState.Gameover;
            uiManager.ShowResult(playerWin);
            return true;
        }
        return false;
    }
}