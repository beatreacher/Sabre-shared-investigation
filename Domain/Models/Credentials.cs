namespace Domain.Models
{
    public class Credentials
    {
        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string Organization { get; private set; }

        public string Domain { get; private set; }

        public Credentials(string userName, string password, string organization, string domain)
        {
            UserName = userName;
            Password = password;
            Organization = organization;
            Domain = domain;
        }
    }
}
