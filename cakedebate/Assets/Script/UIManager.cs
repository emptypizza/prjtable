using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider p1HpBar;
    public Slider p2HpBar;
    public Text p1HpText;
    public Text p2HpText;
    public RectTransform cakeIcon;
    public Text turnText;
    
    [Header("Bubbles")]
    public GameObject p1Bubble;
    public Text p1BubbleText;
    public GameObject p2Bubble;
    public Text p2BubbleText;

    [Header("Popups")]
    public GameObject langPopup;
    public GameObject winPopup;
    public GameObject losePopup;

    [Header("Buttons Text")]
    public Text btnChat;
    public Text btnFlatter;
    public Text btnAttack;
    public Text btnCry;

    private string curLang = "ko";
    private Dictionary<string, string[]> dialogues_ko = new Dictionary<string, string[]>();
    private Dictionary<string, string[]> dialogues_ja = new Dictionary<string, string[]>();

    private void Awake()
    {
        InitializeDialogues();

        // 팝업 초기 상태 설정
    
        langPopup.transform.position = new Vector3(1080, 540, 0);
        winPopup.transform.position = new Vector3(1080, 540, 0);
        losePopup.transform.position = new Vector3(1080, 540, 0);
     
    }

    void Start()
    {
        if(winPopup) winPopup.SetActive(false);
        if(losePopup) losePopup.SetActive(false);
    }

    void InitializeDialogues()
    {


        dialogues_ko.Add("chat", new string[] { "오늘 날씨가 참 좋지 않아?", "너 그 머리핀, 어디서 샀어?", "케이크엔 역시 아메리카노지.", "근데 너 숙제는 다 했니?" });
        dialogues_ko.Add("flatter", new string[] { "넌 웃을 때가 제일 예뻐!", "너처럼 똑똑한 애는 처음 봐.", "역시 넌 배려심이 깊구나.", "오늘 스타일 완전 아이돌 같아!" });
        dialogues_ko.Add("attack", new string[] { "너 진짜 이기적이다.", "양심이 있으면 좀 비켜줄래?", "말이 안 통하네 정말.", "너 친구 없지?" });
        dialogues_ko.Add("cry", new string[] { "나 요즘 너무 힘들어...", "오늘 하루만 양보해주면 안 돼?", "흑흑... 당 떨어져서 그래...", "내가 불쌍하지도 않아?" });

        dialogues_ja.Add("chat", new string[] { "今日の天気、微妙だね。", "その服、どこで買ったの？", "ケーキにはやっぱりコーヒーだね。", "宿題は終わった？" });
        dialogues_ja.Add("flatter", new string[] { "笑った顔が素敵だね！", "君みたいな天才初めて見たよ。", "やっぱり君は優しいね。", "今日のスタイル、アイドルみたい！" });
        dialogues_ja.Add("attack", new string[] { "本当に自分勝手だね。", "良心があるなら譲ってよ。", "話が通じないな。", "友達いないでしょ？" });
        dialogues_ja.Add("cry", new string[] { "最近つらいんだ...", "今日だけ譲ってくれない？", "うう... 糖分が必要なの...", "私が可哀想じゃないの？" });
    }

    public void ShowActionDialogue(bool isPlayer, string actionType)
    {
        string[] pool = (curLang == "ko") ? dialogues_ko[actionType] : dialogues_ja[actionType];
        string msg = pool[Random.Range(0, pool.Length)];

        if (isPlayer) {
            p1BubbleText.text = msg;
            p1Bubble.SetActive(true);
            Invoke("HideP1Bubble", 2.0f);
        } else {
            p2BubbleText.text = msg;
            p2Bubble.SetActive(true);
            Invoke("HideP2Bubble", 2.0f);
        }
    }

    void HideP1Bubble() => p1Bubble.SetActive(false);
    void HideP2Bubble() => p2Bubble.SetActive(false);

    public void ShowLanguageSelect()
    {
        if(langPopup) langPopup.SetActive(true);
        Time.timeScale = 0;
    }

    public void SelectLanguage(string lang)
    {
        curLang = lang;
        if(langPopup) langPopup.SetActive(false);
        Time.timeScale = 1;
        ApplyLanguage();
        GameManager.Instance.StartGame();
    }

    void ApplyLanguage()
    {
        if (curLang == "ko") {
            if(btnChat) btnChat.text = "잡담";
            if(btnFlatter) btnFlatter.text = "칭찬";
            if(btnAttack) btnAttack.text = "비난";
            if(btnCry) btnCry.text = "슬픔";
        } else {
            if(btnChat) btnChat.text = "雑談";
            if(btnFlatter) btnFlatter.text = "賞賛";
            if(btnAttack) btnAttack.text = "非難";
            if(btnCry) btnCry.text = "悲しみ";
        }
    }

    public void UpdateUI(int p1Hp, int p2Hp, float cakePos)
    {
        if(p1HpBar) p1HpBar.value = (float)p1Hp / 100f;
        if(p2HpBar) p2HpBar.value = (float)p2Hp / 100f;
        if(p1HpText) p1HpText.text = p1Hp.ToString();
        if(p2HpText) p2HpText.text = p2Hp.ToString();

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

    public void SpawnFloatingText(bool isPlayer, int value)
    {
        Debug.Log($"{(isPlayer ? "Player" : "Enemy")} Value Change: {value}");
    }

    public void OnClickChat() => GameManager.Instance.OnPlayerAction("chat");
    public void OnClickFlatter() => GameManager.Instance.OnPlayerAction("flatter");
    public void OnClickAttack() => GameManager.Instance.OnPlayerAction("attack");
    public void OnClickCry() => GameManager.Instance.OnPlayerAction("cry");
    public void OnClickLangKo() => SelectLanguage("ko");
    public void OnClickLangJp() => SelectLanguage("ja");
}