using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymfoniaOptimaConverter.Core
{
   public class Logger
   {
      private ListView        m_logListView;

      /**
       * Constructor.
       * 
       * @param logListBox
       */
      public Logger( ListView logListView )
      {
         m_logListView = logListView;
      }

      /**
       * Logs an info message.
       * 
       * @param msg
       */
      public void message( string msg )
      {
         m_logListView.Items.Add( new ListViewItem( msg ) );
         m_logListView.Items[m_logListView.Items.Count - 1].ForeColor = Color.Black;
      }

      /**
       * Logs a warning.
       * 
       * @param msg
       */
      public void warning( string msg )
      {
         m_logListView.Items.Add( new ListViewItem( msg ) );
         m_logListView.Items[m_logListView.Items.Count - 1].ForeColor = Color.Brown;
      }

      /**
       * Logs an error.
       * 
       * @param msg
       */
      public void error( string msg )
      {
         m_logListView.Items.Add( new ListViewItem( msg ) );
         m_logListView.Items[m_logListView.Items.Count - 1].ForeColor = Color.Red;
      }
   }
}
