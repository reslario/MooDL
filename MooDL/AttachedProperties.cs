using System.Security;
using System.Windows;

namespace MooDL
{
    internal static class AttachedProperties
    {
        public static readonly DependencyProperty EncryptedPasswordProperty
            = DependencyProperty.RegisterAttached("EncryptedPassword", typeof(SecureString),
                typeof(AttachedProperties));

        public static SecureString GetEncryptedPassword(DependencyObject obj)
            => (SecureString) obj.GetValue(EncryptedPasswordProperty);

        public static void SetEncryptedPassword(DependencyObject obj, SecureString value)
            => obj.SetValue(EncryptedPasswordProperty, value);
    }
}