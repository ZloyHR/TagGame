using System;
using UnityEditor;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField]
    private GameObject _field;
    private Vector2 _prevCursor;

    public int xId;
    public int yId;

    private void Awake()
    {
        _prevCursor = Vector2.negativeInfinity;
    }

    private void OnMouseUp()
    {
        Field field = transform.parent.GetComponent<Field>();
        Vector3 pos = field.GridToWorld(field.WorldToGrid(transform.localPosition));
        pos.z = transform.localPosition.z;
        transform.localPosition = pos;
    }

    private void OnMouseDown()
    {
        _prevCursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        Vector2 currentCursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector2 delta = currentCursor - _prevCursor;
        transform.Translate(delta);

        _prevCursor = currentCursor;
    }

    private bool checkForCollision()
    {
        
        return true;
    }

    

}
