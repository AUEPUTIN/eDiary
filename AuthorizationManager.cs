using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace eDiary
{
    public class AuthorizationManager
    {
        public IMySerializable _serializeManager { get; }

        public AuthorizationManager(IMySerializable mySerializable)
        {
            this._serializeManager = mySerializable;
        }
        public (bool success, List<string> errors) Authorize(User user, string authorizationType)
        {
            Type userType = user.GetType();
            var properties = userType.GetProperties();

            var errors = ValidateUserFields(properties, user);

            if (errors.Count == 0)
            {
                if (authorizationType.Equals("Login", StringComparison.OrdinalIgnoreCase))
                {
                    var (success, error) = TryLogin(user, _serializeManager);
                    if (!success)
                        errors.Add(error);
                }
                else if (authorizationType.Equals("Register", StringComparison.OrdinalIgnoreCase))
                {
                    var (success, error) = TryRegister(user, _serializeManager);
                    if (!success)
                        errors.Add(error);

                }
            }
            return (errors.Count == 0, errors);
        }

        private List<string> ValidateUserFields(IEnumerable<PropertyInfo> properties, User user)
        {
            List<string> errors = new List<string>();
            foreach (PropertyInfo property in properties)
            {
                if (HasRegexAttribute(property))
                {
                    var (success, error) = ValidateProperty(property, user);
                    if (!success)
                        errors.Add(error);
                }
            }
            return errors;
        }

        bool HasRegexAttribute(PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(RegexPatternAttribute), false).Any();
        }

        private (bool success, string errorMessage) ValidateProperty(PropertyInfo property, User user)
        {
            string value = property.GetValue(user) as string;

            if (value == null)
            {
                return (true, string.Empty);
            }

            var regexAttribute = property.GetCustomAttributes(typeof(RegexPatternAttribute), false).First() as RegexPatternAttribute;

            Regex regex = new Regex(regexAttribute.Pattern);

            if (!regex.IsMatch(value))
            {
                return (false, regexAttribute.ErrorMessage);
            }

            return (true, string.Empty);
        }

        private (bool success, string errorMessage) TryLogin(User user, IMySerializable mySerializable)
        {
            string fileName = $"Users.{mySerializable.extension}";

            if (!File.Exists(fileName))
                throw new FileNotFoundException(nameof(fileName));

            else
            {
                ConsoleApp consoleApp = _serializeManager.DeserializeApp(fileName);

                var isValidUser = consoleApp.Users.Any(u => u.Login == user.Login && u.Password == user.Password);

                if (isValidUser)
                    return (true, string.Empty);

                else
                    return (false, "Incorrect login or password entered");
            }
            return (false, string.Empty);
        }
        private (bool success, string errorMessage) TryRegister(User user, IMySerializable mySerializable)
        {
            string fileName = $"Users.{mySerializable.extension}";

            if (!File.Exists(fileName))
                throw new FileNotFoundException(nameof(fileName));
            else
            {
                ConsoleApp consoleApp = _serializeManager.DeserializeApp(fileName);

                var isUserExists = consoleApp.Users.Any(u => u.Login == user.Login);

                if (!isUserExists)
                {
                    _serializeManager.Serialize(user);
                    return (true, string.Empty);
                }

                else
                    return (false, "User with these login and password is already exists");
            }
            return (false, string.Empty);
        }
    }
}
