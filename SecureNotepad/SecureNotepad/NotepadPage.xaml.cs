using SecureNotepad.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SecureNotepad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotepadPage : ContentPage
    {
        const string _salt = "SUPERDUPERSECRETSALT2019";

        public Note Note { get; set; }

        public bool IsLoaded { get; set; }

        public NotepadPage()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!IsLoaded)
            {
                var salt = Encoding.UTF8.GetBytes(_salt);

                Note = await App.Database.GetNote();

                if (Note == null)
                    Note = await App.Database.CreateNote();

                var noteText = Decrypt(Note.CypherText, App.Password, salt);
                txtNote.Text = noteText;

                IsLoaded = true;
            }
        }

        async private void Save(object sender, EventArgs e)
        {
            var salt = Encoding.UTF8.GetBytes(_salt);
            var cypher = Encrypt(txtNote.Text, App.Password, salt);

            Note.CypherText = cypher;
            await App.Database.SaveNoteAsync(Note);

            App.Password = null;
            await Navigation.PopAsync();
        }

        async private void ChangePassword(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ChangePasswordPage());
        }

        protected override bool OnBackButtonPressed()
        {
            var salt = Encoding.UTF8.GetBytes(_salt);
            var cypher = Encrypt(txtNote.Text, App.Password, salt);

            Note.CypherText = cypher;
            App.Database.SaveNoteAsync(Note).Wait();

            App.Password = null;
            return base.OnBackButtonPressed();
        }


        internal static string Encrypt(string textToEncrypt, string encryptionPassword, byte[] salt)
        {
            var algorithm = GetAlgorithm(encryptionPassword, salt);

            //Anything to process?
            if (textToEncrypt == null || textToEncrypt == "") return "";

            byte[] encryptedBytes;
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
            {
                byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);
                encryptedBytes = InMemoryCrypt(bytesToEncrypt, encryptor);
            }
            return Convert.ToBase64String(encryptedBytes);
        }


        internal static string Decrypt(string encryptedText, string encryptionPassword, byte[] salt)
        {
            var algorithm = GetAlgorithm(encryptionPassword, salt);

            //Anything to process?
            if (encryptedText == null || encryptedText == "") return "";

            byte[] descryptedBytes;
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                descryptedBytes = InMemoryCrypt(encryptedBytes, decryptor);
            }
            return Encoding.UTF8.GetString(descryptedBytes);
        }

        private static byte[] InMemoryCrypt(byte[] data, ICryptoTransform transform)
        {
            MemoryStream memory = new MemoryStream();
            using (Stream stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
            {
                stream.Write(data, 0, data.Length);
            }
            return memory.ToArray();
        }

        private static RijndaelManaged GetAlgorithm(string encryptionPassword, byte[] salt)
        {
            // Create an encryption key from the encryptionPassword and salt.
            var key = new Rfc2898DeriveBytes(encryptionPassword, salt);

            // Declare that we are going to use the Rijndael algorithm with the key that we've just got.
            var algorithm = new RijndaelManaged();
            int bytesForKey = algorithm.KeySize / 8;
            int bytesForIV = algorithm.BlockSize / 8;
            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIV);
            return algorithm;
        }
    }
}