using System.Collections.Generic;


namespace Domain.OtherModels.EmailService
{
    public class EmailReceiverObject
    {
        public string RecipientName { get; set; }
        public string MailAddress { get; set; }
        public List<MailReceiver> MailAddressList { get; set; }
        public string Subject { get; set; }
        public List<MailReceiver> CCList { get; set; }
        public List<MailReceiver> BCCList { get; set; }
        public List<string> AttachmentNames { get; set; }
        public string FileFormat { get; set; }
        public List<byte[]> Files { get; set; }
        public bool IsPasswordProjectedPDF { get; set; } = false;
        public bool IsPasswordProjectedExcel { get; set; } = false;
        public string Password { get; set; }
    }
}
