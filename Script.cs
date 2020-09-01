using StructureOnWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace VMS.TPS
{
    public class Script
    {
        public static void Execute(ScriptContext scriptContext, Window window)
        {
            var repsts = Helpers.CreateRepStructures(scriptContext.StructureSet);
            var gen = new WindowGenerator(repsts);
            
            window.Content = gen.ConstructAndGenerateWindow(scriptContext.StructureSet);
        }
    }
}
