using Aspros.ValueObjects;

namespace Aspros.MessageModel
{
    public class CreateUserModel
    {
        public string UserName { get; set; }
        public UserType UserType { get; set; }
    }
}
