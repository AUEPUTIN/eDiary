using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eDiary
{
    public class ProtoBufManager :IMySerializable
    {
        public string extension { get; } = "bin";

        public void Serialize(User user)
        {
            try
            {
                FileStream stream = new FileStream($"{user.Login}.{extension}", FileMode.OpenOrCreate);
                Serializer.Serialize(stream, user);
                stream.Close();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Serialize(ConsoleApp consoleApp)
        {
            try
            {
                FileStream stream = new FileStream($"Users.{extension}", FileMode.OpenOrCreate);
                Serializer.Serialize(stream, consoleApp);
                stream.Close();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public User Deserialize(string filePath)
        {
            if (filePath == null)
                throw new FileNotFoundException(nameof(filePath));

            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                return Serializer.Deserialize<User>(stream);
            }
        }
        public ConsoleApp DeserializeApp(string filePath)
        {
            if (filePath == null)
                throw new FileNotFoundException(nameof(filePath));

            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                return Serializer.Deserialize<ConsoleApp>(stream);
            }
        }
    }
}
