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
    public Player p1Effect;
    public Player p2Effect;

    private float minVar = 0.8f;
    private float maxVar = 1.4f;

    // AI 턴 타이머 (코루틴 대신 Update로 처리)
    private bool aiWaiting = false;
    private float aiTimer = 0f;
    private float aiDelay = 2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GS = GameState.Ready;
        // UI 매니저가 있으면 언어 선택창 표시, 없으면 바로 시작
        if(uiManager != null) uiManager.ShowLanguageSelect(); 
        else StartGame();
    }

    public void StartGame()
    {
        GS = GameState.PlayerTurn;
        p1Hp = 100; p2Hp = 100; cakePos = 50f;
        aiWaiting = false;
        if(uiManager) {
            uiManager.UpdateUI(p1Hp, p2Hp, cakePos);
            uiManager.ShowTurnIndicator(true);
        }
    }

    // 코루틴 대신 Update로 AI 턴 타이머 처리
    private void Update()
    {

        
        if (aiWaiting) 
        {

     //   aiTimer -= Time.deltaTime;
        aiTimer -= Time.unscaledDeltaTime;
        Debug.Log($"[AI Timer] {aiTimer:F2}s remaining, GS={GS}");

        if (aiTimer <= 0f)
        {
            aiWaiting = false;
            aiTimer = aiDelay; // 리셋
            DoAITurn();
        }
        }
    }

    private void DoAITurn()
    {
        if (aiOpponent != null)
        {
            Debug.LogWarning("AI Opponent 미연결! 랜덤 행동.");
            string[] actions = { "chat", "flatter", "attack", "cry" };
            ProcessAIAction(actions[Random.Range(0, actions.Length)]);
           // aiOpponent.DecideNextMove();
        }
        else
        {
            Debug.LogWarning("AI Opponent 미연결! 랜덤 행동.");
            string[] actions = { "chat", "flatter", "attack", "cry" };
            ProcessAIAction(actions[Random.Range(0, actions.Length)]);
        }
    }

    // 플레이어 행동 진입점
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

        // 플레이어 행동 즉시 처리
        ProcessAction(true, actionType);
        if (CheckWinCondition()) return;

        // AI 턴 시작 (타이머)
        GS = GameState.AITurn;
          aiTimer = aiDelay;
        aiWaiting = true;
        if(uiManager) uiManager.ShowTurnIndicator(false);
      
    }

    // [중요] AI 행동 진입점 (PC2AI에서 호출)
    public void ProcessAIAction(string actionType)
    {
        Debug.Log($"[Turn] AI executes: {actionType}");
        ProcessAction(false, actionType);
        if (CheckWinCondition()) return;

        // AI 턴 끝 -> 플레이어 턴
        GS = GameState.PlayerTurn;
        if(uiManager) uiManager.ShowTurnIndicator(true);
    }

    private void ProcessAction(bool isPlayer, string actionType)
    {
        if(uiManager != null) uiManager.ShowActionDialogue(isPlayer, actionType);

        float baseCakeMove = 0f;
        int baseSelfHp = 0;
        int baseEnemyHp = 0;

        switch (actionType)
        {
            case "chat": baseCakeMove = isPlayer ? -12 : 12; baseSelfHp = -2; break;
            case "flatter": baseCakeMove = isPlayer ? -28 : 28; baseEnemyHp = 12; break;
            case "attack": baseCakeMove = isPlayer ? -5 : 5; baseEnemyHp = -20; baseSelfHp = -10; break;
            case "cry": baseCakeMove = isPlayer ? 5 : -5; baseSelfHp = 24; break;
        }

        // 크리티컬 계산
        float cakeMove = ApplyVariance(baseCakeMove);
        int selfHpChange = baseSelfHp;
        if (baseSelfHp > 0) selfHpChange = Mathf.RoundToInt(ApplyVariance(baseSelfHp));
        
        int enemyHpChange = baseEnemyHp;
        if (baseEnemyHp != 0) enemyHpChange = Mathf.RoundToInt(ApplyVariance(baseEnemyHp));

        if (isPlayer) { p1Hp += selfHpChange; p2Hp += enemyHpChange; }
        else { p2Hp += selfHpChange; p1Hp += enemyHpChange; }

        p1Hp = Mathf.Clamp(p1Hp, 0, 100);
        p2Hp = Mathf.Clamp(p2Hp, 0, 100);
        cakePos += cakeMove;
        cakePos = Mathf.Clamp(cakePos, 0f, 100f);

        // 연출
        if (isPlayer && p1Effect) p1Effect.PlayPopEffect();
        else if(!isPlayer && p2Effect) p2Effect.PlayPopEffect();

        if (actionType == "attack") {
            if (isPlayer && p2Effect) p2Effect.PlayShakeEffect();
            else if(!isPlayer && p1Effect) p1Effect.PlayShakeEffect();
        }

        if(uiManager) {
            uiManager.UpdateUI(p1Hp, p2Hp, cakePos);
            if (enemyHpChange != 0) uiManager.SpawnFloatingText(!isPlayer, enemyHpChange);
            if (baseSelfHp > 0) uiManager.SpawnFloatingText(isPlayer, selfHpChange);
        }
    }

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
            if(uiManager) uiManager.ShowResult(playerWin);
            return true;
        }
        return false;
    }
}