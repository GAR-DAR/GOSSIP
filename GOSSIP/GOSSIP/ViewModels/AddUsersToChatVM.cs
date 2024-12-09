using GOSSIP.JsonHandlers;
using GOSSIP.Models;
using GOSSIP.ViewModels;
using GOSSIP;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

public class AddUsersToChatVM : ObservableObject
{
    public ICommand CloseCommand { get; set; }
    public ICommand CreateChatCommand { get; }

    // Подія для запиту на закриття вікна
    public event Action<bool?> RequestClose;

    public ObservableCollection<UserVM> AllUsers { get; set; }
    public List<UserVM> SelectedUsers { get; set; } = new();

    public AddUsersToChatVM()
    {
        JsonStorage jsonStorage = new("user_data.json");
        AllUsers = new ObservableCollection<UserVM>(
            jsonStorage.LoadUsers()
                       .Select(x => new UserVM(x))
                       .Where(x => x.UserModel.ID != MainVM.AuthorizedUserVM.UserModel.ID));

        CreateChatCommand = new RelayCommand(CreateChatMethod);
        CloseCommand = new RelayCommand(obj => RequestClose?.Invoke(false));
    }

    private void CreateChatMethod(object obj)
    {
        if (SelectedUsers.Count == 0)
        {
            MessageBox.Show("Select at least one user to create a chat");
            return;
        }

        string nameOfChat = MainVM.AuthorizedUserVM.Username + ", " + string.Join(", ", SelectedUsers.Select(user => user.Username));
        if (nameOfChat.Length > 40)
        {
            nameOfChat = nameOfChat.Substring(0, 40);
        }

        List<UserModel> users = [MainVM.AuthorizedUserVM.UserModel ];
        users.AddRange(SelectedUsers.Select(x => x.UserModel));

        ChatModel newChat = new(
            0,
            users,
            nameOfChat,
            DateTime.Now,
            false,
            []
            );

        MainVM.AuthorizedUserVM.UserModel.Chats.Add(newChat);
        RequestClose?.Invoke(true);
    }
}
