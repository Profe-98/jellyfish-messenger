using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.Validation
{
    public class ValidatableObject : ObservableObject
    {
        protected IEnumerable<string> _errors;
        protected bool _isValid;
        public IEnumerable<string> Errors
        {
            get => _errors;
            protected set => SetProperty(ref _errors, value);
        }
        public bool IsValid
        {
            get => _isValid;
            protected set => SetProperty(ref _isValid, value);
        }
        public virtual bool Validate()
        {
            IsValid = !Errors.Any();
            return IsValid;
        }
    }
    public class ValidatableObject<T> : ValidatableObject
    {
        private T _value;
        public List<IValidationRule<T>> Validations { get; } = new();
        
        public T Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        public ValidatableObject()
        {
            _isValid = true;
            _errors = Enumerable.Empty<string>();
        }
        public override bool Validate()
        {
            Errors = Validations
                ?.Where(v => !v.Check(Value))
                ?.Select(v => v.ValidationMessage)
                ?.ToArray()
                ?? Enumerable.Empty<string>();
            IsValid = !Errors.Any();
            return IsValid;
        }
    }
}
