namespace Lab.Coffe.Domain.ValueObjects;

public class Email
{
    public string Value { get; private set; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be null or empty", nameof(value));
        
        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format", nameof(value));
        
        Value = value.ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is Email email)
            return Value == email.Value;
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }
}
