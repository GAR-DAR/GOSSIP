namespace GOSSIP.Models
{
    //Тимчасова модель користувача, використовувана для чатів/постів. Пізніше буде замнінена на робочу
    [Serializable]
    public class User
    {
        public string Username { get; set; }
        public string IconName { get; set; }

        //Поки іконки беруться із папки /Images/TempUserIcons.
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{IconName}";

        public User(string username, string iconName)
        {
            Username = username;
            IconName = iconName;
        }
    }
}
