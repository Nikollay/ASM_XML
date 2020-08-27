using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace ASM_XML
{
    class Comp
    {
        private const double eps = 0.0000001;
        public string format, designation, name, note, chapter, included, doc = "", type = "", rotation;
        public int quantity;
        public double x, y, z;


        private static string Euler(double[] R)
        {

            double alpha, alpha2, beta, beta2, gamma, gamma2;

            if (Math.Abs(Math.Abs(R[6]) - 1) > eps)
            {
                beta = -Math.Asin(R[6]);
                beta2 = Math.PI - beta;
                alpha = Math.Atan2(R[7] / Math.Cos(beta), R[8] / Math.Cos(beta));
                alpha2 = Math.Atan2(R[7] / Math.Cos(beta2), R[8] / Math.Cos(beta2));
                gamma = Math.Atan2(R[3] / Math.Cos(beta), R[0] / Math.Cos(beta));
                gamma2 = Math.Atan2(R[3] / Math.Cos(beta2), R[0] / Math.Cos(beta2));
            }
            else
            {
                gamma = 0;
            }
            if (Math.Abs(R[6] + 1) < eps)
            {
                beta = Math.PI / 2;
                alpha = gamma + Math.Atan2(R[1], R[2]);
            }
            else
            {
                beta = -Math.PI / 2;
                alpha = -gamma + Math.Atan2(-R[1], -R[2]);
            }

            return Math.Round(alpha * 180 / Math.PI, 2) + "; " + Math.Round(beta * 180 / Math.PI, 2) + "; " + Math.Round(gamma * 180 / Math.PI, 2);
        }

        public static List<Comp> GetColl(AssemblyDoc swAssy, SldWorks swApp)
        {

            Comp component;
            List<Comp> coll;
            object[] comps;
            Component2 comp;
            ModelDoc2 compDoc;
            CustomPropertyManager prpMgr;
            ModelDocExtension swModelDocExt;
            swDocumentTypes_e docType= swDocumentTypes_e.swDocPART;
            string configuration;
            double[] aTrans;
            string valOut;
            string path;

            coll = new List<Comp>();

            swAssy.ResolveAllLightWeightComponents(false);

            comps = swAssy.GetComponents(true);
            Console.WriteLine("GetColl");
            Console.WriteLine(comps.Length);


            for (int i = 0; i < comps.Length; i++)
            {
                component = new Comp();
                comp = (Component2)comps[i];
                path = comp.GetPathName();
                if ((comp.GetSuppression() != (int)swComponentSuppressionState_e.swComponentSuppressed) & (comps[i] != null))
                {

                    aTrans = comp.Transform2.ArrayData;
                    if (path.EndsWith("SLDASM")) { docType = (swDocumentTypes_e)swDocumentTypes_e.swDocASSEMBLY; }
                    if (path.EndsWith("SLDPRT")) { docType = (swDocumentTypes_e)swDocumentTypes_e.swDocPART; }
                    int errs = 0, wrns = 0;
                    compDoc = swApp.OpenDoc6(path, (int)docType, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errs, ref wrns);
                    if (compDoc == null) { compDoc = comp.GetModelDoc2(); }

                    {
                        configuration = (string)comp.ReferencedConfiguration;
                        swModelDocExt = (ModelDocExtension)compDoc.Extension;
                        prpMgr = (CustomPropertyManager)swModelDocExt.get_CustomPropertyManager(configuration);

                        prpMgr.Get4("Формат", true, out valOut, out _);
                        component.format = valOut;
                        prpMgr.Get4("Обозначение", true, out valOut, out _);
                        component.designation = valOut;
                        prpMgr.Get4("Наименование", true, out valOut, out _);
                        component.name = valOut;
                        prpMgr.Get4("Примечание", true, out valOut, out _);
                        component.note = valOut;
                        prpMgr.Get4("Раздел", true, out valOut, out _);
                        component.chapter = valOut;
                        prpMgr.Get4("Перв.Примен.", true, out valOut, out _);
                        component.included = valOut;

                        if ((component.chapter == "Стандартные изделия") | (component.chapter == "Прочие изделия"))
                        {
                            prpMgr.Get4("Документ на поставку", true, out valOut, out _);
                            component.doc = valOut;
                            component.type = component.name.Substring(0, component.name.IndexOf((char)32));
                        }
                        component.rotation = Euler(aTrans);
                    }
                    coll.Add(component);
                }
            }

            foreach (Comp k in coll)
            {
                if (k.chapter != "Сборочные единицы" & k.chapter != "Детали" & k.chapter != "Документация" & k.chapter != "Комплекты")
                {
                    k.format = "";
                    k.designation = "";
                }
                if (k.format.Contains("*)"))
                {
                    k.note = k.format.Substring(2);
                    k.format = "*)";
                }
            }

         return coll;
        }
    }
}
