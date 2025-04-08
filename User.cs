using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;
using MessagePack;

namespace eDiary
{
    [ProtoContract()]
    [MessagePackObject]
    public class User
    {
        [RegexPattern(@"^[^ ]{6,18}$", ErrorMessage = "Login must contain from 6 to 18 characters")]
        [ProtoMember(1)]
        [Key("login")]
        public string Login { get; set; }

        [RegexPattern(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "Password must contain Minimum eight characters, at least one letter, one number and one special character")]
        [ProtoMember(2)]
        [Key("password")]
        public string Password { get; set; }
        
        [ProtoMember(3)]
        [Key("userNotes")]
        public List< string> UserNotes { get; set; }

        public User(string password,string login)
        {
            Password = password;
            Login = login;
            UserNotes = new List<string>();
        }
        public User() 
        {
            UserNotes = new List< string>();
        }
    }
}
