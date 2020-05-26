using PasswordsManager.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PasswordsManager.Models;
using PasswordsManager.Utils;

namespace PasswordsManager.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void OpenSite(object sender, RoutedEventArgs e)
        {
            Password password = (sender as Button).DataContext as Password;
            System.Diagnostics.Process.Start(password.Url);
        }

        private void CopyLogin(object sender, RoutedEventArgs e)
        {
            Password password = (sender as Button).DataContext as Password;
            Clipboard.SetText(password.Login);
        }

        private void CopyPassword(object sender, RoutedEventArgs e)
        {
            Password password = (sender as Button).DataContext as Password;
            Clipboard.SetText(Crypto.Decrypt(password.Pass));
        }

        private void FilterByTag(object sender, SelectionChangedEventArgs e)
        {
            Tag SelectTag = (sender as ListView).SelectedItem as Tag;
            var context = DataAccess.PasswordsDbContext.Current;
            var queryPasswords = context.Passwords
                .Where(p => p.Tags.Any(t => t.TagId == SelectTag.Id))
                .ToList();

            showAddForm(sender, e);

            _viewModel.Passwords.Clear();
            foreach (var item in queryPasswords)
                _viewModel.Passwords.Add(item);


        }

        private void ShowUpdateForm(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                _viewModel.ShowUpdate = true;
                _viewModel.SelectedPasswordTag.Clear();
                _viewModel.SelectedPassword = (sender as ListView).SelectedItem as Password;
                // _viewModel.SelectedPassword.Pass = Crypto.Decrypt(_viewModel.SelectedPassword.Pass);
                var context = DataAccess.PasswordsDbContext.Current;
                var queryPasswordTag = context.Tags
                    .Where(p => p.Passwords.Any(t => t.PasswordId == _viewModel.SelectedPassword.Id))
                    .ToList();
                foreach (var item in queryPasswordTag)
                    _viewModel.SelectedPasswordTag.Add(item);

            }
        }

        public void ShowAllTag(object sender, RoutedEventArgs e)
        {
            var context = DataAccess.PasswordsDbContext.Current;
            var queryPasswords = context.Passwords.ToList();
            _viewModel.Passwords.Clear();
            foreach (var item in queryPasswords)
                _viewModel.Passwords.Add(item);
        }
        public void CreatePassword(object sender, RoutedEventArgs e)
        {
            Password newPassword = new Password()
            {
                Label = _viewModel.SelectedPassword.Label,
                Login = _viewModel.SelectedPassword.Login,
                Pass = Crypto.Encrypt(_viewModel.SelectedPassword.Pass),
                Url = _viewModel.SelectedPassword.Url
            };
            var context = DataAccess.PasswordsDbContext.Current;
            context.Add(newPassword);
            context.SaveChanges();
            _viewModel.Passwords.Add(newPassword);
            MessageBox.Show("Le mot de passe a bien été créé");
        }
        public void UpdatePassword(object sender, RoutedEventArgs e)
        {
            var context = DataAccess.PasswordsDbContext.Current;
            var pass = ((sender as Button).DataContext as MainViewModel).SelectedPassword;
            pass.Pass = Crypto.Encrypt(pass.Pass);
            context.SaveChanges();
            MessageBox.Show("Le mot de passe a bien été modifié");
        }

        public void DeletePassword(object sender, RoutedEventArgs e)
        {
            var context = DataAccess.PasswordsDbContext.Current;
            Password password = (sender as Button).DataContext as Password;
            _viewModel.Passwords.Remove(password);
            context.Passwords.Remove(password);
            context.SaveChanges();
        }
        public void DeletePasswordTag(object sender, RoutedEventArgs e)
        {
            var context = DataAccess.PasswordsDbContext.Current;
            Tag tag = (sender as Button).DataContext as Tag;
            _viewModel.SelectedPasswordTag.Remove(tag);
            context.PasswordTagLinks.Remove(new PasswordTag()
            {
                PasswordId = _viewModel.SelectedPassword.Id,
                TagId = tag.Id
            });
            _viewModel.SelectedPassword.Tags.Remove(new PasswordTag()
            {
                PasswordId = _viewModel.SelectedPassword.Id,
                TagId = tag.Id
            });
            context.SaveChanges();
        }
        public void CreatePasswordTag(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedTag != null)
            {
                var context = DataAccess.PasswordsDbContext.Current;
                _viewModel.SelectedPasswordTag.Add(_viewModel.SelectedTag);
                context.PasswordTagLinks.Add(new PasswordTag()
                {
                    PasswordId = _viewModel.SelectedPassword.Id,
                    TagId = _viewModel.SelectedTag.Id
                });
                context.SaveChanges();
            }
        }

        public void CreateTag(object sender, RoutedEventArgs e)
        {
            string TagName = new Utils.InputBox("Entrer le tag", "Tag", "Arial", 20).ShowDialog();

            Tag newTag = new Tag()
            {
                Label = TagName,
            };
            var context = DataAccess.PasswordsDbContext.Current;
            context.Add(newTag);
            context.SaveChanges();
            _viewModel.Tags.Add(newTag);;
        }

        public void showAddForm(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedPassword = new Password();
            _viewModel.SelectedPasswordTag.Clear();
            _viewModel.ShowUpdate = false;
        }
    }
}
