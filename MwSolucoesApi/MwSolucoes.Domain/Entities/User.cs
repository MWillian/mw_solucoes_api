using MwSolucoes.Domain.Enums;
using MwSolucoes.Domain.ValueObjects;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.DomainErrorMessages;
using System.Net.Mail;

namespace MwSolucoes.Domain.Entities
{
    public class User
    {
        private User() { }

        public User(string name, string email, string password, string phoneNumber, string cpf, int role, Address address)
        {
            Validate(name, email, password, phoneNumber, cpf, role, address);
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PasswordHash = password;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public PhoneNumber PhoneNumber { get; private set; }

        public Cpf CPF { get; private set; }

        public UserRoles Role { get; private set; } = UserRoles.Cliente;

        public bool IsActive { get; private set; } = true;

        public Address Address { get; private set; }
        public void Validate(string name, string email, string password, string phoneNumber, string cpf, int role, Address address)
        {
            ValidateName(name);
            ValidateEmail(email);
            ValidatePassword(password);
            ValidatePhoneNumber(phoneNumber);
            ValidateCpf(cpf);
            ValidateRole(role);
            ValidateAddress(address);
        }

        // Validate Methods
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                throw new DomainException(UserErrorMessages.EMPTY_USERNAME);
            }
        }
        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DomainException(UserErrorMessages.EMPTY_EMAIL);
            }
            var isValidEmail = MailAddress.TryCreate(email, out var _);
            if (!isValidEmail)
            {
                throw new DomainException(UserErrorMessages.INVALID_EMAIL);
            }
        }
        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new DomainException(UserErrorMessages.EMPTY_PASSWORD);
            }
        }
        private void ValidatePhoneNumber(string phoneNumber)
        {
            PhoneNumber = new PhoneNumber(phoneNumber);
        }
        private void ValidateCpf(string cpf)
        {
            CPF = new Cpf(cpf);
        }
        private void ValidateRole(int role)
        {
            if (!Enum.IsDefined(typeof(UserRoles), role))
            {
                throw new DomainException(UserErrorMessages.INVALID_ROLE);
            }

            Role = (UserRoles)role;
        }
        private void ValidateAddress(Address address)
        {
            if (address is null)
            {
                throw new DomainException("Endereço é obrigatório.");
            }

            Address = address;
        }

        // Helper Methods
        public bool Deactivate(User user)
        {
            if (!user.IsActive)
            {
                throw new DomainException(UserErrorMessages.USER_ALREADY_INACTIVE);
            }
            user.IsActive = false;
            return true;
        }
    }
}
