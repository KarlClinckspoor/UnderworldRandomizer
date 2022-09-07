using Randomizer.LEVDotARK.GameObjects;

namespace Randomizer.LEVDotARK;

public class UWLinkedList
{
    public int startingIdx;
    private List<GameObject> objects;
    private int lastItemIdx = 0;

    /// <summary>
    /// Returns the GameObject at index `idx`
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public GameObject GetItemAt(int idx)
    {
        return objects[idx];
    }
    
    /// <summary>
    /// Add an item to the end of the linked list and updates the pointers
    /// </summary>
    /// <param name="item">Item to add at end of list</param>
    public void Add(GameObject item)
    {
        objects[^1].next =  item.IdxAtObjectArray;
        objects.Add(item);
    }

    /// <summary>
    /// Removes an entry from the list that's equal to the item provided.
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItemLike(GameObject item)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (item.Equals(objects[i]))
            {
                RemoveItemAt(i);
            }
        }
    }

    public GameObject RemoveItemAt(int position)
    {
        GameObject item = objects[position];
        
        if (position == 0)
        {
            startingIdx = objects[1].IdxAtObjectArray;
        }
        else if (position == objects.Count - 1)
        {
            objects[position - 1].next = 0;
        }
        else
        {
            objects[position - 1].next = objects[position + 1].IdxAtObjectArray;
        }

        return item;
    }
    
    /// <summary>
    /// Adds items from list to the start of the object chain
    /// </summary>
    /// <param name="items"></param>
    public void PrependItems(List<GameObject> items)
    {
        // var toAdd = new UWLinkedList(items);
        var oldObjectList = objects;
        objects = new List<GameObject>();

        AppendItems(items);
        AppendItems(oldObjectList);
        
        startingIdx = objects[0].IdxAtObjectArray;
    }
    
    /// <summary>
    /// Adds items from array to the start of the object chain
    /// </summary>
    /// <param name="items"></param>
    public void PrependItems(GameObject[] items)
    {
        PrependItems(items.ToList());
    }

    /// <summary>
    /// Adds items to the end of the object chain
    /// </summary>
    /// <param name="items"></param>
    public void AppendItems(List<GameObject> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }

        this.objects[^1].next = 0;
    }
    /// <summary>
    /// Adds items to the end of the object chain
    /// </summary>
    /// <param name="items"></param>
    public void AppendItems(GameObject[] items)
    {
        AppendItems(items.ToList());
    }


    /// <summary>
    /// Removes and returns the last object
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        int position = objects.Count - 1;
        var obj = GetItemAt(position);
        RemoveItemAt(position);
        return obj;
    }

    /// <summary>
    /// Checks if the sequence of objects is valid, i.e., the `next` fields all point to the actual next value
    /// </summary>
    /// <returns></returns>
    public bool CheckIntegrity()
    {
        if (startingIdx != objects[0].IdxAtObjectArray)
        {
            return false;
        }

        if (objects[^1].next != 0)
        {
            return false;
        }

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i].next != objects[i + 1].IdxAtObjectArray)
            {
                return false;
            }
        }

        return true;
    }

    public static bool CheckIntegrity(TileInfo tile, UWLinkedList linkedList)
    {
        if (tile.FirstObjIdx != linkedList.startingIdx)
        {
            return false;
        }

        return linkedList.CheckIntegrity();
    }

    public void FixIntegrity()
    {
        if (objects.Count == 0)
        {
            startingIdx = 0;
            return;
        }
        startingIdx = objects[0].IdxAtObjectArray;
        if (objects[^1].next != 0)
        {
            objects[^1].next = 0;
        }

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].next =  objects[i + 1].IdxAtObjectArray;
        }
    }
    
    public UWLinkedList(List<GameObject> objects)
    {
        this.objects = objects;
    }

    public UWLinkedList(GameObject[] objects)
    {
        this.objects = objects.ToList();
    }
}