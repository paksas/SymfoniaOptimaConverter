using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Symfonia
{
   public class Invoice
   {
      private  Header            m_header;
      private  Contractor        m_issuer;
      private  Contractor        m_buyer;
      private  List<Item>        m_items = new List<Item>();

      /**
       * Constructor.
       * 
       * @param invoiceElem
       */
      internal Invoice( XElement invoiceElem )
      {
         m_header = new Header( invoiceElem );
         m_issuer = new Contractor( invoiceElem, Core.ContractorRole.Issuer );
         m_buyer = new Contractor( invoiceElem, Core.ContractorRole.Buyer );

         foreach( XElement itemElem in invoiceElem.Elements( "PozycjaDokumentu" ) )
         {
            m_items.Add( new Item( m_header, itemElem ) );
         }
      }

      /**
       * Returns the invoice number.
       */
      public string invoiceNo
      {
         get { return m_header.m_invoiceNo; }
      }

      public Header header 
      {
         get { return m_header; }
      }

      public Contractor issuer 
      {
         get { return m_issuer; }
      }

      public Contractor buyer 
      {
         get { return m_buyer; }
      }

      public List<Item> items
      {
         get { return m_items; }
      }
   }
}
