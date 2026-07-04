using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTime : LevelCondition
{
    private float m_time;
    private GameManager m_mngr;
    private int m_lastSeconds = -1;

    public override void Setup(float value, Text txt, GameManager mngr)
    {
        base.Setup(value, txt, mngr);

        m_mngr = mngr;

        m_time = value;
        m_lastSeconds = -1;

        UpdateText();
    }

    private void Update()
    {
        if (m_conditionCompleted) return;

        if (m_mngr.State != GameManager.eStateGame.GAME_STARTED) return;

        m_time -= Time.deltaTime;

        UpdateText();

        if (m_time <= -1f)
        {
            OnConditionComplete();
        }
    }

    protected override void UpdateText()
    {
        if (m_time < 0f) return;

        int currentSeconds = Mathf.CeilToInt(m_time);
        if (currentSeconds != m_lastSeconds)
        {
            m_txt.text = string.Format("TIME:\n{0:00}", currentSeconds);
            m_lastSeconds = currentSeconds;
        }
    }
}
