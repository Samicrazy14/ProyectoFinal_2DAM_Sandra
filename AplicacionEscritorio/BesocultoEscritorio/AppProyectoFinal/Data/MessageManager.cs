using Microsoft.Maui.Layouts;

namespace AppProyectoFinal.Data
{
    class MessageManager
    {
        //Atributos
        private readonly Label messageLabel;
        private ScrollView mainScrollView;
        private Layout contentContainer;
        private readonly Grid loadingOverlay;
        private readonly Page mainPage;

        /// <summary>
        /// Clase para gestionar la visualización de mensajes e indicadores de carga
        /// <list type="number">
        /// <item><param name="page">Página en la que se mostrará</param></item>
        /// <item><param name="scrollView">Parámetro opcional para especificar el scrollView</param></item>
        /// </list>
        /// </summary>
        public MessageManager(Page page, ScrollView scrollView = null)
        {
            mainPage = page;
            mainScrollView = scrollView;

            // Configuración del mensaje
            messageLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TextColor = Colors.White,
                BackgroundColor = Colors.Black,
                Opacity = 0,
                Padding = new Thickness(10),
                ZIndex = 99999,
            };

            loadingOverlay = new Grid
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                IsVisible = false,
                ZIndex = 99999,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            loadingOverlay.Children.Add(new ActivityIndicator { IsRunning = true, Color = Colors.White });

            if (mainScrollView != null)
            {
                if (mainScrollView.Content != null)
                {
                    // Si el ScrollView ya tiene contenido, envuélvelo en un Grid.
                    contentContainer = new Grid();
                    contentContainer.Children.Add(mainScrollView.Content); // Agrega el contenido actual.
                }
                else
                {
                    // Si no tiene contenido, crea un Grid vacío.
                    contentContainer = new Grid();
                }

                // Agrega los nuevos elementos.
                contentContainer.Children.Add(messageLabel);
                contentContainer.Children.Add(loadingOverlay);

                // Asigna el contenedor al ScrollView.
                mainScrollView.Content = contentContainer;
            }
            else
            {
                var containerLayout = new Grid();

                if (mainPage is ContentPage contentPageWithExistingContent && contentPageWithExistingContent.Content != null)
                {
                    containerLayout.Children.Add(contentPageWithExistingContent.Content); // Agrega el contenido actual.
                }

                containerLayout.Children.Add(messageLabel);
                containerLayout.Children.Add(loadingOverlay);
                if (mainPage is ContentPage cp)
                {
                    cp.Content = containerLayout; // Asigna el nuevo layout a la ContentPage
                }
                else if (mainPage is TabbedPage)
                {
                    throw new InvalidOperationException("TabbedPage no puede tener contenido directamente. Usa ContentPage para cada pestaña.");
                }
            }
        }

        /// <summary>
        /// Muestra un mensaje temporal en la pantalla por un período de tiempo especificado.
        /// <list type="number">
        /// <item><param name="message">Texto del mensaje a mostrar</param></item>
        /// <item><param name="duration">Duración en milisegundos del mensaje en pantalla (por defecto, 2000 ms)</param></item>
        /// </list>
        /// </summary>
        public async Task ShowMessage(string message, int duration = 2000)
        {
            await mainPage.Dispatcher.DispatchAsync(() =>
            {
                messageLabel.Text = message;
                messageLabel.Opacity = 1;
            });

            await Task.Delay(duration);

            await mainPage.Dispatcher.DispatchAsync(() =>
            {
                messageLabel.Opacity = 0;
            });
        }

        /// <summary>
        /// Muestra el indicador de carga en pantalla junto con un mensaje.
        /// <list type="number">
        /// <item><param name="message">Texto del mensaje a mostrar</param></item>
        /// </list>
        /// </summary>
        public void ShowLoading(string message)
        {
            mainPage.Dispatcher.Dispatch(() =>
            {
                loadingOverlay.IsVisible = true;
                messageLabel.Text = message;
                messageLabel.Opacity = 1;
            });
        }

        /// <summary>
        /// Oculta el indicador de carga en pantalla.
        /// </summary>
        public void HideLoading()
        {
            mainPage.Dispatcher.Dispatch(() =>
            {
                loadingOverlay.IsVisible = false;
                messageLabel.Opacity = 0;
            });
        }
    }
}
