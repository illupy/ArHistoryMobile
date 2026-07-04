using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class AnswerButtonRestyler : EditorWindow
{
    [MenuItem("Tools/AR History/Restyle Answer Buttons")]
    public static void RestyleButtons()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "QuizScene")
        {
            Debug.LogWarning("Hãy mở QuizScene trước khi chạy script này.");
            return;
        }

        GameObject mainCard = GameObject.Find("Canvas/MainCard");
        if (mainCard == null)
        {
            Debug.LogError("Không tìm thấy Canvas/MainCard.");
            return;
        }

        GameObject managerGO = GameObject.Find("QuizSceneManager");
        if (managerGO == null)
        {
            Debug.LogError("Không tìm thấy QuizSceneManager.");
            return;
        }

        QuizSceneManager manager = managerGO.GetComponent<QuizSceneManager>();
        if (manager == null)
        {
            Debug.LogError("QuizSceneManager không có script QuizSceneManager.");
            return;
        }

        // Lấy thông tin UnitsToMove từ AnswerButton v2 mẫu
        float unitsToMove = -5f;
        GameObject sampleBtn = GameObject.Find("Canvas/MainCard/AnswerButton v2");
        if (sampleBtn != null)
        {
            var textPressed = sampleBtn.GetComponent<TextPressed>();
            if (textPressed != null)
            {
                unitsToMove = textPressed.UnitsToMove;
                Debug.Log($"Tìm thấy AnswerButton v2 mẫu. UnitsToMove = {unitsToMove}");
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Canvas/MainCard/AnswerButton v2 mẫu, dùng UnitsToMove = -5f mặc định.");
        }

        // Tải prefab premium dạng chữ nhật từ GUI Pro-FantasyRPG
        string prefabPath = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_Blue.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Không tải được prefab tại: {prefabPath}");
            return;
        }

        Undo.RegisterCompleteObjectUndo(mainCard, "Restyle Answer Buttons");
        Undo.RegisterCompleteObjectUndo(managerGO, "Restyle Answer Buttons");

        // Các vị trí Y và nhãn cũ
        float[] yPositions = new float[] { -1249f, -1109f, -969f, -829f };
        string[] labels = new string[] { "Đáp án 1", "Đáp án 2", "Đáp án 3", "Đáp án 4" };

        // Xóa các nút cũ trước (hoặc hủy liên kết)
        GameObject oldBtn1 = GameObject.Find("Canvas/MainCard/AnswerButton1");
        GameObject oldBtn2 = GameObject.Find("Canvas/MainCard/AnswerButton2");
        GameObject oldBtn3 = GameObject.Find("Canvas/MainCard/AnswerButton3");
        GameObject oldBtn4 = GameObject.Find("Canvas/MainCard/AnswerButton4");

        if (oldBtn1 != null) Undo.DestroyObjectImmediate(oldBtn1);
        if (oldBtn2 != null) Undo.DestroyObjectImmediate(oldBtn2);
        if (oldBtn3 != null) Undo.DestroyObjectImmediate(oldBtn3);
        if (oldBtn4 != null) Undo.DestroyObjectImmediate(oldBtn4);

        Button[] newButtons = new Button[4];
        TextMeshProUGUI[] newTexts = new TextMeshProUGUI[4];

        for (int i = 0; i < 4; i++)
        {
            int index = i + 1;
            // Tạo mới từ prefab
            GameObject newBtn = (GameObject)PrefabUtility.InstantiatePrefab(prefab, mainCard.transform);
            newBtn.name = $"AnswerButton{index}";

            // Thiết lập RectTransform
            RectTransform rt = newBtn.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(760f, 100f);
            rt.anchoredPosition = new Vector2(0f, yPositions[i]);

            // Thiết lập kiểu hiển thị Sliced để giữ nguyên bo góc tuyệt đẹp
            Image btnImg = newBtn.GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.type = Image.Type.Sliced;
                EditorUtility.SetDirty(btnImg);
            }

            // Xử lý Text child
            Transform textChild = newBtn.transform.Find("Text");
            if (textChild != null)
            {
                var oldText = textChild.GetComponent<Text>();
                string textVal = oldText != null ? oldText.text : labels[i];
                if (oldText != null) DestroyImmediate(oldText);

                var tmp = textChild.gameObject.AddComponent<TextMeshProUGUI>();
                tmp.text = textVal;
                tmp.fontSize = 32f;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;

                // Stretch Full
                RectTransform textRT = textChild.GetComponent<RectTransform>();
                textRT.anchorMin = Vector2.zero;
                textRT.anchorMax = Vector2.one;
                textRT.offsetMin = Vector2.zero;
                textRT.offsetMax = Vector2.zero;

                newTexts[i] = tmp;
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy child Text cho AnswerButton{index}");
            }

            // Thiết lập TextPressed
            var textPressed = newBtn.GetComponent<TextPressed>();
            if (textPressed != null)
            {
                textPressed.UnitsToMove = unitsToMove;

                // Gán private field 'gameObject' qua Reflection
                FieldInfo field = typeof(TextPressed).GetField("gameObject", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null && textChild != null)
                {
                    field.SetValue(textPressed, textChild.GetComponent<RectTransform>());
                }
            }

            newButtons[i] = newBtn.GetComponent<Button>();
        }

        // Gán lại tham chiếu trong QuizSceneManager
        manager.answerButton1 = newButtons[0];
        manager.answerButton2 = newButtons[1];
        manager.answerButton3 = newButtons[2];
        manager.answerButton4 = newButtons[3];

        manager.answerText1 = newTexts[0];
        manager.answerText2 = newTexts[1];
        manager.answerText3 = newTexts[2];
        manager.answerText4 = newTexts[3];

        EditorUtility.SetDirty(mainCard);
        EditorUtility.SetDirty(managerGO);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("Đã nâng cấp UI AnswerButton1 đến AnswerButton4 thành phiên bản AnswerButton v2 đẹp mắt và tương thích hoàn toàn!");
    }
}
