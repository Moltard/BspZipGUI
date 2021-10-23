using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Models.Abstract
{

    /// <summary>
    /// Wrapper for String that update the GUI
    /// </summary>
    public abstract class StringWrapper : INotifyPropertyChanged
    {
        #region Attributes

        protected string text;

        #endregion

        #region Constructor

        public StringWrapper()
        {
            Clear();
        }

        #endregion

        #region Methods

        public void Append(string str)
        {
            text += str;
            NotifyPropertyChanged();
        }
        public void AppendLine()
        {
            text += "\n";
            NotifyPropertyChanged();
        }
        public void AppendLine(string str)
        {
            text += str + "\n";
            NotifyPropertyChanged();
        }
        public void Clear()
        {
            text = string.Empty;
            NotifyPropertyChanged();
        }

        #endregion

        #region INotifyPropertyChanged

        public abstract event PropertyChangedEventHandler PropertyChanged;

        public abstract void NotifyPropertyChanged();

        #endregion

    }
}
