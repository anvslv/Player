using System;
using System.IO;
using System.Xml.Serialization;

namespace Player.View.Services
{ 
    public class Serialization {
      
        private static Serialization _baseInstance;
        private static Serialization BaseInstance
        {
            get
            {
                if (_baseInstance == null)
                {  
                    _baseInstance = new Serialization(); 
                }
                return _baseInstance;
            }
        } 
  
        public static T DeserializeXMLFromPath<T>(string path)
        {
            return BaseInstance.DeserializeXMLFromPathEx<T>(path);
        }
          
        public object DeserializeXMLFromPathEx(Type t, string path)
        {
            object result = null;
           
            if (File.Exists(path))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(t);
                using (StreamReader streamReader = new StreamReader(path))
                {
                    result = xmlSerializer.Deserialize(streamReader);
                }
            }
         
            return result;
        }

        public T DeserializeXMLFromPathEx<T>(string path)
        {
            T result = default(T); 
         
            if (File.Exists(path))
            {
                try {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof (T));
                    using (StreamReader streamReader = new StreamReader(path))
                    {
                        result = (T) xmlSerializer.Deserialize(streamReader);
                    }
                } catch {
                    
                }
            }
        
            return result;
        } 

        public static void SerializeAsXMLToPath<T>(T source, string path)
        {
            BaseInstance.SerializeAsXMLToPathEx<T>(source, path);
        }

        public void SerializeAsXMLToPathEx(Type t, object source, string path)
        { 
            StreamWriter streamWriter = new StreamWriter(path);
            XmlSerializer xmlSerializer = new XmlSerializer(t);
            xmlSerializer.Serialize(streamWriter, source);
            streamWriter.Close(); 
        }

        public void SerializeAsXMLToPathEx<T>(T source, string path)
        { 
            StreamWriter streamWriter = new StreamWriter(path);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(streamWriter, source);
            streamWriter.Close(); 
        } 
    }
}