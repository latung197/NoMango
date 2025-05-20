using PlastMB.Helper;
using System.Windows.Forms;

namespace PlastMB.UserControls
{
public partial class LoadingBox : UserControl
{
    public string Title
    {
        get => label1.Text;
        set => label1.SetPropertyThreadSafe(() => label1.Text, value);
    }

    public LoadingBox()
    {
        InitializeComponent();
    }
}
}