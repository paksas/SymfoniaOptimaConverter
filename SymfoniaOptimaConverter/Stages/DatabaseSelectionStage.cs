using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;

namespace SymfoniaOptimaConverter.Stages
{
   using Dialogs;
   using System.Drawing;

   public class DatabaseSelectionStage : SymfoniaOptimaConverter.Core.Stage
   {
      public override bool Activate(Form1 form)
      {
         DBConnectionDialog dlg = new DBConnectionDialog();

         // keep on showing the dialog until either the user inputs the correct credentials
         // and establishes a connection with the DB, or cancels the operation
         bool operationCompleted = false;
         bool continueExecution = true;
         while ( !operationCompleted )
         {
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
               form.dbConnStr = dlg.connectionString;
               form.companyName = dlg.selectedCompanyName;
               form.contractorCategory = dlg.selectedContractorCategory;
               operationCompleted = true;
            }
            else
            {
               // the user canceling the operation ends the cycle
               operationCompleted = true;
               continueExecution = false;
            }
         }

         return continueExecution;
      }

      public override void Deactivate(Form1 form)
      {
      }

      public override SymfoniaOptimaConverter.Core.StageResult Tick(Form1 form)
      {
         // nothing to do here
         return SymfoniaOptimaConverter.Core.StageResult.Completed;
      }
   }
}
