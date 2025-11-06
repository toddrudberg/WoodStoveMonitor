using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoodStoveMonitor
{
  public partial class frmMain : Form
  {
    private void OnBtnConnect_Click(object sender, EventArgs e)
    {
      if (!_isConnected)
      {
        Connect();
      }
      else
      {
        Disconnect();
      }
    }
  }
}
