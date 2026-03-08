using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.Services;
using OrdoMill.ViewModel;
using PropertyChanged;

namespace OrdoMill.Views.Home
{
    [AddINotifyPropertyChangedInterface]
    public class LoginViewModel : DialogContentViewModel
    {
        public static DbCon Context => new DbCon(Settings.Default.ConnectionString);
        public LoginViewModel()
        {
            try
            {
                LogOutCommand = new RelayCommand(LogOut);
                ErrorMessage = "";
                UserName = Settings.Default.LastUser;
                LoggedUser = new User();
                Users = new ObservableCollection<string>(Context.Users.Select(x => x.UserName).ToList());
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        }

        private async void LogOut()
        {
            LoggedUser = new User();
            Password = "";
            await ShowDialoge();
        }

        public User LoggedUser { get; set; }

        public ObservableCollection<string> Users { get; set; }

        public string UserName { get; set; }

        public string ErrorMessage { get; set; }

        public string Password { get; set; }

        public override bool CheckHideDialogesConditions()
        {
            try
            {
                ErrorMessage = "";
                using (DbCon db = new DbCon(Settings.Default.ConnectionString))
                {
                    foreach (var user in db.Users.Where(user => user.UserName == UserName))
                    {
                        if (!PasswordHasher.VerifyPassword(Password, user.Password))
                        {
                            continue;
                        }

                        if (!user.IsWork)
                        {
                            ErrorMessage = "هذا الحساب موقف يرجى مراجعة المسؤول ";
                            return false;
                        }
                        LoggedUser = user;
                        Settings.Default.LastUser = user?.UserName;
                        Settings.Default.Save();

                        if (PasswordHasher.NeedsRehash(user.Password))
                        {
                            user.Password = PasswordHasher.HashPassword(Password);
                            db.SaveChanges();
                        }

                        return true;
                    }
                    ErrorMessage = "خطأ في كلمة المرور أو اسم المستخدم ";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.AppLogging();
                return false;
            }
        }

                        if (!user.IsWork)
                        {
                            ErrorMessage = "هذا الحساب موقف يرجى مراجعة المسؤول ";
                            return false;
                        }

                        if (PasswordHasher.NeedsRehash(user.Password))
                        {
                            user.Password = PasswordHasher.HashPassword(Password);
                            db.SaveChanges();
                        }

                        LoggedUser = user;
                        Settings.Default.LastUser = user?.UserName;
                        Settings.Default.Save();
                        return true;
                    }
                    ErrorMessage = "خطأ في كلمة المرور أو اسم المستخدم ";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.AppLogging();
                return false;
            }
        }

        public ViewModelLocator Locator => ViewModelLocator.Instance;

        public override bool CheckShowDialogesConditions()
        {
            try
            {
                if (LoggedUser == null)
                    return true;
                return !(LoggedUser?.UserName?.Length > 1);
            }
            catch (Exception ex)
            {
                ex.AppLogging();
                return true;
            }

        }

        public RelayCommand LogOutCommand { get; set; }

    }
}
