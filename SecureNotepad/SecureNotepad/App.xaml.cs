using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SecureNotepad
{
    public partial class App : Application
    {
        static Database database;

        public static string Password { get; set; }

        public static bool PasswordChanged { get; set; }


        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "secnotes.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            SecureStorage.RemoveAll();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
