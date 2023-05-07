using JellyFish.Controls;
using JellyFish.Service;
using JellyFish.ViewModel;

namespace JellyFish;

public partial class ChatPage : CustomContentPage
{

	public ChatPage(ChatPageViewModel chatPageViewModel) 
	{
		InitializeComponent();
		this.BindingContext = chatPageViewModel;	
	}
    private void Editor_Focused(object sender, FocusEventArgs e)
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            
            //ChatTitleBar.TranslateTo(0, 30, 50);
            //MessagesList.TranslateTo(0, -200, 50);
            //ChatEditorControlsGrid.TranslateTo(0, -200, 50);
            //RefreshView.TranslateTo(0, 100, 50);
        }
    }

    private void Editor_Unfocused(object sender, FocusEventArgs e)
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            //ChatTitleBar.TranslateTo(0, 0, 50);
            //MessagesList.TranslateTo(0, 0, 50);
            //ChatEditorControlsGrid.TranslateTo(0, 0, 50);
            //RefreshView.TranslateTo(0, 0, 50);
        }
    }
}