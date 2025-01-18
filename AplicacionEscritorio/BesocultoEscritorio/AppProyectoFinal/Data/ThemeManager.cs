using System.ComponentModel;

namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Manejador del tema que permite cambiar entre los colores de items de un tema a otro.
    /// </summary>
    public class ThemeManager : INotifyPropertyChanged
    {
        private static ThemeManager _instance;
        public static ThemeManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ThemeManager();
                return _instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //INITIAL VALUES
        private Color _backgroundPage = Colors.White;
        private Color _textPage = Colors.Black;
        private Color _secondaryColor = Color.FromArgb("#D3D5DB");
        private Color _subSecondaryColor = Color.FromArgb("#313133");

        private string _deleteIcon = "ic_delete_notnight.png";
        private string _moreIcon = "ic_more_notnight.png";
        private string _favIcon = "ic_favourite_filled_notnight.png";
        private string _personIcon = "ic_person_notnight.png";
        private string _openEyeIcon = "ic_eye_open_notnight.png";
        private string _closeEyeIcon = "ic_eye_close_notnight.png";
        private string _marcaIcon = "marca_negro.png";
        private string _logoIcon = "logo_notnight.png";
        private string _clipIcon = "ic_attach_notnight.png";
        private string _commentIcon = "ic_comment_add_night.png";
        private string _categoryIcon = "ic_category_notnight.png";
        private string _pdfIcon = "ic_pdf_black.png";

        //Valores constantes
        public Color PrimaryColor = Color.FromArgb("#DF1995");

        //gets and sets
        public Color BackgroundPage
        {
            get => _backgroundPage;
            set
            {
                if (_backgroundPage != value)
                {
                    _backgroundPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundPage)));
                }
            }
        }
        public Color TextPage
        {
            get => _textPage;
            set
            {
                if (_textPage != value)
                {
                    _textPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextPage)));
                }
            }
        }
        public Color SecondaryColor
        {
            get => _secondaryColor;
            set
            {
                if (_secondaryColor != value)
                {
                    _secondaryColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SecondaryColor)));
                }
            }
        }
        public Color SubSecondaryColor
        {
            get => _subSecondaryColor;
            set
            {
                if (_subSecondaryColor != value)
                {
                    _subSecondaryColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubSecondaryColor)));
                }
            }
        }

        public string DeleteIcon
        {
            get => _deleteIcon;
            set
            {
                if (_deleteIcon != value)
                {
                    _deleteIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeleteIcon)));
                }
            }
        }
        public string MoreIcon
        {
            get => _moreIcon;
            set
            {
                if (_moreIcon != value)
                {
                    _moreIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoreIcon)));
                }
            }
        }
        public string FavIcon
        {
            get => _favIcon;
            set
            {
                if (_favIcon != value)
                {
                    _favIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavIcon)));
                }
            }
        }
        public string PersonIcon
        {
            get => _personIcon;
            set
            {
                if (_personIcon != value)
                {
                    _personIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PersonIcon)));
                }
            }
        }
        public string OpenEyeIcon
        {
            get => _openEyeIcon;
            set
            {
                if (_openEyeIcon != value)
                {
                    _openEyeIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpenEyeIcon)));
                }
            }
        }
        public string CloseEyeIcon
        {
            get => _closeEyeIcon;
            set
            {
                if (_closeEyeIcon != value)
                {
                    _closeEyeIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CloseEyeIcon)));
                }
            }
        }
        public string MarcaIcon
        {
            get => _marcaIcon;
            set
            {
                if (_marcaIcon != value)
                {
                    _marcaIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MarcaIcon)));
                }
            }
        }
        public string LogoIcon
        {
            get => _logoIcon;
            set
            {
                if (_logoIcon != value)
                {
                    _logoIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogoIcon)));
                }
            }
        }
        public string ClipIcon
        {
            get => _clipIcon;
            set
            {
                if (_clipIcon != value)
                {
                    _clipIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClipIcon)));
                }
            }
        }
        public string CommentIcon
        {
            get => _commentIcon;
            set
            {
                if (_commentIcon != value)
                {
                    _commentIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CommentIcon)));
                }
            }
        }
        public string CategoryIcon
        {
            get => _categoryIcon;
            set
            {
                if (_categoryIcon != value)
                {
                    _categoryIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoryIcon)));
                }
            }
        }
        public string PdfIcon
        {
            get => _pdfIcon;
            set
            {
                if (_pdfIcon != value)
                {
                    _pdfIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PdfIcon)));
                }
            }
        }

        /// <summary>
        /// Método para cambiar el tema entre las 2 opciones posibles.
        /// <list type="number">
        /// <param name="selectedTheme">Opción a la que se desea convertir (0: Claro, 1: Oscuro).</param>
        /// </list>
        /// </summary>
        public void UpdateThemeColors(int selectedTheme)
        {
            switch (selectedTheme)
            {
                case 0: // Claro
                    BackgroundPage = Colors.White;
                    TextPage = Colors.Black;
                    SecondaryColor = Color.FromArgb("#D3D5DB");
                    SubSecondaryColor = Color.FromArgb("#313133");

                    DeleteIcon = "ic_delete_notnight.png";
                    MoreIcon = "ic_more_notnight.png";
                    FavIcon = "ic_favourite_filled_notnight.png";
                    PersonIcon = "ic_person_notnight.png";
                    OpenEyeIcon = "ic_eye_open_notnight.png";
                    CloseEyeIcon = "ic_eye_close_notnight.png";
                    MarcaIcon = "marca_negro.png";
                    LogoIcon = "logo_notnight.png";
                    ClipIcon = "ic_attach_notnight.png";
                    CommentIcon = "ic_comment_add_night.png";
                    CategoryIcon = "ic_category_notnight.png";
                    PdfIcon = "ic_pdf_black.png";
                    break;
                case 1: // Oscuro
                    BackgroundPage = Colors.Black;
                    TextPage = Colors.White;
                    SecondaryColor = Color.FromArgb("#313133");
                    SubSecondaryColor = Color.FromArgb("#D3D5DB");

                    DeleteIcon = "ic_delete_night.png";
                    MoreIcon = "ic_more_night.png";
                    FavIcon = "ic_favourite_filled_night.png";
                    PersonIcon = "ic_person_night.png";
                    OpenEyeIcon = "ic_eye_open_night.png";
                    CloseEyeIcon = "ic_eye_close_night.png";
                    MarcaIcon = "marca_blanco.png";
                    LogoIcon = "logo_night.png";
                    ClipIcon = "ic_attach_night.png";
                    CommentIcon = "ic_comment_add_notnight.png";
                    CategoryIcon = "ic_category_night.png";
                    PdfIcon = "ic_pdf.png";
                    break;
            }
        }

    }
}
