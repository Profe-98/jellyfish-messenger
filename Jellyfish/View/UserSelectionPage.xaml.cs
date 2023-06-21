using JellyFish.Controls;
using JellyFish.Service;
using JellyFish.ViewModel;

namespace JellyFish.View;

public partial class UserSelectionPage : CustomContentPage
{
	public UserSelectionPage(UserSelectionPageViewModel contactsPageViewModel) 
    {
		InitializeComponent();
		this.BindingContext = contactsPageViewModel;	
	}
}