using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용 권장 (없으면 Text로 변경)

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider p1HpBar;
    public Slider p2HpBar;
    public TextMeshProUGUI p1HpText;
    public TextMeshProUGUI p2HpText;
    
    public RectTransform cakeIcon; // 케이크 이미지
    public TextMeshProUGUI turnText; // 턴 알림 텍스트
    
    [Header("Popups")]
    public GameObject winPopup;
    public GameObject losePopup;

    [Header("Positions")]
    public Transform p1Pos; // 플레이어 위치 (데미지 텍스트용)
    public Transform p2Pos; // 상대 위치

    public void UpdateUI(int p1Hp, int p2Hp, float cakePos)
    {
        // HP 갱신
        p1HpBar.value = p1Hp / 100f;
        p2HpBar.value = p2Hp / 100f;
        p1HpText.text = p1Hp.ToString();
        p2HpText.text = p2Hp.ToString();

        // 케이크 위치 이동 (Linear Interpolation)
        // 케이크 트랙의 길이에 맞춰 조절 필요 (예: -300 ~ 300)
        float xPos = Mathf.Lerp(-300f, 300f, cakePos / 100f); 
        cakeIcon.anchoredPosition = new Vector2(xPos, cakeIcon.anchoredPosition.y);
    }

    public void ShowTurnIndicator(string msg)
    {
        turnText.text = msg;
        turnText.gameObject.SetActive(true);
        // 애니메이션 효과를 넣고 싶다면 여기에 추가
        Invoke("HideTurnIndicator", 1.0f);
    }

    void HideTurnIndicator()
    {
        turnText.gameObject.SetActive(false);
    }

    public void ShowResult(bool playerWin)
    {
        if (playerWin) winPopup.SetActive(true);
        else losePopup.SetActive(true);
    }

    // 데미지/회복 텍스트 띄우기 (간단 구현)
    public void SpawnFloatingText(bool isPlayer, int value)
    {
        Debug.Log($"{(isPlayer ? "Player" : "Enemy")} Value Change: {value}");
        // 여기에 Prefab 생성 로직 추가 가능
    }

    // --- 버튼 이벤트 연결용 함수 ---
    public void OnClickChat() => GameManager.Instance.OnPlayerAction("chat");
    public void OnClickFlatter() => GameManager.Instance.OnPlayerAction("flatter");
    public void OnClickAttack() => GameManager.Instance.OnPlayerAction("attack");
    public void OnClickCry() => GameManager.Instance.OnPlayerAction("cry");
    
    public void OnClickRestart() => UnityEngine.SceneManagement.SceneManager.LoadScene(0);
}