using UnityEngine;
using UnityEngine.UI;
// TMPro 사용시 using TMPro; 추가하고 Text -> TextMeshProUGUI 로 변경

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider p1HpBar;
    public Slider p2HpBar;
    public Text p1HpText;
    public Text p2HpText;
    public RectTransform cakeIcon;
    public Text turnText;
    
    [Header("Popups")]
    public GameObject langPopup; // 언어 선택 팝업
    public GameObject winPopup;
    public GameObject losePopup;

    [Header("Buttons Text")]
    public Text btnChat;
    public Text btnFlatter;
    public Text btnAttack;
    public Text btnCry;

    private string curLang = "ko"; // 기본 한국어

    public void ShowLanguageSelect()
    {
        langPopup.SetActive(true);
        Time.timeScale = 0; // 게임 정지
    }

    public void SelectLanguage(string lang)
    {
        curLang = lang;
        langPopup.SetActive(false);
        Time.timeScale = 1;
        ApplyLanguage();
        GameManager.Instance.StartGame();
    }

    void ApplyLanguage()
    {
        if (curLang == "ko") {
            btnChat.text = "잡담";
            btnFlatter.text = "칭찬";
            btnAttack.text = "비난";
            btnCry.text = "슬픔";
        } else {
            btnChat.text = "雑談";
            btnFlatter.text = "賞賛";
            btnAttack.text = "非難";
            btnCry.text = "悲しみ";
        }
    }

    public void UpdateUI(int p1Hp, int p2Hp, float cakePos)
    {
        if(p1HpBar) p1HpBar.value = (float)p1Hp / 100f;
        if(p2HpBar) p2HpBar.value = (float)p2Hp / 100f;
        if(p1HpText) p1HpText.text = p1Hp.ToString();
        if(p2HpText) p2HpText.text = p2Hp.ToString();

        // 케이크 아이콘 이동 (-300 ~ 300 범위 가정)
        if(cakeIcon) {
            float xPos = Mathf.Lerp(-300f, 300f, cakePos / 100f);
            cakeIcon.anchoredPosition = new Vector2(xPos, cakeIcon.anchoredPosition.y);
        }
    }

    public void ShowTurnIndicator(bool isPlayerTurn)
    {
        if(!turnText) return;
        
        string msgKor = isPlayerTurn ? "당신의 턴!" : "상대방의 턴...";
        string msgJp = isPlayerTurn ? "あなたのターン!" : "相手のターン...";
        
        turnText.text = (curLang == "ko") ? msgKor : msgJp;
        turnText.gameObject.SetActive(true);
        Invoke("HideTurn", 1.5f);
    }

    void HideTurn() => turnText.gameObject.SetActive(false);

    public void ShowResult(bool playerWin)
    {
        if(playerWin && winPopup) winPopup.SetActive(true);
        else if(!playerWin && losePopup) losePopup.SetActive(true);
    }

    // 버튼 연결용
    public void OnClickChat() => GameManager.Instance.OnPlayerAction("chat");
    public void OnClickFlatter() => GameManager.Instance.OnPlayerAction("flatter");
    public void OnClickAttack() => GameManager.Instance.OnPlayerAction("attack");
    public void OnClickCry() => GameManager.Instance.OnPlayerAction("cry");
    public void OnClickLangKo() => SelectLanguage("ko");
    public void OnClickLangJp() => SelectLanguage("ja");
}