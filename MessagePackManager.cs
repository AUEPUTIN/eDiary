using MessagePack;


namespace eDiary
{
    public class MessagePackManager : IMySerializable
    {
        public string extension { get; } = "msgpack";

        public void Serialize(User user)
        {
            try
            {
                var bytes = MessagePackSerializer.Serialize(user);
                File.WriteAllBytes($"{user.Login}.{extension}", bytes);
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
                var bytes = MessagePackSerializer.Serialize(consoleApp);
                File.WriteAllBytes($"Users.{extension}", bytes);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public User Deserialize(string filePath)
        {
            if (filePath == null)
                return null;

            var loadedBytes = File.ReadAllBytes(filePath);
            var user = MessagePackSerializer.Deserialize<User>(loadedBytes);

            return user;
        }
        public ConsoleApp DeserializeApp(string filePath)
        {
            if (filePath == null)
                throw new FileNotFoundException(nameof(filePath));

            var loadedBytes = File.ReadAllBytes(filePath);
            var consoleApp = MessagePackSerializer.Deserialize<ConsoleApp>(loadedBytes);

            foreach (var user in consoleApp.Users)
            {
                Console.WriteLine($"Deserialized user: Login={user.Login}, Password={user.Password}");
            }

            return consoleApp;
        }
    }
}
