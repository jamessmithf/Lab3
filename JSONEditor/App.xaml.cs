namespace JSONEditor
{
    public partial class App : Application
    {
        public static double WindowWidth { get; } = 1280;
        public static double WindowHeight { get; } = 720;
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        /* Перезаписуємо метод створення вікна, щоб задати власну назву та власні розміри для вікна */
        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            if (window != null)
            {
                window.Title = "Редактор JSON";
                window.Width = window.MinimumWidth = window.MaximumWidth = WindowWidth;
                window.Height = window.MinimumHeight = window.MaximumHeight = WindowHeight;
            }
            return window;
        }
    }

}
