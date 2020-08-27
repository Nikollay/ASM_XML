using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace ASM_XML
{
    class Program
    {
        static void Main(string[] args)
        {
            var progId = "SldWorks.Application.27";

            var progType = System.Type.GetTypeFromProgID(progId);

            var swApp = System.Activator.CreateInstance(progType) as ISldWorks;
            swApp.Visible = false;
            Console.WriteLine(swApp.RevisionNumber());
            Console.CursorSize = 100;
            ModelDoc2 swModel;
            ModelDocExtension swModelDocExt;
            AssemblyDoc swAssy;
            Component2 swComp;
            DrawingDoc Part;
            List<Comp> coll;

            int errors = 0;
            int warnings = 0;
            string fileName;   // GetOpenFileName
            Dictionary<string, string> Dict, Drw;
            string projekt_path, key, pathName;
            string[] сonfNames;
            object[] Comps;

            fileName = swApp.GetOpenFileName("File to SLDASM", "", "SLDASM Files (*.SLDASM)|*.SLDASM|", out _, out _, out _);
            //Проверяем путь
            if (fileName == "")
            {
                swApp.ExitApp();
                return;
            }
            swModel = (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

            //Проверяем открыта сборка или нет
            if ((swModel.GetType() != 2) | (swModel == null))
            {
                swApp.SendMsgToUser2("Откройте сборку", 4, 2);
                swApp.ExitApp();
                return;
            }
            swAssy = (AssemblyDoc)swModel;
            coll=Comp.GetColl(swAssy, (SldWorks)swApp);
        }
    }
}
