using System;

namespace MailManager.Config
{
    
    [Serializable]
    public class ConfigEntity
    {
        public string Mail { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public MailAction[] MailActions { get; set; }
        public IdentityMessage[] IdentityMessages { get; set; }
    }

    [Serializable]
    public class MailAction
    {
        public ActionType ActType { get; set; }
        public string ActTypeValue { get; set; }
        public MailAction(){ }

        public MailAction(ActionType actType, string actTypeValue)
        {
            ActType = actType;
            ActTypeValue = actTypeValue;
        }
    }

    [Serializable]
    public class IdentityMessage
    {
        public IdentityType IdType { get; set; }
        public string IdTypeValue { get; set; }
        public IdentityMessage() { }

        public IdentityMessage(IdentityType idType, string idTypeValue)
        {
            IdType = idType;
            IdTypeValue = idTypeValue;
        }
    }

    public enum ActionType
    {
        Notify = 1,
        CopyTo,
        Forward,
        Print
    }

    public enum IdentityType
    {
        To = 1,
        From,
        Title,
        Body
    }
}
