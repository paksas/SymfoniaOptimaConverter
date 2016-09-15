using SymfoniaOptimaConverter.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymfoniaOptimaConverter.Stages.DBConnectionStates
{
   class PopulateCompaniesList : ConnectionState
   {
      private DBConnectionDialog          m_dlg;
      private Task< List<string> >        m_companyNamesCollectionTask;

      public PopulateCompaniesList( DBConnectionDialog dlg )
      {
         m_dlg = dlg;
      }

      public void Activate()
      {
         // if all entries are valid, establish a connection and get the list of companies for the user to choose from
         m_dlg.okButton.Text = "Wait...";
         m_companyNamesCollectionTask = CollectCompaniesList( m_dlg );
         Application.Idle += OnUpdate;
      }

      public void Deactivate()
      {
         Application.Idle -= OnUpdate;
         m_dlg.okButton.Text = "OK";

         m_dlg.companyName.Items.Clear();
         if ( m_companyNamesCollectionTask.Result == null )
         {
            return;
         }

         foreach (string name in m_companyNamesCollectionTask.Result)
         {
            m_dlg.companyName.Items.Add(name);
         }

         // once the companies list has been filled in, look for the name that was selected last
         string lastUsedCompanyName = Application.UserAppDataRegistry.GetValue("SelectedCompanyName") as string;
         int idx = m_dlg.companyName.FindString(lastUsedCompanyName);
         if (idx >= 0)
         {
            m_dlg.companyName.SelectedIndex = idx;
         }
      }

      public void OnGUIEvent( object sender, EventArgs e ) {}

      void OnUpdate(object sender, EventArgs e)
      {
         try
         {
            bool completed = m_companyNamesCollectionTask.Wait(1);
            if (completed)
            {
               m_dlg.SwitchState( DBConnectionDialog.State.SelectCompany );
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Connection error");
            m_dlg.SwitchState( DBConnectionDialog.State.EnterCredentials );
         }
      }

      static async Task<List<string>> CollectCompaniesList( DBConnectionDialog dlg )
      {
         List<string> results = new List<string>();

         using ( SqlConnection connection = new SqlConnection(dlg.connectionString) ) 
         {
            await connection.OpenAsync();

            SqlCommand collectCompaniesNamesCmd = new SqlCommand(s_collectCompaniesNamesSQL, connection);
            using (SqlDataReader entryReader = await collectCompaniesNamesCmd.ExecuteReaderAsync())
            {
               while (await entryReader.ReadAsync())
               {
                  string name = entryReader["name"] as string;
                  if (name.StartsWith("CDN_"))
                  {
                     results.Add(name.Substring(4));
                  }
               }
            }

            connection.Close();
         }

         return results;
      }

      private static string s_collectCompaniesNamesSQL = "SELECT name FROM master..sysdatabases where dbid > 6;";

   }
}
