using UnityEngine;

public class PC2AI : MonoBehaviour
{
    // GameManager가 이 함수를 호출해서 AI에게 "뭘 할지 결정해!" 라고 시킴
    public void DecideNextMove()
    {
        if (GameManager.Instance == null) return;

        int myHp = GameManager.Instance.p2Hp;
        int playerHp = GameManager.Instance.p1Hp;
        float cakePos = GameManager.Instance.cakePos;
        string action = "chat";

        // AI 로직 (체력과 상황에 따른 판단)
        if (myHp <= 20) 
        {
            action = "cry"; 
        }
        else if (cakePos <= 30) 
        {
            action = "flatter"; 
        }
        else if (playerHp > 50 && Random.value > 0.5f) 
        {
            action = "attack"; 
        }
        else 
        {
            string[] actions = { "chat", "flatter", "attack" };
            action = actions[Random.Range(0, actions.Length)];
        }

        Debug.Log("AI decided to: " + action);
        
        // [중요 변경] 직접 실행하지 않고 매니저에게 행동을 전달만 함
        GameManager.Instance.ProcessAIAction(action);
    }
}