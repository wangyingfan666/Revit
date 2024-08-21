using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfCenterWall;

namespace WpfWall815
{
    [Transaction(TransactionMode.Manual)]
    public class start : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Reter reter = new Reter();

            string targetLevelName = "F1";
            var Wall =SelectWall(targetLevelName,doc);
            string targetLevelName1 = "F2";

            foreach (var wall1 in Wall)
            {

                var floors = SelectFloor(targetLevelName1,doc);
                Floor nearestFloor = null;
                double minDistance = double.MaxValue;

                foreach (Floor floor in floors)
                {
                    BoundingBoxXYZ floorBoundingBox = floor.get_BoundingBox(null);
                    if (floorBoundingBox == null)
                        continue;

                    double floorBottomHeight = floorBoundingBox.Min.Z;

                    double distance = floorBottomHeight - reter.GetMaxHeight(wall1);

                    if (distance > 0 && distance < minDistance)
                    {
                        minDistance = distance;
                        nearestFloor = floor;
                    }
                }
                   using (Transaction tx = new Transaction(doc, "Modify Wall Height"))
                {
                    FailureHandlingOptions failure = tx.GetFailureHandlingOptions();
                    failure.SetFailuresPreprocessor(new FailurePreprocessor());
                    tx.SetFailureHandlingOptions(failure);


                    tx.Start();
                    double a = reter.GetMaxHeight(wall1);
                 

                    double nearestFloorBottomHeight = nearestFloor.get_BoundingBox(null).Min.Z;

                    var b = nearestFloorBottomHeight - a;
                    wall1.LookupParameter("顶部约束").Set(ElementId.InvalidElementId);

                    wall1.LookupParameter("无连接高度").Set(b + reter.GetWallHeight(wall1));

                    tx.Commit();
                }


            }
            watch.Stop();



            return Result.Succeeded;

        }
        public List<Wall> SelectWall(string targetLevelName, Document doc)
        {
            List<Wall> selectedWalls = new List<Wall>();


            FilteredElementCollector wallCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall)); // 收集所有墙对象

            // 收集所有楼层
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Level));
           

            // 根据名称查找目标楼层
            Level targetLevel = levelCollector
                .Cast<Level>()
                .FirstOrDefault(l => l.Name.Equals(targetLevelName));

            if (targetLevel == null)
            {
                TaskDialog.Show("Error", $"找不到指定的楼层: {targetLevelName}");
               
            }

            // 遍历所有墙体，检查每个墙体的所在楼层
            foreach (Wall wall in wallCollector)
            {
                // 获取墙体的基面
                Parameter levelParam = wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                if (levelParam != null && levelParam.HasValue)
                {
                    ElementId levelId = levelParam.AsElementId();
                    Level wallBaseLevel = doc.GetElement(levelId) as Level;

                    // 如果墙体的基面与目标楼层匹配，则将其添加到列表中
                    if (wallBaseLevel != null && wallBaseLevel.Id == targetLevel.Id)
                    {
                        selectedWalls.Add(wall);
                    }
                }
            
               
            }
            


            return selectedWalls;
        }
        public List<Floor> SelectFloor(String targetLevelName,Document doc)
        {
         List<Floor> floorsOnLevel = new List<Floor>();

            // 收集所有楼层元素
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Level));

            // 根据名称查找目标楼层
            Level targetLevel = levelCollector
                .Cast<Level>()
                .FirstOrDefault(l => l.Name.Equals(targetLevelName));

            if (targetLevel == null)
            {
                TaskDialog.Show("Error", $"找不到指定的楼层: {targetLevelName}");
                return floorsOnLevel; // 返回空列表
            }

            // 收集所有楼板元素
            FilteredElementCollector floorCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Floor));

            // 遍历所有楼板
            foreach (Floor floor in floorCollector)
            {
                // 获取楼板的基面所在楼层
                ElementId floorBaseLevel = floor.LevelId;

                // 如果楼板的基面与目标楼层匹配，则将其添加到列表中
                if (floorBaseLevel != null && floorBaseLevel == targetLevel.Id)
                {
                    floorsOnLevel.Add(floor);
                }
            }
          

            return floorsOnLevel;

        }

    }
}
