namespace GOSSIP.Models
{
    public class User (string username, string iconName)
    {
        public string Username { get; set; } = username;
        public string IconName = iconName;
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{IconName}";
    }
}
