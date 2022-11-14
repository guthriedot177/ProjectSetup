#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RevitAddinBootcamp
{
    [Transaction(TransactionMode.Manual)]
    public class CmdProjectSetup : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;


            // read text file data
            string filepathS = @"C:\Users\guthrie.177\Documents\Visual Studio\RAB_Session_02_Challenge_Sheets.csv";
            string fileTextS = System.IO.File.ReadAllText(filepathS);
            string[] fileArrayS = System.IO.File.ReadAllLines(filepathS);
           

            string filepathL = @"C:\Users\guthrie.177\Documents\Visual Studio\RAB_Session_02_Challenge_Levels.csv";
            string fileTextL = System.IO.File.ReadAllText(filepathL);
            string[] fileArrayL = System.IO.File.ReadAllLines(filepathL);
          // RemoveAt doesn't seem to work with arrays, only lists, but I couldn't get the setup correct to convert to a list 

           

            TaskDialog.Show("Levels", "Creating levels from file");

            Transaction tL = new Transaction(doc);
            tL.Start("Create Levels");

            foreach (string rowStringL in fileArrayL)
            {
                
                string[] cellStringL = rowStringL.Split(',');
  

                string levelName = cellStringL[0];
                string levelHeight = cellStringL[1];
               // double levelHeightDouble = cellStringL.ToString;

              // double levelHeightDouble = double.Parse(levelHeight);
              // The regular parse didn't work because of the header information, which wasn't a number

             // TryParse has 2 outcomes, the true/false part and the "out" part, which in this case is the double
               double levelHeightDouble = 0;
               bool didItParse = double.TryParse(levelHeight, out levelHeightDouble);

              // create level
               Level myLevel = Level.Create(doc, levelHeightDouble);
               myLevel.Name = levelName;
            }

            tL.Commit();
            tL.Dispose();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            ElementId tblockId = collector.FirstElementId();

            Transaction tS = new Transaction(doc);
            tS.Start("Create sheets");

            TaskDialog.Show("Sheets", "Creating sheets from file");

            foreach (string rowStringS in fileArrayS)
            {
                string[] cellStringS = rowStringS.Split(',');

                string sheetNumber = cellStringS[0];
                string sheetName = cellStringS[1];

                // create sheet
                ViewSheet mySheet = ViewSheet.Create(doc, tblockId);
                mySheet.Name = sheetName;
                mySheet.SheetNumber = sheetNumber;
            }

            tS.Commit();
            tS.Dispose();


            return Result.Succeeded;
        }

    }
}