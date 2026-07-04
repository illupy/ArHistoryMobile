using System;
using System.Collections.Generic;

[Serializable]
public class GameScenarioResponse
{
    public long id;
    public long lessonId;
    public string title;
    public string instruction;
    public string templateType;
    public string backgroundImageUrl;
    public List<GameTokenResponse> tokens;
    public List<GameZoneResponse> zones;
    public List<GameRuleResponse> rules;
}

[Serializable]
public class GameTokenResponse
{
    public long id;
    public string tokenCode;
    public string displayName;
    public string iconUrl;
    public int orderIndex;
}

[Serializable]
public class GameZoneResponse
{
    public long id;
    public string zoneCode;
    public string displayName;
    public float posX;
    public float posY;
    public float width;
    public float height;
    public int orderIndex;
}

[Serializable]
public class GameRuleResponse
{
    public long id;
    public string tokenCode;
    public string correctZoneCode;
}