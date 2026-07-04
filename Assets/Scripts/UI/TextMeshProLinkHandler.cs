using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProLinkHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI m_TextMeshPro;
    public System.Action<string> onLinkClick;

    private void Awake()
    {
        m_TextMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            onLinkClick?.Invoke(linkId);
        }
    }
}
