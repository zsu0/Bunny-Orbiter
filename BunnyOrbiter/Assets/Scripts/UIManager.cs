using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI Themes")]
    public UITheme lightTheme;
    public UITheme darkTheme;
    
    [Header("Common UI Elements")]
    public List<Image> backgroundImages;
    public List<Text> texts;
    public List<Button> buttons;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        ApplyTheme();
    }
    
    public void ApplyTheme()
    {
        UITheme currentTheme = GameManager.Instance.isDarkMode ? darkTheme : lightTheme;
        
        foreach (var bg in backgroundImages)
        {
            if (bg != null) bg.color = currentTheme.backgroundColor;
        }
        
        foreach (var text in texts)
        {
            if (text != null) text.color = currentTheme.textColor;
        }
        
        foreach (var button in buttons)
        {
            if (button != null)
            {
                var colors = button.colors;
                colors.normalColor = currentTheme.buttonColor;
                colors.highlightedColor = currentTheme.buttonHighlightColor;
                button.colors = colors;
            }
        }
    }
}

[System.Serializable]
public class UITheme
{
    public Color backgroundColor = Color.white;
    public Color textColor = Color.black;
    public Color buttonColor = Color.gray;
    public Color buttonHighlightColor = Color.white;
}