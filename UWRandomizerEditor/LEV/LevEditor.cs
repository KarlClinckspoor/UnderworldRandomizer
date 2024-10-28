using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects;

namespace UWRandomizerEditor.LEV;

public class LevEditor
{
    private LevLoader ark;

    public LevEditor(LevLoader ark)
    {
        this.ark = ark;
    }

    // TOOD: supposed to return the index of the object
    public int AddItem(GameObject item, int LevelNum, Point Position, bool NormalizeHeight) { throw new NotImplementedException(); }
    public int AddItem(MobileObject mobj, int LevelNum, Point Position, bool NormalizeHeight) { throw new NotImplementedException(); }
    public int AddItem(StaticObject sobj, int LevelNum, Point Position, bool NormalizeHeight) { throw new NotImplementedException (); }

    // TODO: supposed to return the object it removed, or if the object itself is supplied, its index
    public GameObject RemoveItem(int level, uint slot) { throw new NotImplementedException(); }
    public GameObject RemoveItem(int level, Point position, int ID) { throw new NotImplementedException(); }
    public int RemoveItem(GameObject item, int level) { throw new NotImplementedException(); }
    public (int lvlNumber, int index) FindItem(GameObject item) { throw new NotImplementedException(); }
    public (int lvlNumber, int index, GameObject obj) FindItemByID(int id) { throw new NotImplementedException(); }

}
