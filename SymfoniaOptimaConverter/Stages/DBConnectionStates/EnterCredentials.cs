using SymfoniaOptimaConverter.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymfoniaOptimaConverter.Stages.DBConnectionStates
{
   class EnterCredentials : ConnectionState
   {
      private DBConnectionDialog          m_dlg;

      public EnterCredentials( DBConnectionDialog dlg )
      {
         m_dlg = dlg;
      }

      public void Activate()
      {
         m_dlg.okButton.Text = "Connect";
      }

      public void Deactivate()
      {
      }

      public void OnGUIEvent( object sender, EventArgs e )
      {
         if ( sender == m_dlg.okButton && ValidateCredentials() )
         {
            m_dlg.SwitchState( DBConnectionDialog.State.PopulateCompaniesList );
         }
      }

      private bool ValidateCredentials()
      {
         bool errorsEncountered = false;
         if (m_dlg.dbName.Text.Length <= 0)
         {
            m_dlg.dbName.BackColor = Color.LightPink;
            errorsEncountered = true;
         }
         else
         {
            m_dlg.dbName.BackColor = Color.White;
         }

         if (m_dlg.userName.Text.Length <= 0)
         {
            m_dlg.userName.BackColor = Color.LightPink;
            errorsEncountered = true;
         }
         else
         {
            m_dlg.userName.BackColor = Color.White;
         }

         return !errorsEncountered;
      }
   }
}
