using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Randomizer.LEVDotARK.GameObjects;
using Randomizer.LEVDotARK.GameObjects.Specifics;

namespace Randomizer.LEVDotARK;

public class UWLinkedList: IList<GameObject>
{
    public bool Initialized = false;

    private int _startingIdx;
    public int startingIdx
    {
        get
        {
            if (Initialized & objects.Count > 0)
                return objects[0].IdxAtObjectArray;
            if (Initialized & objects.Count == 0)
                return 0;
            // if (!Initialized)
            return _startingIdx;
        }
        set
        {
            if (Initialized & objects.Count > 0)
            {
                Debug.WriteLine("Attempting to change the starting index of an initialized UWLinkedList. This will clear the list");
                Clear();
            }

            if (Initialized & objects.Count == 0)
            {
                _startingIdx = value;
            }

            _startingIdx = value;
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
        if (objects.Count == 0)
        {
            startingIdx = item.IdxAtObjectArray;
        }
        else
        {
           objects[^1].next =  item.IdxAtObjectArray;
        }
        item.next = 0;
        Initialized = true;
        objects.Add(item);
    }

    public void Clear()
    {
        Initialized = false;
        _startingIdx = 0;
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
        int idx = objects.IndexOf(item);

        if (idx == -1)
        {
            return false; // Couldn't be found
        }

        if (idx == 0)
        {
            if (objects.Count == 1)
            {
                _startingIdx = 0;
                objects.RemoveAt(0);
            }
            else
            {
                objects.RemoveAt(0);
                _startingIdx = objects[0].IdxAtObjectArray;
            }
        }
        else if (idx == objects.Count - 1)
        {
            objects.RemoveAt(objects.Count - 1);
            objects[^1].next = 0;
        }
        else
        {
            objects[idx - 1].next = objects[idx + 1].IdxAtObjectArray;
            objects.RemoveAt(idx);
        }

        return true;
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
        item.next = objects[index + 1].IdxAtObjectArray;
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

        if (!Initialized)
        {
            return false;
        }
        if (objects.Count == 0)
        {
            if (startingIdx != 0)
            {
                return false;
            }

            return true;
        }
        
        if (startingIdx != objects[0].IdxAtObjectArray)
        {
            return false;
        }

        if (objects[^1].next != 0)
        {
            return false;
        }

        for (int i = 0; i < objects.Count - 1; i++)
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

    private void FixIntegrity()
    {
        int i = 0;
        foreach (var obj in objects)
        {
            if (i == 0) 
                _startingIdx = obj.IdxAtObjectArray;
            if (i == objects.Count - 1)
            {
                obj.next = 0;
            }
            else
            {
                obj.next = objects[i + 1].IdxAtObjectArray;
            }
            i++;
        }
    }

    public static bool CheckIntegrity(TileInfo tile, UWLinkedList linkedList)
    {
        if (tile.FirstObjIdx != linkedList.startingIdx)
        {
            return false;
        }

        return linkedList.CheckIntegrity();
    }

    public List<GameObject> PopObjectsThatShouldBeMoved()
    {
        var tempList = new List<GameObject>();
        foreach (var obj in objects)
        {
            if (obj.ShouldBeMoved)
            {
                tempList.Add(obj);
            }
        }

        foreach (var removedObject in tempList)
        {
            Remove(removedObject);
        }
        // FixIntegrity();
        return tempList;
    }
    
    /// <summary>
    /// Fills in the list of objects given a list of all GameObjects present in a level.
    /// </summary>
    /// <param name="AllBlockObjects">All game objects in the level, both static and mobile</param>
    public void PopulateObjectList(GameObject[] AllBlockObjects)
    {
        objects.Clear();
        if (startingIdx == 0) // Tile empty of objects
        {
            Initialized = true;
            return;
        }

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
            // objects.Add(obj);
            currentIdx = obj.next;
            Add(obj);
        }

        Initialized = true;
    }
    
    
}