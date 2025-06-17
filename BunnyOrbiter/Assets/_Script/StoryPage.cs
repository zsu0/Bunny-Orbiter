using UnityEngine;
using System; // Required for [Serializable]

/// <summary>
/// Represents a single page of a story with its dialogue and a corresponding picture.
/// </summary>
[Serializable] // This attribute makes the struct visible and editable in the Unity Inspector
public struct StoryPage
{
    [TextArea(3, 10)] // Makes the string field a multi-line text area in the Inspector
    public string dialogueText; // The dialogue for this specific page
    public Sprite pagePicture;  // The picture associated with this page's dialogue
}