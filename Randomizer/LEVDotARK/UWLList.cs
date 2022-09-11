using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Net.Sockets;
using System.Text;
using Randomizer.LEVDotARK.GameObjects;

namespace Randomizer.LEVDotARK;

public class UWLinkedList: IList<GameObject>
{
    public bool initialized = false;
    public int startingIdx
    {
        get
        {
            if (objects.Count == 0)
            {
                return 0;
            }

            return objects[0].IdxAtObjectArray;
        }
    }

    private int lastItemIdx
    {
        get
        {
            if (objects.Count == 0)
            {
                return 0;
            }
            return objects[^1].IdxAtObjectArray;
        }
    }
    private List<GameObject> objects;

    public IEnumerator<GameObject> GetEnumerator()
    {
        return objects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(GameObject item)
    {
       objects[^1].next =  item.IdxAtObjectArray;
       item.next = 0;
       objects.Add(item);
    }

    public void Clear()
    {
        objects.Clear();
    }

    public bool Contains(GameObject item)
    {
        return objects.Contains(item);
    }

    public void CopyTo(GameObject[] array, int arrayIndex)
    {
        objects.CopyTo(array, arrayIndex);
    }

    public bool Remove(GameObject item)
    {
        return objects.Remove(item);
    }

    public int Count
    {
        get
        {
            return objects.Count;
        }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }
    public int IndexOf(GameObject item)
    {
        return objects.IndexOf(item);
    }

    public void Insert(int index, GameObject item)
    {
        if (index < 0 | index > objects.Count)
        {
            throw new IndexOutOfRangeException();
        }
        objects[index - 1].next = item.IdxAtObjectArray;
        objects.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 | index > Count - 1)
        {
            throw new IndexOutOfRangeException();
        }
        
        if (index == Count - 1)
        {
            objects[index - 1].next = 0;
        }
        else
        {
            objects[index - 1].next = objects[index + 1].IdxAtObjectArray;
        }
        objects.RemoveAt(index); // todo: Should this go in the beginning of the function?
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
    }

    public GameObject this[int index]
    {
        get
        {
            return objects[index];
        }
        set
        {
            objects[index - 1] = value;
            objects[index] = value;
            value.next = objects[index + 1].IdxAtObjectArray;
        }
    }

    public UWLinkedList()
    {
        objects = new List<GameObject>();
    }
     public UWLinkedList(List<GameObject> objects)
     {
         this.objects = objects;
     }

     public UWLinkedList(GameObject[] objects)
     {
         this.objects = objects.ToList();
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
        var obj = objects[position];
        RemoveAt(position);
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
    
    public static bool CheckIntegrity(TileInfo tile)
    {
        if (tile.FirstObjIdx != tile.ObjectChain.startingIdx)
        {
            return false;
        }

        return tile.ObjectChain.CheckIntegrity();
    }

    public static bool CheckIntegrity(TileInfo tile, UWLinkedList linkedList)
    {
        if (tile.FirstObjIdx != linkedList.startingIdx)
        {
            return false;
        }

        return linkedList.CheckIntegrity();
    }

    // public void FixIntegrity()
    // {
    //     if (objects.Count == 0)
    //     {
    //         startingIdx = 0;
    //         return;
    //     }
    //     startingIdx = objects[0].IdxAtObjectArray;
    //     if (objects[^1].next != 0)
    //     {
    //         objects[^1].next = 0;
    //     }
    //
    //     for (int i = 0; i < objects.Count; i++)
    //     {
    //         objects[i].next =  objects[i + 1].IdxAtObjectArray;
    //     }
    // }

    public List<GameObject> PopObjectsThatShouldBeMoved()
    {
        var tempList = new List<GameObject>();
        foreach (var obj in objects)
        {
            if (obj.ShouldBeMoved)
            {
                tempList.Add(obj);
                Remove(obj);
            }
        }

        return tempList;
    }
    
    
    /// <summary>
    /// Fills in the list of objects given a list of all GameObjects present in a level.
    /// </summary>
    /// <param name="AllBlockObjects">All game objects in the level, both static and mobile</param>
    public void PopulateObjectList(GameObject[] AllBlockObjects)
    {
        if (startingIdx == 0)
            return;

        int safetycounter = 0;
        int maxcounter = 1024;
        int currentIdx = startingIdx;
        while (currentIdx != 0) 
        {
            safetycounter++;
            if (safetycounter >= maxcounter)
            {
                throw new ConstraintException("Encountered potentially infinite loop when populating ObjectList!");
            }

            GameObject obj = AllBlockObjects[currentIdx];
            objects.Add(obj);
            currentIdx = obj.next;
        }

        initialized = true;
    }
    
    
}