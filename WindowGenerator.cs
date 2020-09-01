using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;

namespace StructureOnWPF
{
    public class WindowGenerator
    {
        private IEnumerable<RepStructure> _structures;

        public WindowGenerator(IEnumerable<RepStructure> repStructures)
        {
            _structures = repStructures;
        }

        public StructureUserControl ConstructAndGenerateWindow(StructureSet ss)
        {


            return new StructureUserControl(new MainViewModel(_structures, ss));
        }
    }

}
