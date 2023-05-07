using JellyFish.Service;
using JellyFish.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JellyFish.ViewModel
{
    public class RegisterContentPageViewModel : BaseViewModel
    {
        public class Language : BaseViewModel
        {
            public string PhonePrefix
            {
                get;
                set;
            }
            public string Country
            {
                get;
                set;
            }
        }
        private ObservableCollection<Language> _languages = new ObservableCollection<Language>();
        public ObservableCollection<Language> Languages
        {
            get
            {
                return _languages;
            }
            set
            {
                _languages = value;
            }
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
                return "© by";
            }
        }
        public string CompanyStr
        {
            get
            {
                return "roosIT";
            }
        }

        public ValidatableObject<string> Firstname { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> Lastname { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> Email { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> Password { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> PasswordRepeat { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> PhonePrefix { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> Phone { get; private set; } = new ValidatableObject<string>();
        public bool IsLoading
        {
            get; private set;
        }
        public bool ShowPassword
        {
            get; private set;
        }
        public ICommand SubmitCommand { get; private set; }
        public ICommand ValidateByValueChangeCommand { get; private set; }
        public RegisterContentPageViewModel()
        {
            _languages.Add(new Language { Country = "Deutschland", PhonePrefix = "+49" });
            _languages.Add(new Language { Country = "Deutschland2", PhonePrefix = "+49" });
            AddValidations();
            ValidateByValueChangeCommand = new RelayCommand<string>(ValidateUiVoid);
            SubmitCommand = new RelayCommand(SubmitAction);
        }
        private async void SubmitAction()
        {
            IsLoading = true;
            bool validEntries = ValidateUi();
            if (validEntries)
            {
                if (Email.Value == "test@roos-it.net" && Password.Value == "test")
                {


                }
            }
            IsLoading = false;
        }
        private bool ValidateUi(int checkUiElemt = -1)
        {
            bool isValidEmail = false;
            bool isValidPassword = false;
            bool isValidPasswordRepeat = false;
            bool isValidFirstname = false;  
            bool isValidLastname = false;   
            bool isValidPhonenumber = false;    
            switch (checkUiElemt)
            {
                case 1:
                    isValidEmail = ValidateEmail();
                    break;
                case 2:
                    isValidPassword = ValidatePassword();
                    break;
                case 3:
                    isValidPasswordRepeat = ValidatePasswordRepeat();
                    break;
                case 4:
                    isValidFirstname = ValidateFirstname();
                    break;
                case 5:
                    isValidLastname = ValidateLastname();
                    break;
                case 6:
                    isValidPhonenumber = ValidatePhone();
                    break;
                default:
                    isValidEmail = ValidateEmail();
                    isValidPassword = ValidatePassword();
                    isValidPasswordRepeat = ValidatePasswordRepeat();
                    isValidFirstname = ValidateFirstname();
                    isValidLastname = ValidateLastname();
                    isValidPhonenumber = ValidatePhone();
                    break;
            }
            return isValidEmail && isValidPassword && isValidPasswordRepeat && isValidFirstname && isValidLastname && isValidPhonenumber;
        }
        private void ValidateUiVoid(string checkUiElemt)
        {
            if (int.TryParse(checkUiElemt, out int value))
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
        private bool ValidatePasswordRepeat()
        {
            return PasswordRepeat.Validate();
        }
        private bool ValidateFirstname()
        {
            return Firstname.Validate();
        }
        private bool ValidateLastname()
        {
            return Lastname.Validate();
        }
        private bool ValidatePhone()
        {
            return PhonePrefix.Validate() && Phone.Validate();
        }
        private void AddValidations()
        {
            Firstname.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Firstname entry is required."
            }); 
            Lastname.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Lastname entry is required."
            });
            Email.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Email entry is required."
            });
            Email.Validations.Add(new EmailRule
            {
                ValidationMessage = "Email is invalid."
            });

            Phone.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Phonenumber entry is required."
            });
            PhonePrefix.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Phoneprefix entry is required."
            });

            Password.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Password entry is required."
            });
            Password.Validations.ForEach(x => { PasswordRepeat.Validations.Add(x); });
        }
    }
}
