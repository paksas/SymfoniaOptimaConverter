using SymfoniaOptimaConverter.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymfoniaOptimaConverter.Stages.DBConnectionStates
{
   class PopulateCategoriesList : ConnectionState
   {
      private DBConnectionDialog                      m_dlg;
      private Task< List<ContractorCategoryItem> >        m_categoriesCollectionTask;

      public PopulateCategoriesList( DBConnectionDialog dlg )
      {
         m_dlg = dlg;
      }

      public void Activate()
      {
         // if all entries are valid, establish a connection and get the list of companies for the user to choose from
         m_dlg.okButton.Text = "Wait...";
         m_categoriesCollectionTask = CollectCategories( m_dlg );
         Application.Idle += OnUpdate;
      }

      public void Deactivate()
      {
         Application.Idle -= OnUpdate;

         m_dlg.contractorCategory.Items.Clear();
         foreach (ContractorCategoryItem category in m_categoriesCollectionTask.Result)
         {
            m_dlg.contractorCategory.Items.Add(category);
         }

         // once the categories list has been filled in, look for the name that was selected last
         string lastUsedCategoryName = Application.UserAppDataRegistry.GetValue("SelectedContractorCat") as string;
         int idx = m_dlg.contractorCategory.FindString(lastUsedCategoryName);
         if (idx >= 0)
         {
            m_dlg.contractorCategory.SelectedIndex = idx;
         }
      }

      public void OnGUIEvent( object sender, EventArgs e ) {}

      void OnUpdate(object sender, EventArgs e)
      {
         try
         {
            bool completed = m_categoriesCollectionTask.Wait(1);
            if (completed)
            {
               m_dlg.SwitchState( DBConnectionDialog.State.SelectCategory );
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Connection error");
            m_dlg.SwitchState( DBConnectionDialog.State.EnterCredentials );
         }
      }

      static async Task< List<ContractorCategoryItem> > CollectCategories( DBConnectionDialog dlg )
      {
         List<ContractorCategoryItem> results = new List<ContractorCategoryItem>();
         CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

         using ( SqlConnection connection = new SqlConnection(dlg.connectionString) ) 
         {
            await connection.OpenAsync();

            string collectCategoriesSQL = string.Format( s_collectCategoriesSQL, dlg.selectedCompanyName );
            SqlCommand collectCategoriesCmd = new SqlCommand(collectCategoriesSQL, connection);
            using (SqlDataReader entryReader = await collectCategoriesCmd.ExecuteReaderAsync())
            {
               while (await entryReader.ReadAsync())
               {
                  ContractorCategoryItem category = new ContractorCategoryItem();

                  string catIdStr = entryReader["Kat_KatID"].ToString();
                  category.m_id = int.Parse( catIdStr, NumberStyles.Any, ci );

                  category.m_name = entryReader["Kat_Opis"].ToString();
                  results.Add(category);
               }
            }

            connection.Close();
         }

         return results;
      }

      private static string s_collectCategoriesSQL = "SELECT Kat_KatID, Kat_Opis FROM [CDN_{0}].[CDN].[Kategorie];";

   }
}
