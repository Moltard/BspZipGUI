using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Models
{
    public class StringHolder : INotifyPropertyChanged
    {
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

        public StringHolder()
        {
            Text = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class StringBuilderHolder : INotifyPropertyChanged
    {
        private StringBuilder _sb = new StringBuilder();
        public string Text
        {
            get { return _sb.ToString(); }
        }

        public StringBuilderHolder()
        {
            _sb = new StringBuilder();
        }

        public void Append(string str)
        {
            _sb.Append(str);
            this.NotifyPropertyChanged("Text");
        }
        public void AppendLine(string str)
        {
            _sb.AppendLine(str);
            this.NotifyPropertyChanged("Text");
        }
        public void Clear()
        {
            _sb.Clear();
            this.NotifyPropertyChanged("Text");
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion INotifyPropertyChanged
    }

}
