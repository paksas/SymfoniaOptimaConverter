using SymfoniaOptimaConverter.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymfoniaOptimaConverter.Stages.DBConnectionStates
{
   class SelectCompany : ConnectionState
   {
      private DBConnectionDialog                         m_dlg;

      public SelectCompany( DBConnectionDialog dlg )
      {
         m_dlg = dlg;
      }

      public void Activate()
      {
         m_dlg.okButton.Text = "Company OK";

         if ( ValidateCompanyName() )
         {
            // automatically skip to category selection, if the company name's been selected
            m_dlg.SwitchState( DBConnectionDialog.State.PopulateCategoriesList );
         }

         // highlight the company name combo box, suggesting that the user should verify his selection
         m_dlg.companyName.BackColor = Color.LightGreen;
      }

      public void Deactivate()
      {
         m_dlg.companyName.BackColor = Color.White;
      }

      public void OnGUIEvent( object sender, EventArgs e )
      {
         if ( sender != m_dlg.okButton && sender != m_dlg.companyName )
         {
            return;
         }

         if ( ValidateCompanyName() )
         {
            m_dlg.SwitchState( DBConnectionDialog.State.PopulateCategoriesList );
         }
      }

      private bool ValidateCompanyName()
      {
         bool errorsEncountered = false;
         if (m_dlg.selectedCompanyName.Length <= 0)
         {
            m_dlg.companyName.BackColor = Color.LightPink;
            errorsEncountered = true;
         }
         else
         {
            m_dlg.companyName.BackColor = Color.White;
         }

         return !errorsEncountered;
      }
   }
}
