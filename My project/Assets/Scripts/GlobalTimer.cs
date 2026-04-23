using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GlobalTimer : MonoBehaviour
{
    public static GlobalTimer Instance { get; private set; }

    public float CurrentTime { get; private set; }
    public bool IsTimerRunning { get; private set; }

    // 自动找到的文本（兼容 Text 和 TextMeshProUGUI）
    private Text _passedTimeText;
    private TMP_Text _passedTimeTextTMP;

    private void Awake()
    {
        // 单例 + 防重复
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (FindPassedTimeText())
        {
            GlobalTimer.Instance.StartTimer();
        }
        
    }

    /// <summary>
    /// 自动找场景里叫 "PassedTime" 的TMP文本
    /// </summary>
    private bool FindPassedTimeText()
    {
        GameObject obj = GameObject.Find("PassedTime");
        if (obj != null)
        {
            // 优先尝试 TextMeshProUGUI（推荐），其次兼容旧版 Text
            _passedTimeTextTMP = obj.GetComponent<TMP_Text>();
            if (_passedTimeTextTMP != null)
            {
                Debug.Log("[GlobalTimer] 找到 TextMeshProUGUI");
                return true;
            }
            _passedTimeText = obj.GetComponent<Text>();
            if (_passedTimeText != null)
            {
                Debug.Log("[GlobalTimer] 找到旧版 Text（建议升级为TMP）");
                return true;
            }
            Debug.LogWarning("[GlobalTimer] PassedTime 上未找到文本组件（Text/TMP_Text）");
            return false;
        }
        else
        {
            Debug.Log("未找到PassedTime");
            return false;
        }
    }

    public void StartTimer()
    {
        IsTimerRunning = true;
    }

    public void PauseTimer()
    {
        IsTimerRunning = false;
    }

    public void ResetTimer()
    {
        CurrentTime = 0f;
        UpdateText();
    }

    private void Update()
    {
        if (IsTimerRunning)
        {
            CurrentTime += Time.unscaledDeltaTime;
            UpdateText();
        }
    }

    /// <summary>
    /// 更新显示 00:00
    /// </summary>
    private void UpdateText()
    {
        if (_passedTimeText == null && _passedTimeTextTMP == null)
            return;

        int min = Mathf.FloorToInt(CurrentTime / 60);
        int sec = Mathf.FloorToInt(CurrentTime % 60);
        string display = $"时间：{min:00}:{sec:00}";

        if (_passedTimeTextTMP != null)
            _passedTimeTextTMP.text = display;
        else if (_passedTimeText != null)
            _passedTimeText.text = display;
    }
}