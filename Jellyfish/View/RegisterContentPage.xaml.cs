using JellyFish.Controls;
using JellyFish.Service;
using JellyFish.ViewModel;

namespace JellyFish.View;

public partial class RegisterContentPage : CustomContentPage
{
	public RegisterContentPage(RegisterContentPageViewModel registerContentPageViewModel)
	{
		InitializeComponent();
		this.BindingContext = registerContentPageViewModel;
	}
}