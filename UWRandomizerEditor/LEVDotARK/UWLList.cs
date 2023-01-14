using System.Collections;
using System.Data;
using System.Diagnostics;
using UWRandomizerEditor.LEVDotARK.GameObjects;

namespace UWRandomizerEditor.LEVDotARK;

public class UWLinkedList: IList<GameObject>
{
    private int _startingIdx = 0;
    public int startingIdx
    {
        get
        {
            if (objects.Count > 0)
            {
                // In case this isn't true, the LList is invalid, so it's good to check now
                Debug.Assert(_startingIdx == objects[0].IdxAtObjectArray);
                return _startingIdx;
            }
            return _startingIdx;
        }
        set
        {
            if (objects.Count > 0)
            {
                Debug.WriteLine("Attempting to change the starting index of an initialized UWLinkedList. This will clear the list");
                Clear();
            }
            _startingIdx = value;
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
        objects.Add(item);
    }

    public void Clear()
    {
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

    /// <summary>
    /// Inserts an object and its new position in the LinkedList is the index provided.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public void Insert(int index, GameObject item)
    {
        if (index < 0 | index > Count)
        {
            throw new IndexOutOfRangeException();
        }

        if (index == 0)
        {
            _startingIdx = item.IdxAtObjectArray;
            item.next = objects[0].IdxAtObjectArray;
            objects.Insert(index, item);
        }
        else if (index == Count)
        {
            objects[^1].next = item.IdxAtObjectArray;
            item.next = 0;
            objects.Insert(index, item);
        }
        else
        {
            objects[index - 1].next = item.IdxAtObjectArray;
            item.next = objects[index].IdxAtObjectArray;
            objects.Insert(index, item);
        }
        
    }

    public void RemoveAt(int index)
    {
        if (index < 0 | index > Count - 1)
        {
            throw new IndexOutOfRangeException();
        }

        if (index == 0 & Count == 1)
        {
            objects.RemoveAt(0);
            _startingIdx = 0;
            return;
        }

        if (index == 0 & Count > 1)
        {
            objects.RemoveAt(0);
            _startingIdx = objects[0].IdxAtObjectArray;
            return;
        }

        if (index == Count - 1)
        {
            objects[index - 1].next = 0;
            objects.RemoveAt(index);
            return;
        }
        
        objects[index - 1].next = objects[index + 1].IdxAtObjectArray;
        objects.RemoveAt(index);
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
            if (index < 0 | index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            return objects[index];
        }
        set
        {
            if (index < 0 | index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == 0 & Count > 1)
            {
                _startingIdx = value.IdxAtObjectArray;
                value.next = objects[index + 1].IdxAtObjectArray;
                objects[index] = value;
                return;
            }

            if (index == 0 & Count == 1)
            {
                _startingIdx = value.IdxAtObjectArray;
                value.next = 0;
                objects[0] = value;
                return;
            }

            if (index == Count - 1)
            {
                value.next = 0;
                objects[index] = value;
                objects[index - 1].next = value.IdxAtObjectArray;
                return;
            }
            
            objects[index - 1].next = value.IdxAtObjectArray;
            value.next = objects[index + 1].IdxAtObjectArray;
            objects[index] = value;
        }
    }

    /// <summary>
    /// Constructed a new empty UWLinkedList. To fill it, either set the starting index and provide a
    /// list of GameObjects, or add the objects individually.
    /// </summary>
    public UWLinkedList()
    {
        objects = new List<GameObject>();
    }
    /// <summary>
    /// Creates a UWLinkedList containing the provided list of objects.
    /// </summary>
    /// <param name="objectsToBeInTheList"></param>
    public UWLinkedList(List<GameObject> objectsToBeInTheList, short firstObjectIndex)
    {
        _startingIdx = firstObjectIndex;
        objects = objectsToBeInTheList;
        Debug.WriteLineIf(!CheckIntegrity(), "Added list of objects isn't valid!");
    }

    public UWLinkedList(GameObject[] objectsToBeInTheList, short firstObjectIndex)
    {
        _startingIdx = firstObjectIndex;
        this.objects = objectsToBeInTheList.ToList();
        Debug.WriteLineIf(!CheckIntegrity(), "Added list of objects isn't valid!");
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
        if (Count == 0)
        {
            throw new InvalidOperationException("Can't pop from an empty Linked List!");
        }
        int position = objects.Count - 1;
        var obj = objects[position];
        RemoveAt(position);
        return obj;
    }

    /// <summary>
    /// Checks if the sequence of objects is valid.
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///         The starting index is equal to the first GameObject's index (except if there's no objects, then it has to be 0)
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///         The last object's <c>next</c> field should be 0.
    ///     </description>
    ///     </item>
    ///     <item>
    ///         Every intermediate GameObjects <c>next</c> points to the following GameObject's index. If there's duplicates,
    ///         then this will make the list invalid immediately.
    ///     </item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public bool CheckIntegrity()
    {
        foreach (var obj in objects)
        {
            if (obj.IdxAtObjectArray == 0)
            {
                return false;
            }
        }
        
        if (objects.Count == 0)  // Empty list, so the starting index has to be 0
        {
            if (_startingIdx != 0) // TODO: _startingIdx or startingIdx?
            {
                return false;
            }

            return true;
        }
        
        if (_startingIdx != objects[0].IdxAtObjectArray)
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
    
    private void ForceFixIntegrity()
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

    
    /// <summary>
    /// Fills in the list of objects given a list of all GameObjects present in a level.
    /// </summary>
    /// <param name="AllBlockObjects">All game objects in the level, both static and mobile</param>
    public void PopulateObjectList(GameObject[] AllBlockObjects)
    {
        objects.Clear();
        if (startingIdx == 0) // Tile empty of objects
        {
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

    }

    public void PopulateObjectList(List<GameObject> Objects)
    {
        PopulateObjectList(Objects.ToArray());
    }
    
    
}