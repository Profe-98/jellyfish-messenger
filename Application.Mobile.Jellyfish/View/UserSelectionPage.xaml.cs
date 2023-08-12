using Application.Mobile.Jellyfish.Controls;
using Application.Mobile.Jellyfish.Service;
using Application.Mobile.Jellyfish.ViewModel;

namespace Application.Mobile.Jellyfish.View;

public partial class UserSelectionPage : CustomContentPage
{
	public UserSelectionPage(UserSelectionPageViewModel contactsPageViewModel) 
    {
		InitializeComponent();
		this.BindingContext = contactsPageViewModel;	
	}
}