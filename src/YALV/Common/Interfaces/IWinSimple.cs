using System;
using System.Windows;

namespace YALV.Common.Interfaces
{
    public interface IWinSimple
    {
        bool? DialogResult { get; set; }

        Window Owner { get; set; }

        void Close();
    }
}