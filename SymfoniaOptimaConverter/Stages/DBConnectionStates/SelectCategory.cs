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
   class SelectCategory : ConnectionState
   {
      private DBConnectionDialog                         m_dlg;

      public SelectCategory( DBConnectionDialog dlg )
      {
         m_dlg = dlg;
      }

      public void Activate()
      {
         m_dlg.okButton.Text = "Category OK";

         // highlight the category name combo box, suggesting that the user should verify his selection
         m_dlg.contractorCategory.BackColor = Color.LightGreen;
      }

      public void Deactivate()
      {
         m_dlg.contractorCategory.BackColor = Color.White;
      }

      public void OnGUIEvent( object sender, EventArgs e )
      {
         if ( sender != m_dlg.okButton && sender != m_dlg.contractorCategory )
         {
            return;
         }

         if ( ValidateCategory() )
         {
            // that's it
            m_dlg.CloseDialog();
         }
      }

      private bool ValidateCategory()
      {
         bool errorsEncountered = false;
         if (m_dlg.contractorCategory.Text.Length <= 0)
         {
            m_dlg.contractorCategory.BackColor = Color.LightPink;
            errorsEncountered = true;
         }
         else
         {
            m_dlg.contractorCategory.BackColor = Color.White;
         }

         return !errorsEncountered;
      }
   }
}
