using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

[assembly: ESAPIScript(IsWriteable = true)]

namespace StructureOnWPF
{
    public class MainViewModel
    {
        public RepStructure Displaying { get; set; }
        public List<RepStructure> Sts { get; set; }

        public GeometryModel3D Model3D { get; set; }

        public Patient Pat { get; set; }

        public string SSId { get; set; }

        public string Logger { get; set; }
        public MainViewModel(IEnumerable<RepStructure> sts, StructureSet ss)
        {
            SSId = ss.Id;
            Pat = ss.Patient;
            Sts = sts.ToList();
            Displaying = Sts.First();
            var brush = new SolidColorBrush(Sts.First().Color);
            Model3D = new GeometryModel3D(Sts.First().Mesh, new DiffuseMaterial(brush));

        }

        public GeometryModel3D UpdateDisplaying(string id)
        {
            var model = new GeometryModel3D();
            Displaying = Sts.First(e => e.Id == id);
            model.Geometry = Displaying.Mesh;
            var brush = new SolidColorBrush(Displaying.Color);
            model.Material = new DiffuseMaterial(brush);
            return model;
        }
        public string CopyDisplaying()
        {
            Pat.BeginModifications();
            var ss = Pat.StructureSets.First(e => e.Id == SSId);
            var d_copy = ss.AddStructure("CONTROL", Displaying.Id + "cp");
            d_copy.SegmentVolume = ss.Structures.First(e => e.Id == Displaying.Id);
            Sts.Add(new RepStructure(d_copy.Id, d_copy.Color, Helpers.SolveMeshReference(d_copy.MeshGeometry)));
            Logger += $"Copied {d_copy.Id} with success";
            return d_copy.Id;
        }
        public string MoveDisplaying(double x, double y, double z)
        {
            Pat.BeginModifications();
            var mmchange = new VVector(x, y, z);
            var ss = Pat.StructureSets.First(e => e.Id == SSId);
            var wantMove = ss.Structures.First(e => e.Id == Displaying.Id);
            Structure dCopy = null;
            if (ss.Structures.Any(e => e.Id == Displaying.Id + "mv"))
            {
                dCopy = ss.Structures.Single(e => e.Id == Displaying.Id + "mv");

            }
            else
            {
                dCopy = ss.AddStructure("CONTROL", Displaying.Id + "mv");
            }
            dCopy.SegmentVolume = new SegShift(mmchange, ss, wantMove).MoveStructure();
            Logger += "Moved with success";
            Sts.Add(new RepStructure(dCopy.Id, dCopy.Color, Helpers.SolveMeshReference(dCopy.MeshGeometry)));
            return dCopy.Id;
        }
    }
}
