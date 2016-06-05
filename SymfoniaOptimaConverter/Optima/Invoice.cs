using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Optima
{
   public class Invoice
   {
      private SymfoniaOptimaConverter.Optima.RegularInvoice.Invoice        m_regularInvoice;


      public Invoice( SymfoniaOptimaConverter.Symfonia.Invoice symfoniaInvoice )
      {
         m_regularInvoice = new SymfoniaOptimaConverter.Optima.RegularInvoice.Invoice( symfoniaInvoice );
      }

      /**
       * Invoice header.
       */
      public RegularInvoice.Header header
      {
         get { return m_regularInvoice.header; }
      }

      public Contractor GetContractor( Core.ContractorRole role )
      {
         switch( role )
         {
            case Core.ContractorRole.Issuer:
               {
                  return m_regularInvoice.header.issuer;
               }

            case Core.ContractorRole.Buyer:
               {
                  return m_regularInvoice.header.buyer;
               }
         }

         return null;
      }

      /**
       * Saves the invoice into the specified directory as an XML file.
       * 
       * @param dirName
       * @param dirName
       */
      public void Save( string fileName, Core.Logger log )
      {
         m_regularInvoice.Save( fileName, log );
      }
   }
}
