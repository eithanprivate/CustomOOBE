using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CustomOOBE
{
    public class AnimationController
    {
        private Canvas canvas;
        private List<Shape> animatedShapes = new List<Shape>();
        private Random random = new Random();

        public AnimationController(Canvas animationCanvas)
        {
            canvas = animationCanvas;
        }

        public void StartBackgroundAnimation()
        {
            // Crear formas animadas al estilo de macOS
            for (int i = 0; i < 20; i++)
            {
                CreateAnimatedCircle();
            }
        }

        private void CreateAnimatedCircle()
        {
            var ellipse = new Ellipse
            {
                Width = random.Next(30, 100),
                Height = random.Next(30, 100),
                Fill = new SolidColorBrush(Color.FromArgb(
                    (byte)random.Next(20, 60),
                    (byte)random.Next(100, 255),
                    (byte)random.Next(100, 255),
                    (byte)random.Next(100, 255)
                )),
                RenderTransform = new TranslateTransform()
            };

            Canvas.SetLeft(ellipse, random.Next(0, 800));
            Canvas.SetTop(ellipse, random.Next(0, 600));

            canvas.Children.Add(ellipse);
            animatedShapes.Add(ellipse);

            // Animación de movimiento
            AnimateShape(ellipse);
        }

        private void AnimateShape(Shape shape)
        {
            var transform = (TranslateTransform)shape.RenderTransform;

            var storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };

            var animX = new DoubleAnimation
            {
                From = Canvas.GetLeft(shape),
                To = random.Next(0, 800),
                Duration = TimeSpan.FromSeconds(random.Next(10, 20)),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            var animY = new DoubleAnimation
            {
                From = Canvas.GetTop(shape),
                To = random.Next(0, 600),
                Duration = TimeSpan.FromSeconds(random.Next(10, 20)),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(animX, shape);
            Storyboard.SetTargetProperty(animX, new PropertyPath("(Canvas.Left)"));

            Storyboard.SetTarget(animY, shape);
            Storyboard.SetTargetProperty(animY, new PropertyPath("(Canvas.Top)"));

            storyboard.Children.Add(animX);
            storyboard.Children.Add(animY);

            storyboard.Begin();
        }

        public void UpdateForPage(string pageName)
        {
            // Cambiar animación según la página actual
            switch (pageName)
            {
                case "Welcome":
                    // Animación de bienvenida
                    break;
                case "User":
                    // Animación de usuario
                    break;
                // ... más casos
            }
        }
    }
}
