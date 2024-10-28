using UWRandomizerEditor.LEV.GameObjects;

namespace UWRandomizerTools;

public interface IRandoSettings
{
    /// <summary>
    /// Specifies if an item should be moved, according to whatever internal rules the class has.
    /// </summary>
    /// <param name="obj">GameObject that's being evaluated</param>
    /// <returns></returns>
    public bool ShouldBeMoved(GameObject obj);
}