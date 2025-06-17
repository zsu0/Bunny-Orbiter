using UnityEngine;
using System.Collections.Generic; // Required for List

/// <summary>
/// ScriptableObject to define a single story/chapter in the game.
/// Contains general info and a list of pages for detailed playback.
/// </summary>
[CreateAssetMenu(fileName = "NewStoryData", menuName = "Game Data/Story Data")]
public class StoryData : ScriptableObject
{
    [Header("Story Core Information")]
    public string storyID; // A unique ID for this story (e.g., "story_ch001"). Used for PlayerPrefs.
    public string storyTitle; // The title displayed in the Story Collection list
    public Sprite storyIcon; // The icon displayed next to the title in the Story Collection list

    [Header("Unlock Status")]
    public bool isUnlockedByDefault = false; // Set to true if this story should always be available from the start

    [Header("Story Content (Pages)")]
    // This list will contain all the pages for this story, each with dialogue and a picture.
    public List<StoryPage> pages;

    /// <summary>
    /// Helper property to get all dialogue combined into one string for the Story Collection display.
    /// </summary>
    public string CombinedDialogueForCollection
    {
        get
        {
            string combined = "";
            // Iterate through all pages and append their dialogue text
            foreach (StoryPage page in pages)
            {
                if (!string.IsNullOrEmpty(page.dialogueText))
                {
                    combined += page.dialogueText.Trim() + "\n\n"; // Add new lines between pages for readability
                }
            }
            return combined.Trim(); // Remove any trailing newlines or spaces
        }
    }
}