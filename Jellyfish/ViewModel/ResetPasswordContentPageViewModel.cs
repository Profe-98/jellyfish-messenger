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
    public class ResetPasswordContentPageViewModel : BaseViewModel
    {
        public ValidatableObject<string> Phone { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> Email { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> SmsCodeEntry1 { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> SmsCodeEntry2 { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> SmsCodeEntry3 { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> SmsCodeEntry4 { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> SmsCodeEntry5 { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> SmsCodeEntry6 { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> Password { get; private set; } = new ValidatableObject<string>();
        public ValidatableObject<string> PasswordRepeat { get; private set; } = new ValidatableObject<string>();
        public bool IsLoading
        {
            get; private set;
        }
        private bool _isRequestSent = false;
        public bool IsRequestSent
        {
            get { return _isRequestSent; }
            set
            {
                _isRequestSent = value;
                OnPropertyChanged();
            }
        }
        private bool _isCodeSentToBackendForVerify = false;
        public bool IsCodeSentToBackendForVerify
        {
            get { return _isCodeSentToBackendForVerify; }
            set
            {
                _isCodeSentToBackendForVerify = value;
                OnPropertyChanged();
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
        public ICommand SubmitCodeActionCommand { get; private set; }
        public ICommand SubmitActionCommand { get; private set; }
        public ICommand SubmitPasswordActionCommand { get; private set; }
        public ICommand ValidateByValueChangeCommand { get; private set; }
        public ResetPasswordContentPageViewModel()
        {
            SubmitActionCommand = new RelayCommand(SubmitAction);
            SubmitPasswordActionCommand = new RelayCommand(SubmitPasswordAction);
            SubmitCodeActionCommand = new RelayCommand(SubmitCodeAction);
            ValidateByValueChangeCommand = new RelayCommand<string>(ValidateUiVoid);
            AddValidations();
            Email.Value = "test@roos-it.de";
        }
        public async void SubmitPasswordAction()
        {
            IsLoading = true;
            IsLoading = false;
        }
        public async void SubmitCodeAction()
        {
            IsLoading = true;
            IsCodeSentToBackendForVerify= true; 
            IsLoading = false;
        }
        public async void SubmitAction()
        {
            IsLoading = true;
            bool validEntries = ValidateUi();
            if(validEntries)
            {
                IsRequestSent= true;
            }
            IsLoading = false;
        }
        private bool ValidateUi(int checkUiElemt = -1)
        {
            bool isValidEmail = false;
            bool isValidPhone = false;
            bool isValidPassword = false;
            bool isValidPasswordRepeat = false;
            switch (checkUiElemt)
            {
                case 1:
                    isValidEmail = ValidateEmail();
                    break;
                case 2:
                    isValidPhone = ValidatePhone();
                    break;
                case 3:
                    isValidPassword = ValidatePassword();
                    break;
                case 4:
                    isValidPasswordRepeat = ValidatePasswordRepeat();
                    break;
                default:
                    isValidEmail = ValidateEmail();
                    isValidPhone = ValidatePhone();
                    break;
            }
            return (isValidEmail || isValidPhone) || (isValidPassword&& isValidPasswordRepeat);
        }
        private void ValidateUiVoid(string checkUiElemt)
        {
            if (int.TryParse(checkUiElemt, out int value))
            {
                ValidateUi(value);

            }

        }
        private bool ValidatePassword()
        {
            return Password.Validate();
        }
        private bool ValidatePasswordRepeat()
        {
            return PasswordRepeat.Validate();
        }
        private bool ValidateEmail()
        {
            return Email.Validate();
        }

        private bool ValidatePhone()
        {
            return Phone.Validate();
        }
        private void AddValidations()
        {
            Email.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Email entry is required."
            });
            Email.Validations.Add(new EmailRule
            {
                ValidationMessage = "Email is invalid."
            });

            Phone.Validations.Add(new PhoneNumberRule
            {
                ValidationMessage = "Phone entry is not valid."
            });
            Phone.Validations.Add(new IsNotNullOrEmptyRule
            {
                ValidationMessage = "Phone entry is required."
            });

            SmsCodeEntry1.Validations.Add(new StringIsNumerRule
            {
                ValidationMessage = "Code in Block 1 is NaN."
            });
            SmsCodeEntry2.Validations.Add(new StringIsNumerRule
            {
                ValidationMessage = "Code in Block 2 is NaN."
            });
            SmsCodeEntry3.Validations.Add(new StringIsNumerRule
            {
                ValidationMessage = "Code in Block 3 is NaN."
            });
            SmsCodeEntry4.Validations.Add(new StringIsNumerRule
            {
                ValidationMessage = "Code in Block 4 is NaN."
            });
            SmsCodeEntry5.Validations.Add(new StringIsNumerRule
            {
                ValidationMessage = "Code in Block 5 is NaN."
            });
            SmsCodeEntry6.Validations.Add(new StringIsNumerRule
            {
                ValidationMessage = "Code in Block 6 is NaN."
            });

        }
    }
}
