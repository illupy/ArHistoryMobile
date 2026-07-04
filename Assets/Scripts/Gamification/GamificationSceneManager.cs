using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamificationSceneManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI selectedTokenText;
    public TextMeshProUGUI feedbackText;

    public Button tokenButton1;
    public Button tokenButton2;
    public Button tokenButton3;

    public TextMeshProUGUI tokenButtonText1;
    public TextMeshProUGUI tokenButtonText2;
    public TextMeshProUGUI tokenButtonText3;

    public Button zoneButton1;
    public Button zoneButton2;
    public Button zoneButton3;

    public TextMeshProUGUI zoneButtonText1;
    public TextMeshProUGUI zoneButtonText2;
    public TextMeshProUGUI zoneButtonText3;

    public Button checkButton;
    public Button resetButton;
    public Button homeButton;

    public GamificationApiService gamificationApiService;

    private GameScenarioResponse currentScenario;
    private string selectedTokenCode;
    private readonly Dictionary<string, string> currentPlacements = new Dictionary<string, string>();
    private readonly Dictionary<string, string> correctPlacements = new Dictionary<string, string>();

    private void Start()
    {
        BindStaticButtons();
        LoadScenario();
    }

    private void BindStaticButtons()
    {
        checkButton.onClick.RemoveAllListeners();
        checkButton.onClick.AddListener(CheckPlacements);

        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(ResetGame);

        homeButton.onClick.RemoveAllListeners();
        homeButton.onClick.AddListener(() => SceneTransitionManager.Instance.LoadScene("HomeScene"));
    }

    private void LoadScenario()
    {
        if (LessonSessionStore.Instance == null || LessonSessionStore.Instance.CurrentLesson == null)
        {
            feedbackText.text = "Không tìm thấy lesson hiện tại.";
            return;
        }

        long lessonId = LessonSessionStore.Instance.CurrentLesson.id;

        StartCoroutine(gamificationApiService.GetScenarioByLessonId(
            lessonId,
            json =>
            {
                ApiResponse<GameScenarioResponse> response =
                    JsonConvert.DeserializeObject<ApiResponse<GameScenarioResponse>>(json);

                if (response != null && response.success && response.data != null)
                {
                    currentScenario = response.data;
                    RenderScenario();
                }
                else
                {
                    feedbackText.text = "Không tải được game scenario.";
                }
            },
            error =>
            {
                feedbackText.text = "Lỗi tải game: " + error;
            }
        ));
    }

    private void RenderScenario()
    {
        if (currentScenario == null) return;

        titleText.text = currentScenario.title;
        instructionText.text = currentScenario.instruction;
        selectedTokenText.text = "Đang chọn: chưa có";
        feedbackText.text = "Chọn token, đặt vào zone, rồi bấm Kiểm tra.";

        // rules
        correctPlacements.Clear();
        foreach (var rule in currentScenario.rules)
        {
            correctPlacements[rule.tokenCode] = rule.correctZoneCode;
        }

        // tokens - bản v1 giới hạn 3 token
        if (currentScenario.tokens.Count > 0)
        {
            tokenButtonText1.text = currentScenario.tokens[0].displayName;
            tokenButton1.onClick.RemoveAllListeners();
            tokenButton1.onClick.AddListener(() => SelectToken(currentScenario.tokens[0].tokenCode, currentScenario.tokens[0].displayName));
        }
        if (currentScenario.tokens.Count > 1)
        {
            tokenButtonText2.text = currentScenario.tokens[1].displayName;
            tokenButton2.onClick.RemoveAllListeners();
            tokenButton2.onClick.AddListener(() => SelectToken(currentScenario.tokens[1].tokenCode, currentScenario.tokens[1].displayName));
        }
        if (currentScenario.tokens.Count > 2)
        {
            tokenButtonText3.text = currentScenario.tokens[2].displayName;
            tokenButton3.onClick.RemoveAllListeners();
            tokenButton3.onClick.AddListener(() => SelectToken(currentScenario.tokens[2].tokenCode, currentScenario.tokens[2].displayName));
        }

        // zones - bản v1 giới hạn 3 zone
        if (currentScenario.zones.Count > 0)
        {
            zoneButtonText1.text = currentScenario.zones[0].displayName;
            zoneButton1.onClick.RemoveAllListeners();
            zoneButton1.onClick.AddListener(() => PlaceSelectedToken(currentScenario.zones[0].zoneCode, currentScenario.zones[0].displayName));
        }
        if (currentScenario.zones.Count > 1)
        {
            zoneButtonText2.text = currentScenario.zones[1].displayName;
            zoneButton2.onClick.RemoveAllListeners();
            zoneButton2.onClick.AddListener(() => PlaceSelectedToken(currentScenario.zones[1].zoneCode, currentScenario.zones[1].displayName));
        }
        if (currentScenario.zones.Count > 2)
        {
            zoneButtonText3.text = currentScenario.zones[2].displayName;
            zoneButton3.onClick.RemoveAllListeners();
            zoneButton3.onClick.AddListener(() => PlaceSelectedToken(currentScenario.zones[2].zoneCode, currentScenario.zones[2].displayName));
        }
    }

    private void SelectToken(string tokenCode, string displayName)
    {
        selectedTokenCode = tokenCode;
        selectedTokenText.text = $"Đang chọn: {displayName}";
    }

    private void PlaceSelectedToken(string zoneCode, string zoneName)
    {
        if (string.IsNullOrEmpty(selectedTokenCode))
        {
            feedbackText.text = "Hãy chọn token trước.";
            return;
        }

        currentPlacements[selectedTokenCode] = zoneCode;
        feedbackText.text = $"Đã đặt {selectedTokenCode} vào {zoneName}.";
    }

    private void CheckPlacements()
    {
        List<string> errors = new List<string>();

        foreach (var correct in correctPlacements)
        {
            if (!currentPlacements.ContainsKey(correct.Key))
            {
                errors.Add($"Thiếu: {correct.Key}");
                continue;
            }

            if (currentPlacements[correct.Key] != correct.Value)
            {
                errors.Add($"{correct.Key} đang ở sai vị trí");
            }
        }

        if (errors.Count == 0)
        {
            feedbackText.text = "Chính xác! Bạn đã hoàn thành phần mô phỏng.";
        }
        else
        {
            feedbackText.text = "Chưa đúng:\n- " + string.Join("\n- ", errors);
        }
    }

    private void ResetGame()
    {
        selectedTokenCode = null;
        currentPlacements.Clear();
        selectedTokenText.text = "Đang chọn: chưa có";
        feedbackText.text = "Chọn token, đặt vào zone, rồi bấm Kiểm tra.";
    }
}