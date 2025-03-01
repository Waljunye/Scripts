using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
	public virtual void OnCursorEnter() { }
    public virtual void OnCursorExit() { }
    public virtual void OnCursorSelectStart() { }
    public virtual void OnCursorSelectEnd() { }
}
