using Application.Mobile.Jellyfish.Controls;
using Application.Mobile.Jellyfish.Service;
using Application.Mobile.Jellyfish.ViewModel;

namespace Application.Mobile.Jellyfish.View;

public partial class SettingsPage : CustomContentPage
{
	public SettingsPage(SettingsPageViewModel settingsPageViewModel)
    {
		InitializeComponent();
		this.BindingContext = settingsPageViewModel;	
	}
}