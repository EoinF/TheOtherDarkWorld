using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld
{
    public enum DragAndDropType
    {
        None, //No handler is specified
        SwapElement, //Element 1's position is swapped with element 2's position
        InsertElement, //Element 1 is dropped in Element 2's position; Every element is pushed forward to make room
        DropElement //Element 1 is dropped anywhere
    }
}
