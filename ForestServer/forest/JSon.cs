using System;
using System.IO;
using Newtonsoft.Json;

namespace ForestSolver
{
    public static class JSon
    {
        static public void Write<T>(T obj, Stream stream)
        {
            var serializer = new JsonSerializer();
            var writer = new JsonTextWriter(new StreamWriter(stream));
            try
            {
                serializer.Serialize(writer, obj);
                writer.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                writer.Close();
            }
        }

        static public T Read<T>(Stream stream) where T : class 
        {
            var serializer = new JsonSerializer();
            var reader = new JsonTextReader(new StreamReader(stream));
            T obj = null;
            try
            {
                obj = serializer.Deserialize<T>(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                reader.Close();
            }
            return obj;
        }
    }
}
