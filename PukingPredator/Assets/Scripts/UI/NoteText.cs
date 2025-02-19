using UnityEngine;

public class NoteText : Note
{
    /// <summary>
    /// The text shown by the note.
    /// </summary>
    [SerializeField]
    private string text;



    protected override void Awake()
    {
        base.Awake();
        _ = AddText(text);
    }
}
