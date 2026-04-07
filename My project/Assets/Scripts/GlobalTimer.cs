using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GlobalTimer : MonoBehaviour
{
    public static GlobalTimer Instance { get; private set; }

    public float CurrentTime { get; private set; }
    public bool IsTimerRunning { get; private set; }

    // 自动找到的文本
    private Text _passedTimeText;

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
            _passedTimeText = obj.GetComponent<Text>();
            return true;
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
        if (_passedTimeText == null)
            return;

        int min = Mathf.FloorToInt(CurrentTime / 60);
        int sec = Mathf.FloorToInt(CurrentTime % 60);
        _passedTimeText.text = $"时间：{min:00}:{sec:00}";
    }
}