using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

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
            current.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
            _boxes[current.gridPos.x, current.gridPos.y] = current;
            
        }

        int id = Random.Range(0, cellCount * cellCount);
        var color = _boxes[id / cellCount, id % cellCount].GetComponent<SpriteRenderer>().color;
        color.a = 0;
        _boxes[id / cellCount, id % cellCount].GetComponent<SpriteRenderer>().color = color;

        Box temp = _boxes[id / cellCount, id % cellCount].gameObject.AddComponent<BoxEmpty>();
        Destroy(_boxes[id / cellCount, id % cellCount]);
        _boxes[id / cellCount, id % cellCount] = temp;
        temp.gridPos = new Vector2Int(id / cellCount, id % cellCount);
        
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        float scaleValue = _fieldSize / collider.size.x;
        transform.localScale = new Vector3(scaleValue,scaleValue,1);

        for (int i = 0; i < cellCount; ++i)
        {
            for (int j = 0; j < cellCount; ++j)
            {
                Vector3 pos = GridToWorld(new Vector2Int(i, j));
                pos.z = _boxes[i, j].transform.localPosition.z;
                _boxes[i, j].transform.localPosition = pos;
                collider = _boxes[i, j].GetComponent<BoxCollider2D>();
                scaleValue = cellSize / (collider.size.x * _fieldSize);
                _boxes[i, j].transform.localScale = new Vector3(scaleValue,scaleValue,1);
                
            }
        }
    }

    /**Converts array id into world position*/
    public Vector2 GridToWorld(Vector2Int id)
    {
        Vector2 pos;
        pos.x = -(_fieldSize / 2) + id.x * cellSize + (id.x + 1) * spaceSize + cellSize / 2;
        pos.x /= _fieldSize;
        pos.y = -(_fieldSize / 2) + id.y * cellSize + (id.y + 1) * spaceSize + cellSize / 2;
        pos.y /= -_fieldSize;
        return pos;
    }
    
    public Vector2Int WorldToGrid(Vector2 pos)
    {
        Vector2Int id = new Vector2Int(0,0);
        float dist = float.MaxValue;
        for (int i = 0; i < cellCount; ++i)
        {
            for (int j = 0; j < cellCount; ++j)
            {
                Vector2 cur = GridToWorld(new Vector2Int(i, j));
                if (dist > (cur - pos).sqrMagnitude)
                {
                    dist = (cur - pos).sqrMagnitude;
                    id = new Vector2Int(i,j);
                }
            }
        }
        
        return id;
    }

    public void SwapBoxes(Vector2Int firstID, Vector2Int secondID)
    {
        Vector3 firstPos = GridToWorld(firstID),secondPos = GridToWorld(secondID);
        Box firstBox = _boxes[firstID.x, firstID.y],secondBox = _boxes[secondID.x, secondID.y];
        firstPos.z = firstBox.transform.localPosition.z;
        secondPos.z = secondBox.transform.localPosition.z;
        firstBox.transform.localPosition = secondPos;
        secondBox.transform.localPosition = firstPos;
        firstBox.gridPos = secondID;
        secondBox.gridPos = firstID;
        _boxes[firstID.x, firstID.y] = secondBox;
        _boxes[secondID.x, secondID.y] = firstBox;

    }

    public Vector2 clampPosition(DragMode mode,Vector2Int id)
    {
        Bounds fieldBounds = GetComponent<BoxCollider2D>().bounds;
        Bounds boxBounds = _boxes[0,0].GetComponent<BoxCollider2D>().bounds;
        
        Vector2 border = Vector2.zero;
        
        if (mode == DragMode.X_DRAG)
        {
            border = new Vector2(fieldBounds.min.x, fieldBounds.max.x);
            for (int i = id.x - 1; i >= 0; --i)
            {
                if (_boxes[i, id.y].GetType() == typeof(BoxEmpty)) continue;
                boxBounds = _boxes[i,id.y].GetComponent<BoxCollider2D>().bounds;
                border.x = boxBounds.max.x;
                break;
            }
            
            for (int i = id.x + 1; i < cellCount; ++i)
            {
                if (_boxes[i, id.y].GetType() == typeof(BoxEmpty)) continue;
                boxBounds = _boxes[i,id.y].GetComponent<BoxCollider2D>().bounds;
                border.y = boxBounds.min.x;
                break;
            }
        }
        
        if (mode == DragMode.Y_DRAG)
        {
            border = new Vector2(fieldBounds.min.y, fieldBounds.max.y);
            for (int i = id.y - 1; i >= 0; --i)
            {
                if (_boxes[id.x, i].GetType() == typeof(BoxEmpty)) continue;
                boxBounds = _boxes[id.x,i].GetComponent<BoxCollider2D>().bounds;
                border.y = boxBounds.min.y;
                break;
            }
            
            for (int i = id.y + 1; i < cellCount; ++i)
            {
                if (_boxes[id.x, i].GetType() == typeof(BoxEmpty)) continue;
                boxBounds = _boxes[id.x,i].GetComponent<BoxCollider2D>().bounds;
                border.x = boxBounds.max.y;
                break;
            }
        }

        border.x += boxBounds.size.x / 2;
        border.y -= boxBounds.size.x / 2;

        return border;
    }
}
    
    
