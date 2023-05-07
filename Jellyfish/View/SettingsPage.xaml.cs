using JellyFish.Controls;
using JellyFish.Service;
using JellyFish.ViewModel;

namespace JellyFish.View;

public partial class SettingsPage : CustomContentPage
{
	public SettingsPage(SettingsPageViewModel settingsPageViewModel)
    {
		InitializeComponent();
		this.BindingContext = settingsPageViewModel;	
	}
}