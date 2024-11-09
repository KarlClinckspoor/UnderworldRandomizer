using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;
using UWRandomizerEditor.LEV.GameObjects;

namespace UWRandomizerEditor.LEV;

/// <summary>
/// A UWLinkedList is used to keep items that belong to a tile, container or npc updated as items are added or removed
/// to those objects. It technically does not have an equivalent in the binary file of LEV.ARK, where these relationships
/// are all indirect.
/// Under the hood, it's just a list of GameObjects that updates them when items are added/removed.
/// </summary>
public class UWLinkedList: IList<GameObject>
{
    private uint _startingIdx;
    
    /// <summary>
    /// This variable dictates if this UWLinkedList instance is representing a container, i.e., the items aren't free
    /// on the ground. A container can be either a bag or similar, or a MobileObject (npc). By default, this is false.
    /// </summary>
    public bool RepresentingContainer = false;
    
    /// <summary>
    /// Represents the index of the first element in the LinkedList, which is important because it's what a Tile or
    /// a container references.
    /// </summary>
    public uint StartingIdx
    {
        get
        {
            if (objects.Count == 0) return _startingIdx;
            // In case this isn't true, the LList is invalid, so it's good to check now
            Debug.Assert(_startingIdx == objects[0].IdxAtObjectArray);
            return _startingIdx;
        }
        set
        {
            if (objects.Count > 0)
            {
                if (value != _startingIdx)
                {
                    Debug.WriteLine("Attempting to change the starting index of an initialized UWLinkedList. This will clear the list");
                    Clear();
                } // Else, don't do anything meaningful.
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

    /// <summary>
    /// Adds an item to the end of the linked list. Updates the item's "next" field to 0.
    /// </summary>
    /// <param name="item"></param>
    public void Add(GameObject item)
    {
        if (item.ReferenceCount >= 1)
        {
            // throw new UWLinkedListException(
            //     $"Object {item} has been added elsewhere before (reference count={item.ReferenceCount}");
            Console.WriteLine($"Object {item} has been added elsewhere before (refcount={item.ReferenceCount})!");
        }
        if (objects.Count == 0)
        {
            StartingIdx = item.IdxAtObjectArray;
        }
        else
        {
            objects[^1].next =  item.IdxAtObjectArray;
        }
        item.next = 0;
        item.InContainer = RepresentingContainer;
        objects.Add(item);
        item.ReferenceCount += 1;
    }


    /// <inheritdoc cref="ICollection{T}.Clear"/>
    /// Also sets the starting index to 0.
    public void Clear()
    {
        _startingIdx = 0;
        foreach (var obj in objects)
        {
            obj.ReferenceCount -= 1;
        }
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

    /// <inheritdoc cref="ICollection{T}.Remove"/>
    /// In addition, updates the indexes of all appropriate GameObjects.
    public bool Remove(GameObject item)
    {
        int idx = objects.IndexOf(item);

        if (idx == -1)
        {
            // TODO: Should I raise an error?
            return false; // Couldn't be found
        }

        objects[idx].ReferenceCount -= 1;

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

    public int Count => objects.Count;

    public bool IsReadOnly => false;

    public int IndexOf(GameObject item)
    {
        return objects.IndexOf(item);
    }

    /// <inheritdoc cref="IList{T}.Insert"/>
    /// Fixes the indexes of GameObjects around the insertion point.
    public void Insert(int index, GameObject item)
    {
        if (index < 0 | index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        item.ReferenceCount += 1;

        if (index == 0)
        {
            _startingIdx = item.IdxAtObjectArray;
            item.InContainer = RepresentingContainer;
            item.next = objects[0].IdxAtObjectArray;
            objects.Insert(index, item);
        }
        else if (index == Count)
        {
            objects[^1].next = item.IdxAtObjectArray;
            item.next = 0;
            item.InContainer = RepresentingContainer;
            objects.Insert(index, item);
        }
        else
        {
            objects[index - 1].next = item.IdxAtObjectArray;
            item.next = objects[index].IdxAtObjectArray;
            item.InContainer = RepresentingContainer;
            objects.Insert(index, item);
        }
        
    }

    /// <inheritdoc cref="IList{T}.RemoveAt"/>
    /// Updates the GameObjects surrounding the object.
    public void RemoveAt(int index)
    {
        if (index < 0 | index > Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        objects[index].ReferenceCount -= 1;
        
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
    /// Instantiates a new internal List of GameObjects, appends the items provided, then appends the original items.
    /// </summary>
    /// <param name="items">List of items that will be prepended.</param>
    public void PrependItems(List<GameObject> items)
    {
        var oldObjectList = new List<GameObject>();
        oldObjectList.AddRange(objects);
        Clear();

        AppendItems(items);
        AppendItems(oldObjectList);
    }

    /// <inheritdoc cref="IList{T}.this"/>
    /// When setting, adjusts the GameObjects around the insertion point to accomodate it.
    public GameObject this[int index]
    {
        get
        {
            if (index < 0 | index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return objects[index];
        }
        set
        {
            if (index < 0 | index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            objects[index].ReferenceCount -= 1;
            value.ReferenceCount += 1;

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
    /// Constructs a new empty UWLinkedList. To fill it, either set the starting index and provide a
    /// list of GameObjects, or add the objects individually.
    /// </summary>
    public UWLinkedList()
    {
        objects = new List<GameObject>();
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
    /// Checks if the the UWLinkedList has a valid sequence of objects according to these rules:
    /// <list type="number">
    /// 
    ///     <item>
    ///         <description>
    ///             If any of the objects has index 0 (reserved), or if an object is referenced 0 times (library mistake)
    ///             or more than 1 time (library or lev.ark mistake), then the list is invalid.
    ///         </description>
    ///     </item>
    /// 
    ///     <item>
    ///         <description>
    ///             If the list is empty, the starting index should be 0. It's invalid otherwise.
    ///         </description>
    ///     </item>
    /// 
    ///     <item>
    ///         <description>
    ///             The starting index is equal to the first GameObject's index (except if there's no objects, then it has to be 0)
    ///         </description>
    ///     </item>
    /// 
    ///     <item>
    ///         <description>
    ///             The last object's <c>next</c> field should be 0.
    ///         </description>
    ///     </item>
    ///
    ///     <item>
    ///         <description>
    ///             Every intermediate GameObjects <c>next</c> points to the following GameObject's index. If there's duplicates,
    ///             then this will make the list invalid immediately.
    ///         </description>
    ///     </item>
    /// </list>
    /// </summary>
    /// 
    /// <returns>True if list is sound, False if not</returns>
    public bool CheckIntegrity()
    {
        foreach (var obj in objects)
        {
            if ((obj.IdxAtObjectArray == 0) | (obj.ReferenceCount != 1))
            {
                return false;
            }
        }
        
        if (objects.Count == 0)  // Empty list, so the starting index has to be 0
        {
            if (_startingIdx != 0)
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
    
    /// <summary>
    /// This function goes over all the objects and sets the internal state of UWLinkedList to a valid state.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if any of the objects has index 0 or if the list fails <see cref="CheckIntegrity"/>
    /// </exception>
    private void ForceFixIntegrity()
    {
        if(objects.Any(x => x.IdxAtObjectArray == 0))
            throw new InvalidOperationException("There are objects with IdxAtObjectArray equal to 0. Impossible to fix without altering the list");
        
        int i = 0;

        if (objects.Count == 0)
        {
            _startingIdx = 0;
            return;
        }
        
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

        if (!CheckIntegrity())
        {
            throw new InvalidOperationException("ForceFixIntegrity couldn't fix the integrity!");
        }
    }

    
    /// <summary>
    /// Fills in the list of objects given a list of all GameObjects present in a level.
    /// </summary>
    /// <param name="allBlockObjects">All game objects in the level, both static and mobile</param>
    public void PopulateObjectList(IList<GameObject> allBlockObjects)
    {
        objects.Clear();
        if (StartingIdx == 0) // Tile or container without any objects.
        {
            return;
        }

        int safetyCounter = 0;
        int maxCounter = 1024;
        uint currentIdx = StartingIdx;
        while (currentIdx != 0) 
        {
            safetyCounter++;
            if (safetyCounter >= maxCounter)
            {
                throw new ConstraintException("Encountered potentially infinite loop when populating ObjectList!");
            }

            GameObject obj = allBlockObjects[(int) currentIdx];
            currentIdx = obj.next;
            Add(obj);
        }

    }
    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var obj in objects)
        {
            builder.Append($"{obj.ItemID} (idx={obj.IdxAtObjectArray});");
        }
        return builder.ToString();
    }
}