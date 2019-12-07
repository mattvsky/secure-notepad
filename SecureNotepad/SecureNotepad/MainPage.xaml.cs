using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SecureNotepad
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();

            var isRegistewred = IsRegistered();
            if (!isRegistewred)
                await Navigation.PushModalAsync(new RegisterPage());
        }

        private bool IsRegistered()
        {
            return Application.Current.Properties.TryGetValue("password", out object pwd);
        }

        async private void SignIn(object sender, EventArgs e)
        {
            var isAuthenticated = AuthenticateUser(txtPassword.Text);
            if (isAuthenticated)
            {
                lblError.IsVisible = false;
                App.Password = txtPassword.Text;

                await Navigation.PushAsync(new NotepadPage());
            }
            else
            {
                lblError.IsVisible = true;
            }
        }

        private bool AuthenticateUser(string password)
        {
            using (var sha512 = new SHA512Managed())
            {
                var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                //var p0wd = await SecureStorage.GetAsync("password");

                if(Application.Current.Properties.TryGetValue("password", out object pwd))
                    if (Encoding.UTF8.GetString(hash) == pwd.ToString())
                        return true;

                return false;
            }
        }

        async private void Reset(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Reset", "Are you sure?", "Yes", "Nope");

            if (confirm)
            {
                //SecureStorage.RemoveAll();
                Application.Current.Properties.Clear();
                App.Database.Clear();
                await Navigation.PushModalAsync(new RegisterPage());
            }
        }
    }
}
