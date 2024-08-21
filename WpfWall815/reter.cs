using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCenterWall
{
    public class Reter
    {
        public double GetMaxHeight(Wall wall)
        {
            BoundingBoxXYZ boundingBox = wall.get_BoundingBox(null);
            if (boundingBox != null)
            {
                return boundingBox.Max.Z;
            }
            return 0;
        }

        public double GetWallHeight(Wall wall)
        {
            BoundingBoxXYZ boundingBox = wall.get_BoundingBox(null);
            if (boundingBox != null)
            {
                return boundingBox.Max.Z - boundingBox.Min.Z;
            }
            return 0;
        }
    }
}
