using SymfoniaOptimaConverter.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymfoniaOptimaConverter.Stages.DBConnectionStates
{
   interface ConnectionState
   {
      void Activate();
      void Deactivate();
      void OnGUIEvent( object sender, EventArgs e );
   }
}
