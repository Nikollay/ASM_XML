using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
            Console.WriteLine(DateTime.Now.ToString());
            Console.CursorSize = 100;
            ModelDoc2 swModel;
            AssemblyDoc swAssy;
            List<Comp> coll;
            XDocument doc;
            XElement xml, transaction, project, configurations, configuration, components, component;

            int errors = 0;
            int warnings = 0;
            string fileName;   // GetOpenFileName
            List<string> conf;
            string[] сonfNames;
            
            fileName = swApp.GetOpenFileName("Выберите сборку", "", "SLDASM Files (*.SLDASM)|*.SLDASM|", out _, out _, out _);
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

            doc = new XDocument(new XDeclaration("1.0", "Windows-1251", "Yes"));
            xml = new XElement("xml");
            transaction = new XElement("transaction", new XAttribute("Type", "SOLIDWORKS"), new XAttribute("version", "1.0"), new XAttribute("Date", DateTime.Now.ToString("d")), new XAttribute("Time", DateTime.Now.ToString("T")));
            project = new XElement("project", new XAttribute("Project_Path", fileName), new XAttribute("Project_Name", swModel.GetTitle() + ".SldAsm"));
            configurations = new XElement("configurations");
            components = new XElement("components");

            сonfNames = swModel.GetConfigurationNames();
            conf = new List<string>(сonfNames);

            ConfigForm f = new ConfigForm(conf);
            f.ShowDialog();

            if (f.conf == null)
            {
                swApp.ExitApp();
                return;
            }

            if (f.conf.Count == 0)
            {
                swApp.ExitApp();
                return;
            }

            for (int i = 0; i < f.conf.Count; i++)
            {
                swModel.ShowConfiguration2(f.conf[i]);
                configuration = new XElement("configuration", new XAttribute("name", f.conf[i]));
                coll = Comp.GetColl(swAssy, (SldWorks)swApp);
                foreach (Comp k in coll)
                {
                    component = Comp.GetComponent(k);
                    components.Add(component);
                }
                if (i == 0) { configuration.Add(Comp.GetGraphs(swAssy)); }
                configuration.Add(components);
                configurations.Add(configuration);
            }
            project.Add(configurations);
            transaction.Add(project);
            xml.Add(transaction);
            doc.Add(xml);
            Console.WriteLine(doc);
            doc.Save("d:\\macro\\test.xml");
            //Console.ReadKey();
            swApp.ExitApp();
        }
    }
}
