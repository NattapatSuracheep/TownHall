using System.Collections.Generic;

public class VfxConstant
{
    public const string ClickVfx = "Click";

    public static IReadOnlyDictionary<string, string> ResourcesVfxDict = new Dictionary<string, string>()
    {
        {ClickVfx,$"Vfx/{ClickVfx}"}
    };
}