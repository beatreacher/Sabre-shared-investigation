namespace Domain.Models
{
    public class Client
    {
        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string Organization { get; private set; }

        public string Domain { get; private set; }

        public Client(string userName, string password, string organization, string domain)
        {
            UserName = userName;
            Password = password;
            Organization = organization;
            Domain = domain;
        }
    }
}
