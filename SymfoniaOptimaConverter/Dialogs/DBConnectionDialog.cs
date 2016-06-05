using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SymfoniaOptimaConverter.Dialogs
{
   using SymfoniaOptimaConverter.Stages.DBConnectionStates;

   public partial class DBConnectionDialog : Form
   {
      public enum State
      {
         EnterCredentials,
         PopulateCompaniesList,
         SelectCompany,
         PopulateCategoriesList,
         SelectCategory
      }

      private ConnectionState[]  m_states;
      private State              m_currState;

      public string connectionString
      {
         get { return "Server=" + dbName.Text + ";Database=;User Id=" + userName.Text + ";Password=" + password.Text + ";"; }
      }

      public string selectedCompanyName
      {
         get { return companyName.Text; }
      }

      public int selectedContractorCategory
      {
         get 
         { 
            ContractorCategoryItem item = contractorCategory.SelectedItem as ContractorCategoryItem; 
            return item.m_id;
         }
      }

      public DBConnectionDialog()
      {
         InitializeComponent();
         RestoreSettings();

         m_states = new ConnectionState[] {
            new EnterCredentials( this ),
            new PopulateCompaniesList( this ),
            new SelectCompany( this ),
            new PopulateCategoriesList( this ),
            new SelectCategory( this )
         };

         m_currState = State.EnterCredentials;
         m_states[(int)m_currState].Activate();
      }

      private void RestoreSettings()
      {

         dbName.Text = Application.UserAppDataRegistry.GetValue("DBName") as string;
         userName.Text = Application.UserAppDataRegistry.GetValue("UserName") as string;
         password.Text = Application.UserAppDataRegistry.GetValue("Password") as string;
      }

      private void SaveSettings()
      {
         Application.UserAppDataRegistry.SetValue("DBName", dbName.Text);
         Application.UserAppDataRegistry.SetValue("UserName", userName.Text);
         Application.UserAppDataRegistry.SetValue("Password", password.Text);
         Application.UserAppDataRegistry.SetValue("SelectedCompanyName", selectedCompanyName);
         Application.UserAppDataRegistry.SetValue("SelectedContractorCat", contractorCategory.Text );
      }

      public void SwitchState( State state )
      {
         if ( state == m_currState )
         {
            return;
         }
         m_states[(int)m_currState].Deactivate();
         m_currState = state;
         m_states[(int)m_currState].Activate();
      }

      public void CloseDialog()
      {
         SaveSettings();

         this.DialogResult = DialogResult.OK;
         this.Close();
      }

      private void OK_Click(object sender, EventArgs e)
      {
         m_states[(int)m_currState].OnGUIEvent( sender, e );
      }

      private void companyName_SelectedIndexChanged(object sender, EventArgs e)
      {
         if ( (int)m_currState > (int)State.SelectCompany )
         {
            // selecting a different company after one has already been selected
            // resets the company settings
            SwitchState( State.SelectCompany );
         }

         m_states[(int)m_currState].OnGUIEvent( sender, e );
      }

   }
}
