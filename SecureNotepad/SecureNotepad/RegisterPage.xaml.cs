using System;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SecureNotepad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        async private void Button_Clicked(object sender, EventArgs e)
        {
            var password = txtPassword.Text;

            using (var sha512 = new SHA512Managed())
            {
                var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                //await SecureStorage.SetAsync("password", Encoding.UTF8.GetString(hash));
                Application.Current.Properties.Add("password", Encoding.UTF8.GetString(hash));

                App.Password = password;
                App.PasswordChanged = true;
            }

            await Navigation.PopModalAsync();
        }
    }
}