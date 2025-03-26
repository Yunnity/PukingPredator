using UnityEngine;

public class NoteImage : Note
{
    /// <summary>
    /// Max sizes of the image.
    /// </summary>
    [SerializeField]
    private float height = 36f;

    /// <summary>
    /// The image shown by the note.
    /// </summary>
    [SerializeField]
    private Sprite sprite;



    protected override void Awake()
    {
        base.Awake();
        _ = AddImage(sprite, height);
    }
}
