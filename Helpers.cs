using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;

namespace StructureOnWPF
{
    public class Helpers
    {
        public static MeshGeometry3D SolveMeshReference(MeshGeometry3D meshGeometry3D)
        {
            var meshClone = meshGeometry3D.CloneCurrentValue();
            //meshClone.TriangleIndices = null;
            return meshClone;
        }
        public static IEnumerable<RepStructure> CreateRepStructures(StructureSet ss)
        {
            var sts = ss.Structures.Where(e => e.MeshGeometry != null);

            var repsts = new List<RepStructure>();

            sts.ToList().ForEach(e => repsts.Add(new RepStructure(e.Id, e.Color, Helpers.SolveMeshReference(e.MeshGeometry))));

            return repsts;
        }
        public static string CheckStructureIds(StructureSet ss, string wantedId)
        {
            var maxLength = 18;
            if (ss.Structures.Any(e=>e.Id.ToLower() == wantedId.ToLower()))
            {
                wantedId = limitChars(new Random().Next(0, 9).ToString() + "_" + wantedId, maxLength);
            }
            return wantedId;
        }
        public static string limitChars(string wantedId, int maxLength)
        {
            return wantedId.Length <= maxLength ? wantedId : wantedId.Substring(0, maxLength);

        }

    }
}
