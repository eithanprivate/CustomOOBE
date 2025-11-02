using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CustomOOBE.Pages;

namespace CustomOOBE
{
    public class PageNavigator
    {
        private ContentControl contentArea;
        private AnimationController animationController;
        private int currentPageIndex = 0;
        private bool hasInternetConnection = false;
        private string deviceName = "Tu Nuevo Equipo";

        public bool IsCompleted { get; private set; } = false;

        public PageNavigator(ContentControl content, AnimationController animation)
        {
            contentArea = content;
            animationController = animation;

            // Obtener el nombre del modelo del equipo
            deviceName = GetDeviceName();
        }

        private string GetDeviceName()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\SystemInformation");

                var model = key?.GetValue("SystemProductName")?.ToString();
                var manufacturer = key?.GetValue("SystemManufacturer")?.ToString();

                if (!string.IsNullOrEmpty(model) && !string.IsNullOrEmpty(manufacturer))
                {
                    return $"{manufacturer} {model}";
                }
            }
            catch { }

            return "Tu Nuevo Equipo";
        }

        public void NavigateToWelcome()
        {
            var welcomePage = new WelcomePage();
            welcomePage.Completed += (s, e) => NavigateToNextPage();

            SetContent(welcomePage);
            animationController.UpdateForPage("Welcome");
        }

        private void NavigateToNextPage()
        {
            currentPageIndex++;

            switch (currentPageIndex)
            {
                case 1:
                    ShowEnjoyPage();
                    break;
                case 2:
                    ShowSetupIntroPage();
                    break;
                case 3:
                    ShowUserSetupPage();
                    break;
                case 4:
                    ShowNetworkSetupPage();
                    break;
                case 5:
                    if (hasInternetConnection)
                        ShowProgramsPage();
                    else
                        NavigateToNextPage(); // Saltar programas si no hay internet
                    break;
                case 6:
                    ShowPersonalizationPage();
                    break;
                case 7:
                    if (hasInternetConnection)
                        ShowReviewPage();
                    else
                        NavigateToNextPage(); // Saltar reseña si no hay internet
                    break;
                case 8:
                    ShowCompletionPage();
                    break;
                default:
                    // Finalizado
                    break;
            }
        }

        private void ShowEnjoyPage()
        {
            var enjoyPage = new EnjoyPage(deviceName);
            enjoyPage.Completed += (s, e) => NavigateToNextPage();

            SetContentWithFade(enjoyPage);
            animationController.UpdateForPage("Enjoy");
        }

        private void ShowSetupIntroPage()
        {
            var setupIntroPage = new SetupIntroPage();
            setupIntroPage.Completed += (s, e) => NavigateToNextPage();

            SetContentWithFade(setupIntroPage);
            animationController.UpdateForPage("SetupIntro");
        }

        private void ShowUserSetupPage()
        {
            var userSetupPage = new UserSetupPage();
            userSetupPage.Completed += (s, e) => NavigateToNextPage();

            SetContentWithFade(userSetupPage);
            animationController.UpdateForPage("User");
        }

        private void ShowNetworkSetupPage()
        {
            var networkSetupPage = new NetworkSetupPage();
            networkSetupPage.Completed += (s, isConnected) =>
            {
                hasInternetConnection = isConnected;
                NavigateToNextPage();
            };

            SetContentWithFade(networkSetupPage);
            animationController.UpdateForPage("Network");
        }

        private void ShowProgramsPage()
        {
            var programsPage = new ProgramsPage();
            programsPage.Completed += (s, e) => NavigateToNextPage();

            SetContentWithFade(programsPage);
            animationController.UpdateForPage("Programs");
        }

        private void ShowPersonalizationPage()
        {
            var personalizationPage = new PersonalizationPage();
            personalizationPage.Completed += (s, e) => NavigateToNextPage();

            SetContentWithFade(personalizationPage);
            animationController.UpdateForPage("Personalization");
        }

        private void ShowReviewPage()
        {
            var reviewPage = new ReviewPage();
            reviewPage.Completed += (s, e) => NavigateToNextPage();

            SetContentWithFade(reviewPage);
            animationController.UpdateForPage("Review");
        }

        private void ShowCompletionPage()
        {
            var completionPage = new CompletionPage();
            completionPage.Completed += (s, e) =>
            {
                IsCompleted = true;
            };

            SetContentWithFade(completionPage);
            animationController.UpdateForPage("Completion");
        }

        private void SetContent(UIElement content)
        {
            contentArea.Content = content;
        }

        private void SetContentWithFade(UIElement content)
        {
            // Fade out actual
            if (contentArea.Content != null)
            {
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
                fadeOut.Completed += (s, e) =>
                {
                    // Cambiar contenido
                    contentArea.Content = content;
                    contentArea.Opacity = 0;

                    // Fade in nuevo
                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500))
                    {
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };
                    contentArea.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                contentArea.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
            else
            {
                // Primera vez, solo mostrar
                contentArea.Content = content;
                contentArea.Opacity = 1;
            }
        }
    }
}
