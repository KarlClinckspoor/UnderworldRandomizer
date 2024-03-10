namespace UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

public class CreateObjectTrap : Trap
{
    public CreateObjectTrap(byte[] buffer, ushort idxAtObjArray, GameObject? objectTemplate = null) : base(buffer, idxAtObjArray)
    {
        _objectTemplate = objectTemplate;
        LinkSpecial = (ushort) (objectTemplate == null ? 0 : objectTemplate.IdxAtObjectArray);
    }

    private GameObject? _objectTemplate;
    
    public GameObject? GetCreatedObject()
    {
        return _objectTemplate;
    }
    
    public void SetTemplateToCreate(GameObject obj)
    {
        LinkSpecial = obj.IdxAtObjectArray;
        obj.ReferenceCount++;
        _objectTemplate = obj;
    }

    public void RemoveTemplateToCreate()
    {
        LinkSpecial = 0;
        if (_objectTemplate != null) _objectTemplate.ReferenceCount--;
    }

    private void SetProbability(byte number)
    {
        Quality = number;
    }

    public void SetProbability(uint prob)
    {
        if (prob < 0 | prob > 100)
        {
            throw new Exception("Can't have a probability greater than 100 or lower than 0!");
        }
        var number = (byte) (63 * prob / 100);
        SetProbability(number);
    }

    public uint GetProbability()
    {
        return (uint) (Quality * 100 / 63);
    }
    


}