using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eDiary
{
    public class DiaryManager
    {
        IMySerializable _serializationManager;
        public DiaryManager(IMySerializable mySerializable)
        {
            this._serializationManager = mySerializable;
        }
        public void InputDiaryNote(User user)
        {
            Console.WriteLine(DateTime.Now.Date);
            Console.WriteLine("Write your note");

            string note = Console.ReadLine();

            if (note == string.Empty)
            {
                InputDiaryNote(user);
            }

            string fileName = $"{user.Login}.{_serializationManager.extension}";

            if(!File.Exists(fileName))
                Console.WriteLine("There is no file with this name");

            User existingUser = _serializationManager.Deserialize(fileName) as User;

            existingUser.UserNotes.Add(note);

            _serializationManager.Serialize(existingUser);
        }
        public List<string> GetNotes(User user)
        {
            if (user == null) 
                throw new ArgumentNullException(nameof(user));

            string fileName = $"{user.Login}.{_serializationManager.extension}";

            if (!File.Exists(fileName))
                Console.WriteLine("There is no file with this name");

            User existingUser = _serializationManager.Deserialize(fileName) as User;

            return existingUser.UserNotes;
        }
    }
}
