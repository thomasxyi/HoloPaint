using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaintableRegister : MonoBehaviour {
    public static Dictionary<int, int> paintableToNum = new Dictionary<int, int>();
    public static Dictionary<int, P3D_Paintable> numToPaintable = new Dictionary<int, P3D_Paintable>();
    public static int counter = 0;

    public static int register(P3D_Paintable paintable)
    {
        counter++;
        paintableToNum.Add(paintable.GetHashCode(), counter);
        numToPaintable.Add(counter, paintable);
        return counter;
    }

    public static P3D_Paintable getPaintable(int num)
    {
        return numToPaintable[num];
    }

    public static int getNum(P3D_Paintable paintable)
    {
        return paintableToNum[paintable.GetHashCode()];
    }
}
