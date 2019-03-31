using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MooDL
{
    static class AttachedProperties
    {
        public static SecureString GetEncryptedPassword(DependencyObject obj)
            => (SecureString)obj.GetValue(EncryptedPasswordProperty);

        public static void SetEncryptedPassword(DependencyObject obj, SecureString value) 
            => obj.SetValue(EncryptedPasswordProperty, value);

        public static readonly DependencyProperty EncryptedPasswordProperty 
            = DependencyProperty.RegisterAttached("EncryptedPassword", typeof(SecureString), typeof(AttachedProperties));
    }
}
