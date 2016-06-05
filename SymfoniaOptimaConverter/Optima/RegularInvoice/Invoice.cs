using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Optima.RegularInvoice
{
   class Invoice
   {
      private Header m_header;
      private List<Item> m_items = new List<Item>();
      private List<Discount> m_discounts = new List<Discount>();

      /**
       * Invoice header.
       */
      public Header header
      {
         get { return m_header; }
      }

      /**
       * Constructor.
       */
      public Invoice( SymfoniaOptimaConverter.Symfonia.Invoice symfoniaInvoice )
      {
         m_header = new Header( symfoniaInvoice.header );
         
         m_header.issuer = new Contractor( symfoniaInvoice.issuer );
         m_header.buyer = new Contractor( symfoniaInvoice.buyer );

         foreach( SymfoniaOptimaConverter.Symfonia.Item symfoniaItem in symfoniaInvoice.items )
         {
            Item optimaItem = new Item( symfoniaItem );
            m_items.Add( optimaItem );
         }
      }

      /**
       * Saves the invoice into the specified directory as an XML file.
       * 
       * @param dirName
       * @param dirName
       */
      public void Save(string fileName, Core.Logger log)
      {

         try
         {
            XDocument d = new XDocument();
            d.Declaration = new XDeclaration("1.0", "utf-8", "true");

            XElement root = new XElement("ROOT", new XAttribute("XExmlns", "http://www.cdn.com.pl/optima/dokument"));
            d.Add(root);

            SaveToXml(root);

            d.Save(fileName);
            log.message("Invoices file: " + fileName + " was successfully created");
         }
         catch (Exception ex)
         {
            log.error("Can't save the invoices file: " + fileName + " {" + ex.Message + "}");
         }
      }

      /**
       * Saves the invoice in the specified file in XML format.
       * 
       * @param fileName
       */
      private void SaveToXml(XContainer xmlDoc)
      {
         // process the items
         foreach (Item item in m_items)
         {
            m_header.Process(item);
         }

         // save the doc
         XElement doc = new XElement("DOKUMENT");
         xmlDoc.Add(doc);

         m_header.SaveToXml(doc);

         XElement items = new XElement("POZYCJE");
         doc.Add(items);
         foreach (Item item in m_items)
         {
            item.SaveToXml(items);
         }
      }
   }
}
