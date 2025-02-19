using UnityEngine;

public class NoteImage : Note
{
    /// <summary>
    /// The image shown by the note.
    /// </summary>
    [SerializeField]
    private Sprite sprite;

    /// <summary>
    /// Max sizes of the image.
    /// </summary>
    [SerializeField]
    private Vector2 size = new Vector2(36f, 36f);



    protected override void Awake()
    {
        base.Awake();
        _ = AddImage(sprite, size);
    }
}
