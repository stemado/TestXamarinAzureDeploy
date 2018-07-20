namespace Shopping.DemoApp.UWP
{
    using DemoApp.Helpers;
    using Microsoft.ProjectOxford.Emotion.Contract;
    using Services;
    using Helpers;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;
    using Microsoft.Azure.CognitiveServices.Vision.Face;
    using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

    public sealed partial class Rating : Page
    {
        private MediaCaptureHelper mediaCaptureHelper;

        public Rating()
        {
            this.InitializeComponent();
            this.Loaded += this.Rating_Loaded;
            this.Unloaded += this.Rating_Unloaded;

            this.mediaCaptureHelper = new MediaCaptureHelper();
        }
     
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.capturedImage.Source = null;
            
            VisualStateManager.GoToState(this, ProcessingState.Name, true);

            this.processedControl.Blink();

            var photoStream = await this.mediaCaptureHelper.TakePhotoAsync();

            photoStream = await this.mediaCaptureHelper.RotatePhoto(photoStream);

            await this.SetCapturedImage(photoStream);
                  private const string subscriptionKey = "51522335952d4d97bc59b7f20fce1ae2";
                  
        private const string baseUri =
            "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";

        private readonly IFaceClient emotionService = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        IList<DetectedFace> faceList;   // The list of detected faces.
        String[] faceDescriptions;      // The list of descriptions for the detected faces.
        double resizeFactor;            // The resize factor for the displayed image.

            var detectedEmotions = await emotionService.RecognizeAsync(photoStream.AsStream());

            Emotion emotion = detectedEmotions.FirstOrDefault();

            if (emotion != null)
            {
                var texts = emotionService.GetTextsFor(emotion.Scores.Happiness);
                this.TextState.Text = texts.Top;
                this.processedControl.Happiness = emotion.Scores.Happiness;
                this.processedControl.Text = texts.Message;
                VisualStateManager.GoToState(this, RateState.Name, true);
            }
            else
            {
                await Dialog.AlertAsync("No face detected. Please, try again.");
                Dialog.HideLoading();
                VisualStateManager.GoToState(this, CaptureState.Name, true);
            }

        }

        private async void Rating_Loaded(object sender, RoutedEventArgs e)
        {
            await this.mediaCaptureHelper.InitializeCameraAsync(this.PreviewControl);

            this.mediaCaptureHelper.RegisterOrientationEventHandlers();

            VisualStateManager.GoToState(this, CaptureState.Name, true);
        }

        private async void Rating_Unloaded(object sender, RoutedEventArgs e)
        {
            this.mediaCaptureHelper.UnregisterOrientationEventHandlers();
            await this.mediaCaptureHelper.StopPreviewAsync(Dispatcher);
            this.mediaCaptureHelper.Dispose();
        }


        private async Task SetCapturedImage(InMemoryRandomAccessStream photoStream)
        {
            var bmp = new BitmapImage();
            
            await bmp.SetSourceAsync(photoStream as IRandomAccessStream);

            this.capturedImage.Source = bmp;
            this.capturedImage.Visibility = Visibility.Visible;
            photoStream.Seek(0);
        }

        private void repeatButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, CaptureState.Name, true);
        }

        private void submitPhoto_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).RootFrame.GoBack();
        }
    }
}
