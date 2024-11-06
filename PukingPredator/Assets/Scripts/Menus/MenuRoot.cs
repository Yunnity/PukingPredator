using UnityEngine;
using UnityEngine.EventSystems;

public class MenuRoot : MonoBehaviour
{
    /// <summary>
    /// The object to start the menu on.
    /// </summary>
    [SerializeField]
    private GameObject firstSelected;



    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
