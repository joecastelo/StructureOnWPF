using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace StructureOnWPF
{
    public class RepStructure
    {
        public string Id { get; set; }
        public Color Color { get; set; }

        public MeshGeometry3D Mesh { get; set; }

        public RepStructure(string id, Color color, MeshGeometry3D mesh)
        {
            Id = id;
            Color = color;
            Mesh = mesh;
        }
    }
}