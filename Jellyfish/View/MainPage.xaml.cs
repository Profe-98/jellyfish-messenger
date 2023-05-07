using JellyFish.Controls;
using JellyFish.Service;
using JellyFish.ViewModel;

namespace JellyFish
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