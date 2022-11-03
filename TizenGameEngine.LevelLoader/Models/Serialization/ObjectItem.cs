using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TizenGameEngine.LevelLoader.Models.Serialization
{
    [DataContract]
    public class ObjectItem
    {
        #region Level
        [DataMember(Name = "name")]
        public string Name { get; set; }
        #endregion

        #region Common
        [DataMember(Name = "type")]
        public ObjectType Type { get; set; }

        [DataMember(Name = "children")]
        public ICollection<ObjectItem> Children { get; set; }
        #endregion

        #region OBJ_MESH
        [DataMember(Name = "position")]
        public Vector Position { get; set; }

        [DataMember(Name = "rotation")]
        public Vector Rotation { get; set; }

        [DataMember(Name = "scale")]
        public Vector Scale { get; set; }

        [DataMember(Name = "geometryPath")]
        public string GeometryPath { get; set; }

        [DataMember(Name = "textures")]
        public ICollection<string> Textures { get; set; }
        #endregion
    }
}
