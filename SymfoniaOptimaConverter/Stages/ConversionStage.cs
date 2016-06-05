using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Stages
{
   public class ConversionStage : SymfoniaOptimaConverter.Core.Stage
   {
      private XElement[]                        m_invoices;
      private int                               m_processedInvoiceIdx;
      private int                               m_numProcessedInvoices;
      private int                               m_numReceipts;
      private int                               m_numRejectedInvoices;

      public override bool Activate( Form1 form )
      {
         // load the Symfonia doc
         try
         {
            Stream fileStream = form.openFileDialog.OpenFile();
            LoadSymfoniaDoc( fileStream );
            form.log.message( "Input file " + form.openFileDialog.FileName + " successfully loaded" );
         }
         catch( Exception )
         {
            form.log.error( "Can't load the input file: " + form.openFileDialog.FileName );
            return false;
         }

         if ( m_invoices == null || m_invoices.Length == 0 )
         {
            form.log.warning( "No invoices were found in file: " + form.openFileDialog.FileName );
            return false;
         }

         // initialize the progress bar
         InitializeGUI( form );
         m_processedInvoiceIdx = 0;
         form.log.message( m_invoices.Length + " invoices left to process..." );

         m_numRejectedInvoices = 0;

         return true;
      }

      public override void Deactivate( Form1 form )
      {
         // update the GUI
         form.processedInvoiceName = "Processing complete. ";

         // stats
         form.log.message( "--------------------------------------------------" );
         form.log.message( "                STATISTICS" );
         form.log.message( "--------------------------------------------------" );
         form.log.message( "Total number of documents : " + m_invoices.Length );
         form.log.message( "Number of processed invoices : " + m_numProcessedInvoices );
         form.log.message( "Number of rejected invoices : " + m_numRejectedInvoices );
         form.log.message( "Number of ignored receipts : " + m_numReceipts );
      }

      public override SymfoniaOptimaConverter.Core.StageResult Tick( Form1 form )
      {
         XElement invoiceToProcess = m_invoices[m_processedInvoiceIdx++];
         Core.XmlParser parser = new Core.XmlParser( invoiceToProcess );
         
         // convert the invoice
         string invoiceNo = "UNKNOWN";
         string invoiceName = "";
         Optima.Invoice optimaInvoice = null;
         try
         {
            parser.GetMandatoryStr( "NumerDokumentu", ref invoiceNo );
            parser.GetOptionalStr( "Nazwa", ref invoiceName );

            bool isReceipt = invoiceNo.Contains( "MAG/PAR" ) || invoiceName.Contains( "Paragon" );
            
            // ignore receipts, process only actual invoices
            if ( isReceipt )
            {
               ++m_numReceipts;
               form.log.warning( "Document " + invoiceNo + " is a receipt - ignoring" );
            }
            else 
            {
               form.processedInvoiceName = "Processing " + invoiceNo + "...";

               Symfonia.Invoice symfoniaInvoice = new Symfonia.Invoice( invoiceToProcess );
               optimaInvoice = new Optima.Invoice( symfoniaInvoice );
               form.optimaArchive.Add( optimaInvoice );

               ++m_numProcessedInvoices;
               form.log.message( "Invoice " + invoiceNo + " processed correctly" );
            }
         }
         catch( Exception ex )
         {
            // continue with subsequent invoices
            form.log.error( "Invoice " + invoiceNo + " error: " + ex.Message );
            ++m_numRejectedInvoices;
         }

         if ( optimaInvoice != null )
         {
            string formattedInvoiceNo = optimaInvoice.header.m_invoiceNo.Replace( "/", "_" );
            string outputFileName = form.outputDir.FullName + "/" + m_numProcessedInvoices.ToString("D4") + "_" + formattedInvoiceNo + ".xml";
            optimaInvoice.Save( outputFileName, form.log );
         }  

         // update the GUI
         form.progressBar.Value = m_processedInvoiceIdx;

         return ( m_processedInvoiceIdx < m_invoices.Length ) ? SymfoniaOptimaConverter.Core.StageResult.InProgress : SymfoniaOptimaConverter.Core.StageResult.Completed;
      }

      private void LoadSymfoniaDoc( Stream fileStream )
      {
         // load the contents of the XML file
         string xmlFileContents = "";
         {
            StreamReader sr = new StreamReader(fileStream, Encoding.GetEncoding("windows-1250") );
            char[] fileBuf = new char[fileStream.Length];
            if ( sr.Read( fileBuf, 0, fileBuf.Length ) > 0 )
            {
               xmlFileContents = new string( fileBuf );
            }
         }
     
         // parse the file
         List< XElement > invoices = new List<XElement>();
         XDocument readDoc = XDocument.Parse( xmlFileContents );
         foreach( XElement invoiceElem in readDoc.Root.Elements( "DokumentHandlowy" ) )
         {
            invoices.Add( invoiceElem );
         }

         m_invoices = invoices.ToArray();
      }

      private void InitializeGUI( Form1 form )
      {
         form.progressBar.Minimum = 0;
         form.progressBar.Maximum = m_invoices.Length;
         form.progressBar.Step = 1;
         form.progressBar.Value = 0;
      }

   }
}
