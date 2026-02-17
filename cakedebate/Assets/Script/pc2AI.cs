using UnityEngine;

// 클래스 이름을 PC2AI로 유지 (기존 파일명과 일치시키기 위해)
public class PC2AI : MonoBehaviour
{
    // GameManager가 이 함수를 호출해서 AI 턴을 시작시킵니다.
    public void DecideNextMove()
    {
        // 현재 게임 상태 가져오기
        int myHp = GameManager.Instance.p2Hp;
        int playerHp = GameManager.Instance.p1Hp;
        float cakePos = GameManager.Instance.cakePos;

        string action = "chat"; // 기본값

        // 1. 위기 상황: HP가 20 이하일 때 회복 시도
        if (myHp <= 20) 
        {
            action = "cry"; 
        }
        // 2. 유리한 상황: 케이크가 플레이어 쪽(0~30)에 있으면 '칭찬'으로 확 당기기
        else if (cakePos <= 30) 
        {
            action = "flatter"; 
        }
        // 3. 공격 기회: 플레이어 체력이 많으면 견제 (50% 확률)
        else if (playerHp > 50 && Random.value > 0.5f) 
        {
            action = "attack"; 
        }
        // 4. 그 외: 랜덤 행동
        else 
        {
            string[] actions = { "chat", "flatter", "attack" };
            action = actions[Random.Range(0, actions.Length)];
        }

        // 결정된 행동을 실행 (AI 턴이므로 isPlayer = false)
        StartCoroutine(GameManager.Instance.ExecuteTurn(false, action));
    }
}