using PasswordsManager.DataAccess;
using PasswordsManager.Models;
using PasswordsManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordsManager.ViewModels
{
    public class MainViewModel : BaseNotifyPropertyChanged
    {
        public MainViewModel()
        {
            var context = DataAccess.PasswordsDbContext.Current;
            var queryPasswords = context.Passwords.ToList();
            foreach (var item in queryPasswords)
                Passwords.Add(item);
            var queryTags = context.Tags.ToList();
            foreach (var item in queryTags)
                Tags.Add(item);
            ShowUpdate = false;
            SelectedPassword = new Password();
        }

        private ObservableCollection<Password> _passwords;
        public ObservableCollection<Password> Passwords
        {
            get
            {
                if (_passwords == null) _passwords = new ObservableCollection<Password>();
                return _passwords;
            }
        }

        private ObservableCollection<Tag> _tags;
        public ObservableCollection<Tag> Tags
        {
            get
            {
                if (_tags == null) _tags = new ObservableCollection<Tag>();
                return _tags;
            }
        }


        private ObservableCollection<Tag> _selectedPasswordTag;
        public ObservableCollection<Tag> SelectedPasswordTag
        {
            get
            {
                if (_selectedPasswordTag == null) _selectedPasswordTag = new ObservableCollection<Tag>();
                return _selectedPasswordTag;
            }
        }

        public Password SelectedPassword
        {
            get { return GetValue<Password>(); }
            set { SetValue(value); }
        }

        public Tag SelectedTag
        {
            get { return GetValue<Tag>(); }
            set { SetValue(value); }
        }
        
        private bool _showUpdate;
        public bool ShowUpdate
        {
            get { return _showUpdate; }
            set
            {
                _showUpdate = value;
                OnPropertyChanged("ShowUpdate");
            }
        }
    }
}
