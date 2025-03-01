using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType
{
    Default,
    Sacrifice,
    Place,
    Fight,
}

public class Cursor3D : MonoBehaviour
{
    public static Cursor3D instance;

    public Interactable CurrentInteractable { get { return currentInteractable; } }
    private Interactable currentInteractable;

    [SerializeField]
    private Camera rayCamera;

    [SerializeField]
    private List<Texture2D> cursorTextures;

    [SerializeField]
    private List<Texture2D> cursorDownTextures;

    private CursorType cursorType = CursorType.Default;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetCursorType(CursorType.Default);
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    void Update()
    {

        SetCurrentInteractable();

        //Vector2 mousePos = Input.mousePosition;
        //transform.position = rayCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, rayCamera.nearClipPlane));

        HandleClicks();
    }

    private void SetCurrentInteractable()
    {
        Interactable hitInteractable = null;

        RaycastHit rayHit;
        Ray ray = rayCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit, 1000f))
        {
            hitInteractable = rayHit.transform.GetComponent<Interactable>();
        }

        if (hitInteractable != currentInteractable)
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnCursorExit();
            }
            if (hitInteractable != null)
            {
                hitInteractable.OnCursorEnter();
            }
        }

        currentInteractable = hitInteractable;
    }

    public void SetCursorType(CursorType type)
    {
        cursorType = type;
    }

    private void SetCursorDown(bool down)
    {
        Vector2 hotSpot = new Vector2(60f, 60f);
        if (cursorType == CursorType.Sacrifice)
        {
            hotSpot = new Vector2(60f, 100f);
        }

        Cursor.SetCursor(down ? cursorDownTextures[(int)cursorType] : cursorTextures[(int)cursorType], hotSpot, CursorMode.ForceSoftware);

    }

    private void HandleClicks()
    {
        SetCursorDown(Input.GetButton("Select"));

        if (currentInteractable != null)
        {
            if (Input.GetButtonDown("Select"))
            {
                currentInteractable.OnCursorSelectStart();
            }
            else if (Input.GetButtonUp("Select"))
            {
                currentInteractable.OnCursorSelectEnd();
            }
        }
    }
}
