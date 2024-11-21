namespace GOSSIP.Models
{
    //Тимчасова модель користувача, використовувана для чатів/постів. Пізніше буде замнінена на робочу
    public class User (string username, string iconName)
    {
        public string Username { get; set; } = username;
        public string IconName = iconName;

        //Поки іконки беруться із папки /Images/TempUserIcons.
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{IconName}";
    }
}
