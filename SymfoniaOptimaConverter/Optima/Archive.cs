using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Optima
{
   public class Archive
   {
      private List< Contractor >                m_uniqueContractors = new List< Contractor >();

      /**
       * Saves the list of contractors into the specified XML file.
       * 
       * @param outputDir
       * @param log
       */
      public void SaveContractors( string outputDir, Core.Logger log )
      {
         if ( m_uniqueContractors.Count <= 0 )
         {
            log.message( "No contractors remaining to be saved to a file." );
            return;
         }

         string fileName = outputDir+ "/contractors.csv";

         try
         {
            FileInfo file = new FileInfo( fileName );
            FileStream stream = file.Create();
            StreamWriter fileWriter = new StreamWriter( stream, Encoding.GetEncoding("windows-1250") );

            // write the header
            fileWriter.WriteLine( "Kod;Nazwa;Nazwa2;Nazwa3;Telefon;Telefon2;TelefonSms;Fax;Ulica;NrDomu;NrLokalu;KodPocztowy;Poczta;Miasto;Kraj;Wojewodztwo;Powiat;Gmina;URL;Grupa;OsobaFizyczna;NIP;NIPKraj;Zezwolenie;Regon;Pesel;Email;BankRachunekNr;BankNazwa;Osoba;Opis;Rodzaj;PlatnikVAT;Eksport;LimitKredytu;Termin;FormaPlatnosci;Ceny;PodatnikVatCzynny;CenyNazwa;Upust;NieNaliczajOdsetek;MetodaKasowa;WindykacjaEMail;WindykacjaTelefonSms" );
            foreach( Contractor contractor in m_uniqueContractors )
            {
               contractor.SaveToCSV( fileWriter );
            }
            fileWriter.Flush();

            log.warning( "Remaining contractors were saved to a file: " + fileName );
         }
         catch( Exception ex )
         {
            log.error( "Can't save the contractors file: " + fileName + " {" + ex.Message + "}" );
         }
      }

      /**
       * Adds a new invoice to the archive.
       * 
       * @param optimaInvoice 
       */
      public void Add( Invoice optimaInvoice )
      {
         if ( optimaInvoice != null )
         {
            // make a list of unique contractors
            AddContractor( optimaInvoice.GetContractor( Core.ContractorRole.Buyer ) );
            AddContractor( optimaInvoice.GetContractor( Core.ContractorRole.Issuer ) );

         }
      }

      private void AddContractor( Optima.Contractor contractor )
      {
         if ( contractor == null )
         {
            return;
         }

         if ( !m_uniqueContractors.Exists( element => ( contractor.hasNIPAssigned && element.m_NIP == contractor.m_NIP ) || ( element.m_code == contractor.m_code ) ) )
         {
            m_uniqueContractors.Add( contractor );
         }
      }

      public List< Contractor > contractors
      {
         get { return m_uniqueContractors; }
         set { m_uniqueContractors = value; }
      }

   }
}
