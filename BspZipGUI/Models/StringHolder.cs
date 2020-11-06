using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Models
{

    /// <summary>
    /// Wrapper for String that update the GUI
    /// </summary>
    public class StringHolder : INotifyPropertyChanged
    {
        #region Attributes

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        #endregion

        #region Constructor

        public StringHolder()
        {
            Text = string.Empty;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

    }

    /// <summary>
    /// Wrapper for StringBuilder that update the GUI
    /// </summary>
    public class StringBuilderHolder : INotifyPropertyChanged
    {
        #region Attributes

        private StringBuilder _sb;
        public string Text
        {
            get { return _sb.ToString(); }
            set { AppendLine(value); }
        }

        #endregion

        #region Constructor

        public StringBuilderHolder()
        {
            _sb = new StringBuilder();
        }

        #endregion

        #region Methods

        public void Append(string str)
        {
            _sb.Append(str);
            NotifyPropertyChanged("Text");
        }
        public void AppendLine(string str)
        {
            _sb.AppendLine(str);
            NotifyPropertyChanged("Text");
        }
        public void Clear()
        {
            _sb.Clear();
            NotifyPropertyChanged("Text");
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion INotifyPropertyChanged
    }

}
