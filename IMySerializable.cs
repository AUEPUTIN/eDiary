using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eDiary
{
    public interface IMySerializable
    {
        public string extension { get; }
        public void Serialize(User user);
        public User Deserialize(string filePath);
        public void Serialize(ConsoleApp consoleApp);
        public ConsoleApp DeserializeApp(string filePath);
    }
}
