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

        // стандартный конструктор без параметров для Сериализации
        public ConfigEntity(){ }

        #region Разные способы массив/коллекция
        //public IList<MailAction> MailActionsList { get; set; }
        //public string[,] ActionMas = new string[4,2];
        //private string[,] _actionMas = new string[4,2];
        //public string[,] ActionMas
        //{
        //    get { return _actionMas; }
        //    set
        //    {
        //        //value = массив
        //        //if (value.Length == _actionMas.Length){}

        //        int rows = value.GetUpperBound(0) + 1;
        //        int columns = value.Length / rows;                

        //        for (int i = 0; i < rows; i++)
        //        {
        //            for (int j = 0; j < columns; j++)
        //            {
        //                string element = value[i,j];
        //                _actionMas[i,j] = element;
        //            }   
        //        }                
        //    }
        //}
        #endregion

    }

    [Serializable]
    public class MailAction
    {
        public ActionType ActType { get; set; }
        public string ActTypeValue { get; set; }

        // стандартный конструктор без параметров для Сериализации
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

        // стандартный конструктор без параметров для Сериализации
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
