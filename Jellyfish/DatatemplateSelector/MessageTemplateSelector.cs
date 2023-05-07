using JellyFish.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JellyFish.DatatemplateSelector
{

    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ReceiveMessageTemplate { get; set; }
        public DataTemplate SendMessageTemplate { get; set; }
        public DataTemplate LinkSendMessageTemplate { get; set; }
        public DataTemplate LinkReceiveMessageTemplate { get; set; }
        public DataTemplate GpsSendMessageTemplate { get; set; }
        public DataTemplate GpsReceiveMessageTemplate { get; set; }
        public DataTemplate ImageSendMessageTemplate { get; set; }
        public DataTemplate ImageReceiveMessageTemplate { get; set; }
        public DataTemplate ContactSendMessageTemplate { get; set; }
        public DataTemplate ContactReceiveMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var msg = (Message)item;
            var selectedTemplate = msg.Received ? (msg.IsLink ? LinkReceiveMessageTemplate : msg.IsGpsMessage ? GpsReceiveMessageTemplate : (msg.IsImg ? ImageReceiveMessageTemplate : (msg.IsContact? ContactReceiveMessageTemplate : ReceiveMessageTemplate))) :
                msg.IsLink ? LinkSendMessageTemplate : msg.IsGpsMessage ? GpsSendMessageTemplate : (msg.IsImg?ImageSendMessageTemplate: (msg.IsContact?ContactSendMessageTemplate: SendMessageTemplate));
            if (selectedTemplate == null)
            {
                throw new ArgumentNullException();
            }
            return selectedTemplate;
        }
    }
}
