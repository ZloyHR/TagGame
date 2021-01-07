using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Field : MonoBehaviour
{
    private Box[,] _boxes;

    public int cellCount;
    public float cellSize;
    public float spaceSize;

    private float _fieldSize;

    private void Awake()
    {
        _boxes = new Box[cellCount,cellCount];
        _fieldSize = cellSize * cellCount + spaceSize * (cellCount + 1);
        

        foreach (Transform child in transform)
        {
            Box current = child.GetComponent<Box>();
            _boxes[current.xId, current.yId] = current;
        }

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        float scaleValue = _fieldSize / collider.size.x;
        transform.localScale = new Vector3(scaleValue,scaleValue,1);

        for (int i = 0; i < cellCount; ++i)
        {
            for (int j = 0; j < cellCount; ++j)
            {
                Vector3 pos = GridToWorld(new Vector2(i, j));
                pos.z = _boxes[i, j].transform.localPosition.z;
                _boxes[i, j].transform.localPosition = pos;
                collider = _boxes[i, j].GetComponent<BoxCollider2D>();
                scaleValue = cellSize / (collider.size.x * _fieldSize);
                _boxes[i, j].transform.localScale = new Vector3(scaleValue,scaleValue,1);
                
            }
        }
    }

    /**Converts array id into world position*/
    public Vector2 GridToWorld(Vector2 id)
    {
        Vector2 pos;
        pos.x = -(_fieldSize / 2) + id.x * cellSize + (id.x + 1) * spaceSize + cellSize / 2;
        pos.x /= _fieldSize;
        pos.y = -(_fieldSize / 2) + id.y * cellSize + (id.y + 1) * spaceSize + cellSize / 2;
        pos.y /= -_fieldSize;
        return pos;
    }
    
    public Vector2 WorldToGrid(Vector2 pos)
    {
        Vector2 id = Vector2.zero;
        float dist = float.MaxValue;
        for (int i = 0; i < cellCount; ++i)
        {
            for (int j = 0; j < cellCount; ++j)
            {
                Vector2 cur = GridToWorld(new Vector2(i, j));
                if (dist > (cur - pos).sqrMagnitude)
                {
                    dist = (cur - pos).sqrMagnitude;
                    id = new Vector2(i,j);
                }
            }
        }
        
        return id;
    }
}
    
    
