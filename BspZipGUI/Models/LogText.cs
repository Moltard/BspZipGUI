using BspZipGUI.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Models
{

    /// <summary>
    /// Store the logs and update the GUI automatically
    /// </summary>
    public class LogText : StringWrapper
    {

        #region Attributes

        public string Logs
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Constructor

        public LogText() : base() { }

        #endregion

        #region INotifyPropertyChanged

        public override event PropertyChangedEventHandler PropertyChanged;

        public override void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Logs"));
        }

        #endregion

    }
}
