﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DVJUCSVConverterService
{
    public static class Serializer
    {
        public static bool SerializeItem<T>(T file, string fileName)
        {            
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));

                FileStream stream = new FileStream(fileName, FileMode.Create);
                formatter.Serialize(stream, file);
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static T DeserializeItem<T>(string fileName)
        {            
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));

                FileStream stream = new FileStream(fileName, FileMode.Open);
                T item = (T)formatter.Deserialize(stream);
                stream.Close();
                return item;
            }
            catch (FileNotFoundException)
            {
                return default;
            }
            catch
            {
                throw;
            }
            
        }
    }
}
