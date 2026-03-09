using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmartApp.Helpers.Helpers
{
    public static class Serialize
    {
        public static void SerializeDataToBinaryFile<T>(string path, T data)
        {
            try
            {
                var bf = new BinaryFormatter();
                using var fs = File.Create(path);
                bf.Serialize(fs, data);
            }
            catch (Exception)
            {
            }
        }

        public static async Task SerializeDataToBinaryFileAsync<T>(string path, T data) => await Task.Run(() =>
        {
            try
            {
                var fs = File.Create(path);
                var bf = new BinaryFormatter();
                bf.Serialize(fs, data);
                fs.Close();
            }
            catch (Exception)
            {
            }
        });

        public static T DeSerializeBinaryToData<T>(string path)
        {
            T data = default;
            try
            {
                var bf = new BinaryFormatter();
                var fs = File.OpenRead(path);
                data = (T)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
            }

            return data;
        }

        public static async Task<T> DeSerializeBinaryToDataAsync<T>(string path) => await Task.Run(() =>
        {
            T data = default;
            try
            {
                var bf = new BinaryFormatter();
                var fs = File.OpenRead(path);
                data = (T)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
            }

            return data;
        });

        public static void SerializeDataToXmlFile<T>(string saveToPath, T data, bool append = false)
        {
            try
            {
                var xml = new XmlSerializer(typeof(T));
                using var w = new StreamWriter(saveToPath, append);
                xml.Serialize(w, data);
            }
            catch (Exception)
            {
            }
        }

        public static async Task SerializeDataToXmlFileAsync<T>(string saveToPath, T data, bool append = false)
            => await Task.Run(() =>
            {
                try
                {
                    var xml = new XmlSerializer(typeof(T));
                    using var w = new StreamWriter(saveToPath, append);
                    xml.Serialize(w, data);
                }
                catch (Exception)
                {
                }
            });

        public static T DeSerializeXmlToData<T>(string path)
        {
            T a = default;
            try
            {
                var xml = new XmlSerializer(typeof(T));
                using var reader = new StreamReader(path);
                a = (T)xml.Deserialize(reader);
            }
            catch (Exception)
            {
            }

            return a;
        }

        public static async Task<T> DeSerializeXmlToDataAsync<T>(string path) => await Task.Run(() =>
        {
            T a = default;
            try
            {
                var xml = new XmlSerializer(typeof(T));
                using var reader = new StreamReader(path);
                a = (T)xml.Deserialize(reader);
            }
            catch (Exception)
            {
            }

            return a;
        });
    }
}
