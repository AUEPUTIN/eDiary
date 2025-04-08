using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eDiary
{
    internal class XmlManager : IMySerializable
    {
        public string extension { get; } = "xml";
        public  void Serialize(User user)
        {
            try
            {
                FileStream stream = new FileStream($"{user.Login}.xml", FileMode.OpenOrCreate);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(User));
                xmlSerializer.Serialize(stream, user);
                stream.Close();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public  void Serialize(ConsoleApp consoleApp )
        {
            try
            {
                FileStream stream = new FileStream("Users.xml", FileMode.OpenOrCreate);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConsoleApp));
                xmlSerializer.Serialize(stream, consoleApp);
                stream.Close();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public  User Deserialize(string filePath)
        {
            if (filePath == null)
                throw new FileNotFoundException(nameof(filePath));

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(User));

                return xmlSerializer.Deserialize(stream) as User;
            }
        }
        public  ConsoleApp DeserializeApp(string filePath)
        {
            if (filePath == null)
                throw new FileNotFoundException(nameof(filePath));

            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConsoleApp));

                return xmlSerializer.Deserialize(stream) as ConsoleApp;
            }
        }
    }
}
