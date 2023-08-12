using Application.Mobile.Jellyfish.ControlExtension;
using Application.Mobile.Jellyfish.Controls;
using Application.Mobile.Jellyfish.Model;
using Application.Mobile.Jellyfish.Service;
using Application.Mobile.Jellyfish.ViewModel;

namespace Application.Mobile.Jellyfish;

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

    private void MessagesList_BindingContextChanged(object sender, EventArgs e)
    {

        
    }

    private void MessagesList_Loaded(object sender, EventArgs e)
    {
        var sv = MessagesList;
        sv.ScrollToEnd(typeof(MessageGroup));
    }
}