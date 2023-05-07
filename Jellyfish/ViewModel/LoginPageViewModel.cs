using JellyFish.Service;
using JellyFish.Validation;
using JellyFish.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JellyFish.ViewModel
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly MainPageViewModel _mainPageViewModel;

        public ValidatableObject<string> Email { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> Password { get; private set; } = new ValidatableObject<string>();
        public bool IsLoading
        {
            get; private set;   
        }
        public bool ShowPassword
        {
            get; private set;   
        }
        public string ApplicationVersionStr
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public string CopyrightStr
        {
            get
            {
                return "© by 0x00405a00";
            }
        }
        public string CompanyStr
        {
            get
            {
                return "roosIT";
            }
        }

        public ICommand OpenRegisterPageCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }
        public ICommand ValidateByValueChangeCommand { get; private set; }
        public ICommand OpenForgotPasswordPageCommand { get; private set; } 

        public LoginPageViewModel(MainPageViewModel mainPageViewModel,NavigationService navigationService)
        {
            _navigationService = navigationService;
            _mainPageViewModel = mainPageViewModel; 
            OpenRegisterPageCommand = new RelayCommand(OpenRegisterPageAction);
            ValidateByValueChangeCommand = new RelayCommand<string>(ValidateUiVoid);
            SubmitCommand = new RelayCommand(SubmitAction);
            OpenForgotPasswordPageCommand = new RelayCommand(OpenForgotPasswordPageAction);
            AddValidations();
            Email.Value = "test@roos-it.net";
            Password.Value = "test";
        }
        private void OpenForgotPasswordPageAction()
        {
            _navigationService.PushAsync(new ResetPasswordContentPage(new ResetPasswordContentPageViewModel()));
        }
        private async void SubmitAction()
        {
            IsLoading=true;
            bool validEntries = ValidateUi();
            if (validEntries) {
                if(Email.Value == "test@roos-it.net" && Password.Value == "test")
                {
                    await _navigationService.PushAsync(new MainPage(_mainPageViewModel));
                    _mainPageViewModel.SwipeRightAction(_mainPageViewModel.ViewTemplates);
                    Email.Value = null;
                    Password.Value = null;
                }
            }
            IsLoading=false;
        }
        private bool ValidateUi(int checkUiElemt = -1)
        {
            bool isValidEmail = false;
            bool isValidPassword = false;
            switch(checkUiElemt)
            {
                case 1:
                    isValidEmail = ValidateEmail();    
                    break;
                case 2:
                    isValidPassword= ValidatePassword();  
                    break;
                default:
                    isValidEmail = ValidateEmail();
                    isValidPassword = ValidatePassword();
                    break;
            }
            return isValidEmail && isValidPassword;  
        }
        private void ValidateUiVoid(string checkUiElemt)
        {
            if(int.TryParse(checkUiElemt, out int value))
            {
                ValidateUi(value);

            }

        }
        private bool ValidateEmail()
        {
            return Email.Validate();
        }

        private bool ValidatePassword()
        {
            return Password.Validate();
        }
        private void AddValidations()
        {
            Email.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Email entry is required."
            });
            Email.Validations.Add(new EmailRule { 
                ValidationMessage = "Email is invalid."
            });

            Password.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Password entry is required."
            });
        }
        private void OpenRegisterPageAction()
        {
            _navigationService.PushAsync(new RegisterContentPage(new RegisterContentPageViewModel()));
        }
    }
}
