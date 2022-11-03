using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SerializationModels = TizenGameEngine.LevelLoader.Models.Serialization;

namespace TizenGameEngine.LevelLoader.Models
{
    public class TlfLoader: ILevelLoader
    {
        public Level LoadContent(string content)
        {
            var deserializedModel = JsonConvert.DeserializeObject<SerializationModels.ObjectItem>(content);

            return ToLevel(deserializedModel);
        }

        public Level LoadFile(string path)
        {
            string result;

            using (StreamReader streamReader = new StreamReader(path))
            {
                result = streamReader.ReadToEnd();
            }

            return LoadContent(result);
        }

        private Object ToObject(SerializationModels.ObjectItem objectItem)
        {
            switch (objectItem.Type)
            {
                case SerializationModels.ObjectType.OBJ_MESH:
                    return ToObjMesh(objectItem);
                default:
                    throw new Exception($"An unknown object type found.");
            }
        }

        private Level ToLevel(SerializationModels.ObjectItem objectItem)
        {
            return new Level
            {
                Name = objectItem.Name,
                Children = objectItem.Children.Select(e => ToObject(e)).ToArray()
            };
        }

        private ObjMesh ToObjMesh(SerializationModels.ObjectItem objectItem)
        {
            return new ObjMesh
            {
                Type = ObjectType.OBJ_MESH,
                Position = ToVector(objectItem.Position),
                Rotation = ToVector(objectItem.Rotation),
                Scale = ToVector(objectItem.Scale),
                GeometryPath = objectItem.GeometryPath,
                Textures = objectItem.Textures,
                Children = objectItem.Children.Select(e => ToObject(e)).ToArray()
            };
        }

        private Vector ToVector(SerializationModels.Vector deserializedVector)
        {
            return new Vector
            {
                X = deserializedVector.X,
                Y = deserializedVector.Y,
                Z = deserializedVector.Z
            };
        }
    }
}

