using JellyFish.Controls;
using JellyFish.Service;
using JellyFish.ViewModel;

namespace JellyFish.View;

public partial class ContactsPage : CustomContentPage
{
	public ContactsPage(ContactsPageViewModel contactsPageViewModel) 
    {
		InitializeComponent();
		this.BindingContext = contactsPageViewModel;	
	}
}