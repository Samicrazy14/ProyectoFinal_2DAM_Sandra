using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Storage;

namespace AppProyectoFinal
{
    public partial class App : Application
    {
        //Objetos de Firebase
        public static FirebaseAuthClient AuthClient { get; private set; }
        public static FirebaseClient Database { get; private set; }
        public static FirebaseStorage Storage { get; private set; }

        public App()
        {
            InitializeComponent();

            // Configuración del proyecto con Firebase
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyCwNoP4Ke84KTgaLC6uYkiGdLZYRvl40aA",
                AuthDomain = "appproyectofinalsandra-9e477.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };
            AuthClient = new FirebaseAuthClient(config);
            Database = new FirebaseClient("https://appproyectofinalsandra-9e477-default-rtdb.europe-west1.firebasedatabase.app/");
            Storage = new FirebaseStorage("appproyectofinalsandra-9e477.appspot.com");
            MainPage = new AppShell();
        }
    }
}
