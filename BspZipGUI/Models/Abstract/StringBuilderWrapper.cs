using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Models.Abstract
{
    /// <summary>
    /// Wrapper for StringBuilder that update the GUI
    /// </summary>
    public abstract class StringBuilderWrapper : INotifyPropertyChanged
    {
        #region Attributes

        private readonly StringBuilder _sb;

        /* public string Text
         {
             get => _sb.ToString();
             set => AppendLine(value);
         }*/

        #endregion

        #region Constructor

        public StringBuilderWrapper()
        {
            _sb = new StringBuilder();
        }

        public StringBuilderWrapper(StringBuilder sb)
        {
            _sb = sb;
        }

        #endregion

        #region Methods

        public void Append(string str)
        {
            _sb.Append(str);
            NotifyPropertyChanged();
        }
        public void AppendLine(string str)
        {
            _sb.AppendLine(str);
            NotifyPropertyChanged();
        }
        public void Clear()
        {
            _sb.Clear();
            NotifyPropertyChanged();
        }

        #endregion

        #region INotifyPropertyChanged

        public abstract event PropertyChangedEventHandler PropertyChanged;

        public abstract void NotifyPropertyChanged();

        #endregion INotifyPropertyChanged
    }

}
