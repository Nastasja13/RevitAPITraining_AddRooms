using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitAPITraining_AddRooms
{
    [Transaction(TransactionMode.Manual)]
    public class AddRooms : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            ICollection<ElementId> rooms;
            int roomNum = 0;
            foreach (Level level in listLevel)
            {
                roomNum++;

                Transaction ts = new Transaction(doc, "Добавление помещений");
                ts.Start();
                rooms = doc.Create.NewRooms2(level);
                ts.Commit();

                Transaction ts2 = new Transaction(doc, "Добавление марок");
                ts.Start();


                foreach (ElementId roomid in rooms)
                {
                    Element e = doc.GetElement(roomid);
                    Room r = e as Room;

                    r.Name = $"{roomNum}_{r.Number}";
                    doc.Create.NewRoomTag(new LinkElementId(roomid), GetElementCenter(r), null);
                }
                ts.Commit();
            }
            return Result.Succeeded;
        }

        private UV GetElementCenter(Element e)
        {
            BoundingBoxXYZ bounding = e.get_BoundingBox(null);
            XYZ p = (bounding.Max + bounding.Min) / 2;
            UV center = new UV(p.X, p.Y);
            return center;
        }
    }
}
