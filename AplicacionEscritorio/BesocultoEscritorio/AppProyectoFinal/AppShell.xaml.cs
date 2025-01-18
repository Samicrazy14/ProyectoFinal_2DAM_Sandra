namespace AppProyectoFinal
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            AutomationProperties.SetIsInAccessibleTree(this, true);

        }
    }
}
