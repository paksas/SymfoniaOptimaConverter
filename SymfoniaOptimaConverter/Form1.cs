using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Reflection;
using System.Data.SqlClient;


namespace SymfoniaOptimaConverter
{
   public partial class Form1 : Form
   {
      private Core.Stage[]       m_stages;
      private int                m_currentStage;
      private Core.Logger        m_logger;

      private DirectoryInfo      m_outputDir;
      private string             m_dbConnStr;
      private string             m_companyName;
      private int                m_contractorCategory = 0;
      private Optima.Archive     m_optimaArchive = new Optima.Archive();


      public Form1()
      {
         InitializeComponent();

         // create a logger
         m_logger = new Core.Logger( m_listView );

         // define stages
         m_stages = new Core.Stage[] {
            new Stages.InputFileSelectionStage(),
            new Stages.ConversionStage(),
            new Stages.DatabaseSelectionStage(),
            new Stages.UploadContactsToDB(),
            new Stages.SaveContactsToFile(),
         };

         // activate the first stage
         m_currentStage = -1;
         Application.Idle += HandleApplicationIdle;
      }

      void HandleApplicationIdle (object sender, EventArgs e) 
      {
         bool advanceToNextStage = false;
         if ( m_currentStage >= 0 )
         {
            SymfoniaOptimaConverter.Core.StageResult stageResult = m_stages[m_currentStage].Tick( this );

            switch( stageResult )
            {
               default: // fallthrough
               case SymfoniaOptimaConverter.Core.StageResult.InProgress:
                  {
                     break;
                  }

               case SymfoniaOptimaConverter.Core.StageResult.Completed:
                  {
                     m_stages[m_currentStage].Deactivate( this );
                     advanceToNextStage = true;
                     break;
                  }

               case SymfoniaOptimaConverter.Core.StageResult.Failed:
                  {
                     m_stages[m_currentStage].Deactivate( this );
                     advanceToNextStage = false;
                     Application.Idle -= HandleApplicationIdle;
                     break;
                  }
            }
         }
         else
         {
            advanceToNextStage = true;
         }

         if ( advanceToNextStage )
         {
            Application.Idle -= HandleApplicationIdle;
            m_currentStage++;

            bool nextStageAvailable = false;
            if ( m_currentStage < m_stages.Length )
            {
               nextStageAvailable = m_stages[m_currentStage].Activate( this );
            }

            if ( !nextStageAvailable )
            {
               //we don't have any more stages to go through - stop the update loop
               return;
            }
            Application.Idle += HandleApplicationIdle;
         }
      }

      private void Form1_Load(object sender, EventArgs e)
      {

      }

      public System.Windows.Forms.ProgressBar progressBar
      {
         get { return m_progressBar; }
      }

      public System.Windows.Forms.OpenFileDialog openFileDialog
      {
         get { return m_openFileDialog; }
      }

      public DirectoryInfo outputDir
      {
         get { return m_outputDir; }
         set { m_outputDir = value; }
      }

      public string processedInvoiceName
      {
         get { return m_processedInvoiceName.Text; }

         set { m_processedInvoiceName.Text = value; }
      }

      public string dbConnStr
      {
         get { return m_dbConnStr; }
         set { m_dbConnStr = value; }
      }

      public Optima.Archive optimaArchive
      {
         get { return m_optimaArchive; }
      }

      public string companyName
      {
         get { return m_companyName; }
         set { m_companyName = value; }
      }

      public string companyNameSQL
      {
         get { return "CDN_" + m_companyName; }
      }

      public int contractorCategory
      {
         get { return m_contractorCategory; }
         set { m_contractorCategory = value; }
      }

      public Core.Logger log
      {
         get { return m_logger; }
      }
   }
}
