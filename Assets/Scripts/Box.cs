using System;
using UnityEditor;
using UnityEngine;

public class Box : MonoBehaviour
{
    private GameObject _fieldObject;
    private Vector2 _prevCursor;
    private Field _field;

    private DragMode _dragMode = DragMode.NO_DRAG;

    public Vector2Int gridPos;

    protected virtual void Awake()
    {
        _prevCursor = Vector2.negativeInfinity;
        _field = transform.parent.GetComponent<Field>();
        _fieldObject = transform.parent.gameObject;
    }

    protected virtual void OnMouseUp()
    {
        Vector2Int newGridPos = _field.WorldToGrid(transform.localPosition);
        _field.SwapBoxes(gridPos, newGridPos);
        _dragMode = DragMode.NO_DRAG;
    }

    protected virtual void OnMouseDown()
    {
        _prevCursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    protected virtual void OnMouseDrag()
    {
        Vector2 currentCursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector2 delta = currentCursor - _prevCursor;

        if (_dragMode == DragMode.NO_DRAG && delta.sqrMagnitude > 1e-6)
        {
            if (Math.Abs(delta.x) > Math.Abs(delta.y))
            {
                _dragMode = DragMode.X_DRAG;
            }
            else
            {
                _dragMode = DragMode.Y_DRAG;
            }
        }

        if (_dragMode == DragMode.X_DRAG) delta.y = 0;
        if (_dragMode == DragMode.Y_DRAG) delta.x = 0;

        Vector2 border = _field.clampPosition(_dragMode, gridPos);
        Vector3 pos = transform.position + (Vector3)delta;

        if (_dragMode == DragMode.X_DRAG) pos.x = Mathf.Clamp(pos.x, border.x, border.y);
        if (_dragMode == DragMode.Y_DRAG) pos.y = Mathf.Clamp(pos.y, border.x, border.y);
        
        transform.position = pos;

        _prevCursor = currentCursor;
    }
}
