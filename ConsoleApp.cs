using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace eDiary
{
    [ProtoBuf.ProtoContract()]
    [XmlRoot("ConsoleApplication")]
    [MessagePackObject]
    public class ConsoleApp
    {
        [ProtoBuf.ProtoMember(1)]
        [XmlElement("UserList")]
        [Key("users")]
        public List<User> Users { get; set; }

        private AuthorizationManager _authorizationManager;
        private DiaryManager _diaryManager;
        private IMySerializable _serializationManager;

        public ConsoleApp()
        {
            Users = new List<User>();
        }

        public void InitializeSerialization(IMySerializable serializable)
        {
            _serializationManager = serializable;
            _authorizationManager = new AuthorizationManager(_serializationManager);
            _diaryManager = new DiaryManager(_serializationManager);
        }
        public static IMySerializable SelectSerialization(string serialization)
        {

            if (serialization == string.Empty)
                SelectSerialization(serialization);

            if (serialization.Equals("Xml", StringComparison.OrdinalIgnoreCase))
                return new XmlManager();

            else if (serialization.Equals("Bin", StringComparison.OrdinalIgnoreCase))
                return new ProtoBufManager();

            else if (serialization.Equals("msgpack", StringComparison.OrdinalIgnoreCase))
                return new MessagePackManager();

            else
            {
                Console.WriteLine("There is no such format. Selected default serializer - xml");
                return new XmlManager();
            }
        }
        public void ShowActivities()
        {
            Console.WriteLine("1.Input diary note.");
            Console.WriteLine("2.Show my diary notes");

        }

        public void ChooseActivity(User user)
        {
            int activityNumber = InputInteger();

            if (activityNumber == 1)
                _diaryManager.InputDiaryNote(user);

            else if (activityNumber == 2)
                ShowUserNotes(_diaryManager.GetNotes(user));

            return;
        }

        private static int InputInteger()
        {
            int activityNumber = 0;

            while (!int.TryParse(Console.ReadLine(), out activityNumber))
            {
                Console.WriteLine("Input correct number!");
            }
            return activityNumber;
        }
        private static string InputString(string message)
        {
            Console.WriteLine(message);
            string serialization = Console.ReadLine();

            if (serialization == string.Empty)
                InputString(message);

            return serialization;
        }
        public void RunApp()
        {
            Console.WriteLine("Press enter to start");
            while ((Console.ReadLine()) != "exit")
            {
                Console.WriteLine("Are you registered (y/n)");

                string result = Console.ReadLine();
                if (result.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    User user = new User();

                    Console.Write($"{"Login:",-10}");
                    user.Login = Console.ReadLine();

                    Console.Write($"{"Password:",-10}");
                    user.Password = Console.ReadLine();

                    var application = ConsoleApp.GetConsoleApp();

                    bool isLoggedIn = application.Authorize(user, "Login");

                    if (isLoggedIn)
                    {
                        ShowActivities();
                        ChooseActivity(user);
                    }
                }
                else if (result.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    User user = new User();

                    Console.Write($"{"Email:",-10}");
                    user.Login = Console.ReadLine();

                    Console.Write($"{"Password:",-10}");
                    user.Password = Console.ReadLine();

                    var application = ConsoleApp.GetConsoleApp();
                    application.Authorize(user, "Register");
                }
                else
                    Console.WriteLine("Input correct data");
            }
        }

        public bool Authorize(User user, string authorizationType)
        {
            var (success, errors) = _authorizationManager.Authorize(user, authorizationType);

            if (authorizationType.Equals("Login", StringComparison.OrdinalIgnoreCase))
            {
                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("User has successfully logged in ");
                    Console.ResetColor();

                    return true;
                }

                else
                {
                    foreach (var error in errors)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(error);
                        Console.ResetColor();
                    }

                    return false;
                }
            }
            else
            {
                if (success)
                {
                    AddUserToFile(user, _serializationManager);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("The user has successfully registered!");
                    Console.ResetColor();

                    return false;
                }

                else
                {
                    foreach (var error in errors)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(error);
                        Console.ResetColor();
                    }

                    return false;
                }
            }
        }

        public void AddUserToFile(User user, IMySerializable mySerializable)
        {
            string fileName = $"Users.{mySerializable.extension}";

            if (!File.Exists(fileName))
                Console.WriteLine("There is no file with this name");

            else
            {
                ConsoleApp consoleApp = _serializationManager.DeserializeApp(fileName);

                consoleApp.Users.Add(user);

                _serializationManager.Serialize(consoleApp);
            }

        }

        public void ShowUserNotes(List<string> notes)
        {
            foreach (var note in notes)
                Console.WriteLine(note);
        }

        private static ConsoleApp instance;
        private static IMySerializable serializationManager;

        public static ConsoleApp GetConsoleApp()
        {
            if (serializationManager == null)
            {
                serializationManager = SelectSerialization(InputString("Input type of serialization:"));
            }

            if (instance == null)
            {
                string filePath = $"Users.{serializationManager.extension}";

                if (File.Exists(filePath))
                {
                    try
                    {
                        instance = serializationManager.DeserializeApp(filePath);

                        instance.InitializeSerialization(serializationManager);
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading application data: {ex.Message}");
                        Console.WriteLine("Creating new application instance...");

                        instance = new ConsoleApp();
                        instance.InitializeSerialization(serializationManager);

                        serializationManager.Serialize(instance);
                    }
                }
                else
                {
                    instance = new ConsoleApp();

                    instance.InitializeSerialization(serializationManager);
                    serializationManager.Serialize(instance);
                }

            }
            return instance;
        }
    }
}
