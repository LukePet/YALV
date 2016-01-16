using System.Windows.Input;

namespace YALV.Common.Interfaces
{
    public interface ICommandAncestor
        : ICommand
    {
        void OnCanExecuteChanged();
    }
}
