using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spillville.Models
{
    public class ModelFactory
    {
        // Change this sometime so something more efficient
        private static readonly Dictionary<string, Model> ModelDictionary = new Dictionary<string,Model>();

        public static void Add(string modelName, Model model)
        {
            ModelDictionary.Add(modelName, model);
        }

        public static Model Get(string modelName)
        {
            if(ModelDictionary.ContainsKey(modelName))
                return ModelDictionary[modelName];
            // Implement a default model for missing models
            return ModelDictionary["DefaultModel"];
        }

        public static void Clear()
        {
            ModelDictionary.Clear();
        }
    }
}
