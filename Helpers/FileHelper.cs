using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Helpers
{
    public static class FileHelper
    {
        public static void SaveObjectToFile<T>(T obj, string outputPath)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(outputPath, true))
            {
                writer.WriteLine(json);
            }
            //File.WriteAllText(outputPath, json);
        }

        public static T GetObjectFromFile<T>(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                T obj = JsonConvert.DeserializeObject<T>(jsonString);
                if (obj is null)
                    throw new Exception($"Error while deserialize object from file in file path: {filePath} and object type: {typeof(T)}");
                return obj;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
