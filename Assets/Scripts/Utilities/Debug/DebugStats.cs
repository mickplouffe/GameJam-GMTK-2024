using TMPro;
using UnityEngine;

namespace Utilities.Debug
{
    public class DebugStats : MonoBehaviour
    {
        [SerializeField] private Transform canvas;
    

        private TextMeshProUGUI _debugText;
        //private Canvas canvas;
        private float _deltaTime;

        private void Awake()
        {
            if (canvas == null)
            {
                canvas = GetComponent<RectTransform>().transform;
            }
            CreateDebugUI();

        }

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            UpdateDebugInfo();
        }

        private void CreateDebugUI()
        {
            // Create TextMeshPro Text
            GameObject textGo = new GameObject("DebugText");
            textGo.transform.SetParent(canvas);
            _debugText = textGo.AddComponent<TextMeshProUGUI>();
            _debugText.rectTransform.anchorMin = new Vector2(0, 0);
            _debugText.rectTransform.anchorMax = new Vector2(0, 1);
            _debugText.rectTransform.pivot = new Vector2(0, 1);
            _debugText.rectTransform.anchoredPosition = new Vector2(10, -10);
            _debugText.fontSize = 22;
            _debugText.alignment = TextAlignmentOptions.TopLeft;
            _debugText.textWrappingMode = TextWrappingModes.NoWrap;
        }

        private void UpdateDebugInfo()
        {
            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;

            string debugInfo = $"FPS: {fps:F2} ({msec:F1} ms)\n";
            // debugInfo += $"Time Left: {TimerManager.Instance.timeLeft}\n";
            debugInfo += $"Memory: {SystemInfo.systemMemorySize} MB\n";
            debugInfo += $"Screen Resolution: {Screen.currentResolution.width} x {Screen.currentResolution.height} @ {Screen.currentResolution.refreshRateRatio}Hz\n";
            //debugInfo += $"GPU Utilization: N/A\n";  // Unity does not have direct API for GPU utilization
            //debugInfo += $"CPU Utilization: N/A\n";  // Unity does not have direct API for CPU utilization
            debugInfo += $"Quality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}\n";
            debugInfo += $"VSync: {QualitySettings.vSyncCount}\n";
            debugInfo += $"Target Frame Rate: {Application.targetFrameRate}\n";
            debugInfo += $"Time Scale: {Time.timeScale}\n";
            debugInfo += $"Time: {Time.time}\n";
            debugInfo += $"Real Time Since Startup: {Time.realtimeSinceStartup}\n";
            debugInfo += $"Platform: {Application.platform}\n";
            //debugInfo += $"Device Model: {SystemInfo.deviceModel}\n";
            debugInfo += $"Device Type: {SystemInfo.deviceType}\n";
            debugInfo += $"Device Name: {SystemInfo.deviceName}\n";
            debugInfo += $"Operating System: {SystemInfo.operatingSystem}\n";
            debugInfo += $"Processor Type: {SystemInfo.processorType}\n";
            debugInfo += $"Processor Count: {SystemInfo.processorCount}\n";
            debugInfo += $"System Memory Size: {SystemInfo.systemMemorySize} MB\n";
            debugInfo += $"Graphics Device Name: {SystemInfo.graphicsDeviceName}\n";
            debugInfo += $"Graphics Device Type: {SystemInfo.graphicsDeviceType}\n";
            debugInfo += $"Graphics Memory Size: {SystemInfo.graphicsMemorySize} MB\n";
            debugInfo += $"Graphics Shader Level: {SystemInfo.graphicsShaderLevel}\n";

            _debugText.text = debugInfo;
        }
    
        private void OnValidate()
        {
            if (canvas == null)
            {
                canvas = GetComponent<RectTransform>().transform;
            }
        }
    }
}