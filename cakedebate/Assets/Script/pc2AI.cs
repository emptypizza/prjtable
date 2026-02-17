using UnityEngine;

public class PC2AI : MonoBehaviour
{
    public void DecideNextMove()
    {
        int myHp = GameManager.Instance.p2Hp;
        int playerHp = GameManager.Instance.p1Hp;
        float cakePos = GameManager.Instance.cakePos;
        string action = "chat";

        // AI 로직
        if (myHp <= 20) action = "cry"; // 위기 시 회복
        else if (cakePos <= 30) action = "flatter"; // 케이크 뺏겼으면 칭찬
        else if (playerHp > 50 && Random.value > 0.5f) action = "attack"; // 견제
        else {
            string[] actions = { "chat", "flatter", "attack" };
            action = actions[Random.Range(0, actions.Length)];
        }

        StartCoroutine(GameManager.Instance.ExecuteTurn(false, action));
    }
}