using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace StructureOnWPF
{
    public class SegShift
    {
        public VVector MMDisplacement { get; set; }


        public Structure Structure { get; set; }
        public StructureSet SS { get; set; }

        public IDictionary<int, IEnumerable<IEnumerable<VVector>>> Contours { get; private set; }

        public readonly IDictionary<int, IEnumerable<IEnumerable<VVector>>> OriginalContours;


        public SegShift(VVector mMDisplacement, StructureSet structureSet, Structure structure)
        {
            MMDisplacement = mMDisplacement;
            Structure = structure;
            SS = structureSet;
            OriginalContours = GetContours();
            Contours = OriginalContours;
        }
        public static IEnumerable<int> _GetMeshBounds(Structure structure, StructureSet SS)
        {
            var mesh = structure.MeshGeometry.Bounds;
            var meshLow = _GetSlice(mesh.Z, SS);
            var meshUp = _GetSlice(mesh.Z + mesh.SizeZ, SS) + 1;
            return Enumerable.Range(meshLow, meshUp);

        }
        public KeyValuePair<int, int> MeshBoundsSlices()
        {
            var mesh = Structure.MeshGeometry.Bounds;
            var meshLow = GetSlice(mesh.Z);
            var meshUp = GetSlice(mesh.Z + mesh.SizeZ) + 1;
            return new KeyValuePair<int, int>(meshLow, meshUp);
        }
        public Dictionary<int, IEnumerable<IEnumerable<VVector>>> GetContours()
        {
            var msh = MeshBoundsSlices();

            return Enumerable.Range(msh.Key, msh.Value).
                Where(e => Structure.GetContoursOnImagePlane(e).Any()).
                ToDictionary(s => s, s => Structure.GetContoursOnImagePlane(s).Select(e => e.Select(x => new VVector(x.x, x.y, x.z))));
        }

        public SegmentVolume MoveStructure()
        {
            var placeholder = SS.AddStructure("CONTROL", "placeholder" + new Random().Next(0, 20));
            if (Structure.IsHighResolution)
            {
                placeholder.ConvertToHighResolution();
            }
            foreach (var cc in Contours)
            {
                var contoursOnPlane = cc.Value;
                var moved = Move(contoursOnPlane.Select(e => e.ToArray()).ToArray());
                foreach (var vector in moved)
                {
                    placeholder.AddContourOnImagePlane(vector, PlaneToContour(cc.Key));
                }
            }
            var seg = placeholder.SegmentVolume;
            SS.RemoveStructure(placeholder);
            //Contours = OriginalContours;
            return seg;


        }
        public static int _GetSlice(double z, StructureSet SS)
        {
            var imageRes = SS.Image.ZRes;

            return Convert.ToInt32((z) - SS.Image.Origin.z / imageRes);
        }
        public int GetSlice(double z)
        {
            var imageRes = SS.Image.ZRes;

            return Convert.ToInt32((z) - SS.Image.Origin.z / imageRes);
        }
        public static double PlaneToCentimeters(int plane, Image image3D)
        {
            var zeroSlice = Math.Round((image3D.Origin.z - image3D.UserOrigin.z) / 10, 2);
            return zeroSlice + image3D.ZRes / 10 * plane;
        }
        public int PlaneToContour(int actualplane)
        {
            var wantedslicedisplacement = MMDisplacement.z;
            var plane = (int)Math.Round((wantedslicedisplacement) / (SS.Image.ZRes), 1);
            return actualplane + plane;
        }
        public VVector[][] Move(VVector[][] contoursOnPlane)
        {
            var copy = (VVector[][])contoursOnPlane.Clone();
            for (int i = 0; i < contoursOnPlane.Length; i++)
            {
                var countour = contoursOnPlane[i];
                var copy_c = copy[i];
                for (int j = 0; j < countour.Length; j++)
                {
                    copy_c[j] = countour[j] + MMDisplacement;
                }
            }
            return copy;


        }
    }
}