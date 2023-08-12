using Application.Mobile.Jellyfish.Controls;
using Application.Mobile.Jellyfish.Service;
using Application.Mobile.Jellyfish.ViewModel;

namespace Application.Mobile.Jellyfish
{
    public partial class MainPage : CustomContentPage
    {


        public MainPage(MainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
            this.BindingContext = mainPageViewModel;
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {

        }

    }
}